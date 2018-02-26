param([string]$migrationName)
dotnet ef --project . --startup-project ../SolarSystemCore.WebApi database update $migrationName