echo "Running coverage.ps1"


# Build
msbuild /p:AppxBundlePlatforms="x64" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /m /nr:false
# Run Tests

# find the latest report
$coverageFileGsaGh = Get-ChildItem -Path .\results\GsaGh\**\* -Filter 'coverage.cobertura.xml' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$coverageFileIntegration = Get-ChildItem -Path .\results\integration\**\* -Filter 'coverage.cobertura.xml' | Sort-Object LastWriteTime -Descending | Select-Object -First 1

# generate the report
reportgenerator -reports:$coverageFileGsaGh.FullName -targetdir:.\results\CoverageReportGsaGh\
reportgenerator -reports:$coverageFileIntegration.FullName -targetdir:.\results\CoverageReportIntegration\
# display report
.\results\CoverageReportIntegration\index.html
.\results\CoverageReportGsaGh\index.html
