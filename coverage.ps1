echo "Running coverage.ps1"


# Build
msbuild /p:AppxBundlePlatforms="x64" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /m /nr:false
# Run Tests
dotnet test --collect:"XPlat Code Coverage" /TestAdapterPath:$env:UserProfile\.nuget\packages\coverlet.collector\6.0.0\build --results-directory .\results\GsaGh .\GsaGHTests\bin\x64\Debug\net48\GsaGHTests.dll

dotnet test --collect:"XPlat Code Coverage" /TestAdapterPath:$env:UserProfile\.nuget\packages\coverlet.collector\6.0.0\build --results-directory .\results\integration  .\IntegrationTests\bin\x64\Debug\net48\IntegrationTests.dll

# find the latest report
$coverageFileGsaGh = Get-ChildItem -Path .\results\GsaGh\**\* -Filter 'coverage.cobertura.xml' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$coverageFileIntegration = Get-ChildItem -Path .\results\integration\**\* -Filter 'coverage.cobertura.xml' | Sort-Object LastWriteTime -Descending | Select-Object -First 1

# generate the report
reportgenerator -reports:$coverageFileGsaGh.FullName -targetdir:.\results\CoverageReportGsaGh\
reportgenerator -reports:$coverageFileIntegration.FullName -targetdir:.\results\CoverageReportIntegration\
# display report
.\results\CoverageReportIntegration\index.html
.\results\CoverageReportGsaGh\index.html