$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (![Environment]::Is64BitProcess) {
    throw 'GSA COM registration and preflight require 64-bit PowerShell, matching the integration tests.'
}

$gsaPath = Join-Path $env:ProgramFiles 'Oasys\GSA 10.3\GSA.exe'
if (!(Test-Path -LiteralPath $gsaPath -PathType Leaf)) {
    throw "GSA 10.3 executable not found: $gsaPath"
}

# GSA documents that COM automation with an active GSA window is unsupported.
$runningGsa = @(Get-Process -Name GSA -ErrorAction SilentlyContinue)
if ($runningGsa.Count -gt 0) {
    throw "GSA is already running (process IDs: $($runningGsa.Id -join ', ')). Use an idle dedicated runner; no existing processes were stopped."
}

Write-Host "Registering $gsaPath (file version $((Get-Item -LiteralPath $gsaPath).VersionInfo.FileVersion))"
$registration = Start-Process -FilePath $gsaPath -ArgumentList '/Register' -Wait -PassThru
if ($registration.ExitCode -ne 0) {
    throw "GSA COM registration failed with exit code $($registration.ExitCode)."
}

$clsid = '{77644F17-E6EB-49E3-AA35-39CC6A3484C9}'
$progId = 'Gsa_10_3.ComAuto'
$classRoot = 'Registry::HKEY_CLASSES_ROOT'
$registeredClsid = (Get-Item -LiteralPath "$classRoot\$progId\CLSID").GetValue('')
if ($registeredClsid -ne $clsid) {
    throw "$progId resolves to '$registeredClsid', not the integration test CLSID $clsid."
}

# HKCR is the effective registration, including per-user overrides of HKLM.
$serverKey = Get-Item -LiteralPath "$classRoot\CLSID\$clsid\LocalServer32"
$serverCommand = [string]$serverKey.GetValue('')
$serverExecutable = [string]$serverKey.GetValue('ServerExecutable')
if ([string]::IsNullOrWhiteSpace($serverExecutable)) {
    if ($serverCommand -match '^\s*"([^"]+\.exe)"(?:\s|$)') {
        $serverExecutable = $Matches[1]
    } elseif ($serverCommand -match '^\s*(.+?\.exe)(?:\s|$)') {
        $serverExecutable = $Matches[1]
    } else {
        throw "Cannot determine the GSA COM executable from LocalServer32: '$serverCommand'."
    }
}

Write-Host "Effective $clsid LocalServer32: $serverCommand; ServerExecutable: $serverExecutable"
if (![string]::Equals($serverExecutable, $gsaPath, [StringComparison]::OrdinalIgnoreCase)) {
    throw "GSA COM resolves to '$serverExecutable', not the installed nightly '$gsaPath'. Check per-user COM overrides on the runner."
}

$gsa = $null
$modelOpened = $false
try {
    $gsa = New-Object -ComObject $progId
    $status = $gsa.NewFile()
    if ($status -ne 0) {
        throw "GSA COM NewFile failed with status $status."
    }
    $modelOpened = $true
    $version = $gsa.VersionString()
} finally {
    try {
        if ($modelOpened) {
            $status = $gsa.Close()
            if ($status -ne 0) {
                throw "GSA COM Close failed with status $status."
            }

            Write-Host "GSA COM preflight succeeded: $version"
        }
    } finally {
        if ($null -ne $gsa -and [Runtime.InteropServices.Marshal]::IsComObject($gsa)) {
            [void][Runtime.InteropServices.Marshal]::FinalReleaseComObject($gsa)
        }
    }
}
