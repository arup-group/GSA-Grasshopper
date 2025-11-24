function Has-Version {
    param ($version)

    # Check if the version argument is provided
    if ($version.Count -eq 0) {
        Write-Host "Please provide the version number as an argument. Usage: .\bump-version.ps1 <new-version>"
        exit
    }

    # Get the new version from the CLI argument
    return $version[0]
}

$newVersion = Has-Version($args)

# Function to validate the version format (X.X.X where X is a number)
function Validate-VersionFormat {
    param (
        [string]$version
    )

    # Regex pattern for validating version format (X.X.X)
    $versionPattern = '^\d+\.\d+\.\d+$'

    # Check if version matches the pattern
    return $version -match $versionPattern
}

# Function to update version in a file
function Update-Version {
    param (
        [string]$filePath,
        [string]$searchPattern,
        [string]$newVersion,
        [string]$replacementPattern
    )

    # Read the content of the file
    $content = Get-Content $filePath -Encoding UTF8

    # Replace the version based on the provided pattern and replacement
    $updatedContent = $content -replace $searchPattern, $replacementPattern
	$updatedContent = $updatedContent -replace "`r`n", "`n"
	
    # Write the updated content back to the file
    Set-Content $filePath -Value $updatedContent -Encoding UTF8 -Force

    Write-Host "Updated version in $filePath to $newVersion"
}

# Function to update the copyright year
# Function to update the copyright year
function Update-CopyrightYear {
    param (
        [string]$filePath,
        [string]$currentYear
    )

    Write-Host "Processing file: $filePath"

    # Read the content of the file
    $content = Get-Content $filePath -Encoding UTF8

    # Define the simpler pattern for "Oasys 1985 - <oldYear>"
    # We are looking for the year format "Oasys 1985 - <4-digit year>"
    $searchPattern = "1985 - \d{4}"

    Write-Host "Searching for copyright year in file $filePath using pattern: $searchPattern"

    # Check if the content matches the search pattern
    if ($content -match $searchPattern) {
        Write-Host "Found copyright year: $matches"

        # Define the replacement pattern using the current year
        $replacementPattern = "1985 - $currentYear"

        Write-Host "Replacing year with: $replacementPattern"

        # Replace the old year with the current year in the copyright text
        $updatedContent = $content -replace $searchPattern, $replacementPattern
		$updatedContent = $updatedContent -replace "`r`n", "`n"

        # Write the updated content back to the file
        Set-Content $filePath -Value $updatedContent -Encoding UTF8 -Force

        Write-Host "Updated copyright year in $filePath to $currentYear"
    } else {
        Write-Host "No copyright year found in $filePath. Skipping update."
    }
}

# Check if the version format is valid
if (-not (Validate-VersionFormat $newVersion)) {
    Write-Host "Invalid version format. Please use the format: X.X.X where X is a number."
    exit
}

# Define the paths and patterns for each file (version updates)
$filesToUpdate = @(
    @{
        FilePath = ".\GsaGH\GsaGH.csproj"
        SearchPattern = '<Version>(.*?)<\/Version>'
        ReplacementPattern = "<Version>$newVersion</Version>"
    },
    @{
        FilePath = ".\GsaGH\GsaGHInfo.cs"
        SearchPattern = 'string GrasshopperVersion = "(.*?)"'
        ReplacementPattern = 'string GrasshopperVersion = "' + $newVersion + '"'
    }
)

# Loop through each file and update the version
foreach ($file in $filesToUpdate) {
    Update-Version -filePath $file.FilePath -searchPattern $file.SearchPattern -newVersion $newVersion -replacementPattern $file.ReplacementPattern
}

# Define the paths of the files for copyright year update
$filesWithCopyright = @(
    ".\GsaGH\GsaGH.csproj",
    ".\GsaGH\GsaGHInfo.cs",
    ".\GsaGH\UI\AboutBox.cs",
    ".\GsaGHTests\UI\AboutBoxTests.cs"
)

# Get the current year
$currentYear = (Get-Date).Year

# Loop through each file and update the copyright year
foreach ($filePath in $filesWithCopyright) {
    Update-CopyrightYear -filePath $filePath -currentYear $currentYear
}

Write-Host "Version and copyright year update completed."
