$registryPath = 'HKCU:\Software\Classes\CLSID\{92a4b120-dc52-11df-937b-0800200c9a66}\LocalServer32\XL^'

# Check if the registry path exists
if (Test-Path $registryPath) {
    # Remove the registry key and all subkeys
    Remove-Item -Path $registryPath -Recurse -Force
    Write-Host "Registry key $registryPath and its subkeys have been deleted."
} else {
    Write-Host "Registry path $registryPath does not exist."
}
