param([string]$migrationName)
dotnet ef --project . --startup-project ../SolarSystemCore.WebApi migrations add $migrationName