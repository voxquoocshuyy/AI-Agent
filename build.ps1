# Build and publish the project
$ErrorActionPreference = "Stop"

Write-Host "Building solution..."
dotnet build -c Release

Write-Host "Running tests..."
dotnet test -c Release

Write-Host "Publishing API..."
dotnet publish src/AI.Agent.API/AI.Agent.API.csproj -c Release -o ./publish

Write-Host "Creating initial migration..."
dotnet ef migrations add InitialCreate --project src/AI.Agent.Infrastructure --startup-project src/AI.Agent.API

Write-Host "Applying migration..."
dotnet ef database update --project src/AI.Agent.Infrastructure --startup-project src/AI.Agent.API

Write-Host "Build completed successfully!" 