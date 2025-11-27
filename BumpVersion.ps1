function Has-Version {
    param ($version)

    if (-not $version) {
        Write-Host "Please provide the version number. Usage: .\bump-version.ps1 <new-version>"
        exit
    }

    $version[0]
}

$newVersion = Has-Version $args

function Validate-VersionFormat {
    param ([string]$version)
    $version -match '^\d+\.\d+\.\d+$'
}

function Update-FileContent {
    param (
        [string]$filePath,
        [string]$searchPattern,
        [string]$replacementPattern
    )

    $content = Get-Content $filePath -Encoding UTF8
    $updated = $content -replace $searchPattern, $replacementPattern
    $updated = $updated -replace "`r`n", "`n"

    Set-Content $filePath -Value $updated -Encoding UTF8 -Force
    Write-Host "Updated file '$filePath' using pattern '$searchPattern' to '$replacementPattern'"
}

if (-not (Validate-VersionFormat $newVersion)) {
    Write-Host "Invalid version format. Use X.X.X"
    exit
}

$currentYear = (Get-Date).Year

# Define updates
$updates = @(
    # Version updates
    @{
        FilePath = ".\GsaGH\GsaGH.csproj"
        SearchPattern = '<Version>(.*?)<\/Version>'
        ReplacementPattern = "<Version>$newVersion</Version>"
    },
    @{
        FilePath = ".\GsaGH\GsaGHInfo.cs"
        SearchPattern = 'string GrasshopperVersion = "(.*?)"'
        ReplacementPattern = "string GrasshopperVersion = `"$newVersion`""
    },

    # Year updates
    @{
        FilePath = ".\GsaGH\GsaGH.csproj"
        SearchPattern = '1985 - \d{4}'
        ReplacementPattern = "1985 - $currentYear"
    },
    @{
        FilePath = ".\GsaGH\GsaGHInfo.cs"
        SearchPattern = '1985 - \d{4}'
        ReplacementPattern = "1985 - $currentYear"
    },
    @{
        FilePath = ".\GsaGH\UI\AboutBox.cs"
        SearchPattern = '1985 - \d{4}'
        ReplacementPattern = "1985 - $currentYear"
    },
    @{
        FilePath = ".\GsaGHTests\UI\AboutBoxTests.cs"
        SearchPattern = '1985 - \d{4}'
        ReplacementPattern = "1985 - $currentYear"
    },
    @{
        FilePath = "LICENSE"
        SearchPattern = '2020-\d{4}'
        ReplacementPattern = "2020-$currentYear"
    }
)

foreach ($item in $updates) {
    Update-FileContent `
        -filePath $item.FilePath `
        -searchPattern $item.SearchPattern `
        -replacementPattern $item.ReplacementPattern
}

Write-Host "All updates completed."

