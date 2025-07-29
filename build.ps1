# Category-Based Questionnaire System Build Script
# This script helps set up and build the complete solution

Write-Host "Category-Based Questionnaire System - Build Script" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Check if .NET 8 is installed
Write-Host "Checking .NET 8 installation..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: .NET 8 is not installed. Please install .NET 8 SDK first." -ForegroundColor Red
    exit 1
}
Write-Host "Found .NET version: $dotnetVersion" -ForegroundColor Green

# Check if Node.js is installed
Write-Host "Checking Node.js installation..." -ForegroundColor Yellow
$nodeVersion = node --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Node.js is not installed. Please install Node.js 18+ first." -ForegroundColor Red
    exit 1
}
Write-Host "Found Node.js version: $nodeVersion" -ForegroundColor Green

# Check if Angular CLI is installed
Write-Host "Checking Angular CLI installation..." -ForegroundColor Yellow
$ngVersion = ng version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Installing Angular CLI..." -ForegroundColor Yellow
    npm install -g @angular/cli@18
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Failed to install Angular CLI." -ForegroundColor Red
        exit 1
    }
}
Write-Host "Angular CLI is ready." -ForegroundColor Green

# Restore .NET packages
Write-Host "Restoring .NET packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to restore .NET packages." -ForegroundColor Red
    exit 1
}
Write-Host ".NET packages restored successfully." -ForegroundColor Green

# Build .NET solution
Write-Host "Building .NET solution..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to build .NET solution." -ForegroundColor Red
    exit 1
}
Write-Host ".NET solution built successfully." -ForegroundColor Green

# Install frontend dependencies
Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
Set-Location frontend
npm install
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to install frontend dependencies." -ForegroundColor Red
    exit 1
}
Write-Host "Frontend dependencies installed successfully." -ForegroundColor Green

# Build frontend
Write-Host "Building frontend..." -ForegroundColor Yellow
ng build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to build frontend." -ForegroundColor Red
    exit 1
}
Write-Host "Frontend built successfully." -ForegroundColor Green

# Return to root directory
Set-Location ..

Write-Host "`nBuild completed successfully!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host "To run the backend:" -ForegroundColor Cyan
Write-Host "  cd src/QuestionnaireSystem.API" -ForegroundColor White
Write-Host "  dotnet run" -ForegroundColor White
Write-Host "`nTo run the frontend:" -ForegroundColor Cyan
Write-Host "  cd frontend" -ForegroundColor White
Write-Host "  npm start" -ForegroundColor White
Write-Host "`nBackend will be available at: https://localhost:7001" -ForegroundColor Cyan
Write-Host "Frontend will be available at: http://localhost:4200" -ForegroundColor Cyan 