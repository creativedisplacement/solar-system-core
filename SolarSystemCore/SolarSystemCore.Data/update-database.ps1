param([string]$buildEnv = 'Development')
$env:ASPNETCORE_ENVIRONMENT = $buildEnv;
dotnet ef database update --project . --startup-project ../SolarSystemCore.WebApi