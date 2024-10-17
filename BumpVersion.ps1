
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
    $content = Get-Content $filePath

    # Replace the version based on the provided pattern and replacement
    $updatedContent = $content -replace $searchPattern, $replacementPattern

    # Write the updated content back to the file
    Set-Content $filePath -Value $updatedContent

    Write-Host "Updated version in $filePath to $newVersion"
}

# Check if the version format is valid
if (-not (Validate-VersionFormat $newVersion)) {
    Write-Host "Invalid version format. Please use the format: X.X.X where X is a number."
    exit
}

# Define the paths and patterns for each file
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

Write-Host "Version update completed."