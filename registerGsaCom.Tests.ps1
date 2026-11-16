$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Shadow every command that could launch GSA or access its installation/registration.
function Test-Path {
    param($LiteralPath, $PathType)
    return $testState.Scenario -ne 'missing executable'
}

function Get-Process {
    param($Name, $ErrorAction)
    if ($testState.Scenario -eq 'active GSA') {
        [pscustomobject]@{ Id = 123 }
    }
}

function Start-Process {
    param($FilePath, $ArgumentList, [switch]$Wait, [switch]$PassThru)
    $expectedPath = Join-Path $env:ProgramFiles 'Oasys\GSA 10.3\GSA.exe'
    if ($FilePath -ne $expectedPath -or $ArgumentList -ne '/Register' -or !$Wait -or !$PassThru) {
        throw 'Registration must use the exact executable and wait for its exit code.'
    }
    $testState.Registrations++
    [pscustomobject]@{ ExitCode = $(if ($testState.Scenario -eq 'registration failure') { 5 } else { 0 }) }
}

function Get-Item {
    param($LiteralPath)
    if ($LiteralPath -like '*.exe') {
        return [pscustomobject]@{ VersionInfo = [pscustomobject]@{ FileVersion = 'test version' } }
    }
    $key = [pscustomobject]@{ Path = $LiteralPath }
    $key | Add-Member -MemberType ScriptMethod -Name GetValue -Value {
        param($Name)
        if ($this.Path -like '*Gsa_10_3.ComAuto\CLSID') {
            if ($testState.Scenario -eq 'wrong CLSID') { return '{00000000-0000-0000-0000-000000000000}' }
            return '{77644F17-E6EB-49E3-AA35-39CC6A3484C9}'
        }
        if ($this.Path -ne 'Registry::HKEY_CLASSES_ROOT\CLSID\{77644F17-E6EB-49E3-AA35-39CC6A3484C9}\LocalServer32') {
            throw "Unexpected registry key: $($this.Path)"
        }
        $path = Join-Path $env:ProgramFiles 'Oasys\GSA 10.3\GSA.exe'
        if ($testState.Scenario -eq 'stale server') { $path = 'C:\Old GSA\GSA.exe' }
        if ($Name -eq 'ServerExecutable') {
            if ($testState.Scenario -eq 'ServerExecutable override') { return 'C:\Old GSA\GSA.exe' }
            return $null
        }
        if ($testState.Scenario -eq 'unquoted server') { return "$path /Automation" }
        return "`"$path`" /Automation"
    }
    return $key
}

function New-Object {
    param($ComObject)
    if ($ComObject -ne 'Gsa_10_3.ComAuto') { throw "Unexpected COM class: $ComObject" }
    $testState.Activations++
    if ($testState.Scenario -eq 'activation failure') {
        throw [Runtime.InteropServices.COMException]::new('Test activation failed', -2146959355)
    }
    $com = [pscustomobject]@{}
    $com | Add-Member -MemberType ScriptMethod -Name NewFile -Value {
        if ($testState.Scenario -eq 'NewFile failure') { return 1 }
        return 0
    }
    $com | Add-Member -MemberType ScriptMethod -Name VersionString -Value { return 'test version' }
    $com | Add-Member -MemberType ScriptMethod -Name Close -Value {
        $testState.Closes++
        if ($testState.Scenario -eq 'Close failure') { return 1 }
        return 0
    }
    return $com
}

$cases = @(
    @{ Name = 'quoted server'; Error = $null; Registrations = 1; Activations = 1; Closes = 1 }
    @{ Name = 'unquoted server'; Error = $null; Registrations = 1; Activations = 1; Closes = 1 }
    @{ Name = 'active GSA'; Error = '*GSA is already running*'; Registrations = 0; Activations = 0; Closes = 0 }
    @{ Name = 'missing executable'; Error = '*executable not found*'; Registrations = 0; Activations = 0; Closes = 0 }
    @{ Name = 'registration failure'; Error = '*registration failed with exit code 5*'; Registrations = 1; Activations = 0; Closes = 0 }
    @{ Name = 'wrong CLSID'; Error = '*not the integration test CLSID*'; Registrations = 1; Activations = 0; Closes = 0 }
    @{ Name = 'stale server'; Error = '*not the installed nightly*'; Registrations = 1; Activations = 0; Closes = 0 }
    @{ Name = 'ServerExecutable override'; Error = '*not the installed nightly*'; Registrations = 1; Activations = 0; Closes = 0 }
    @{ Name = 'activation failure'; Error = '*Test activation failed*'; Registrations = 1; Activations = 1; Closes = 0 }
    @{ Name = 'NewFile failure'; Error = '*NewFile failed with status 1*'; Registrations = 1; Activations = 1; Closes = 0 }
    @{ Name = 'Close failure'; Error = '*Close failed with status 1*'; Registrations = 1; Activations = 1; Closes = 1 }
)

foreach ($case in $cases) {
    $testState = [pscustomobject]@{ Scenario = $case.Name; Registrations = 0; Activations = 0; Closes = 0 }
    $failure = $null
    try {
        & "$PSScriptRoot\registerGsaCom.ps1"
    } catch {
        $failure = $_
    }
    if (($null -eq $case.Error -and $null -ne $failure) -or
        ($null -ne $case.Error -and ($null -eq $failure -or $failure.Exception.Message -notlike $case.Error))) {
        throw "Case '$($case.Name)': expected '$($case.Error)', got '$failure'."
    }
    if ($testState.Registrations -ne $case.Registrations -or
        $testState.Activations -ne $case.Activations -or $testState.Closes -ne $case.Closes) {
        throw "Case '$($case.Name)': unexpected registration, activation or cleanup calls."
    }
    if ($case.Name -eq 'activation failure' -and $failure.Exception.HResult -ne -2146959355) {
        throw 'The original COM activation HRESULT must be preserved.'
    }
    Write-Host "PASS: $($case.Name)"
}

Write-Host "$($cases.Count) isolated GSA COM setup cases passed; no GSA processes or registry entries were touched."
