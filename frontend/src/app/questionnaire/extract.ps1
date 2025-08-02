# Questionnaire Module Extraction Script (PowerShell)
# This script helps extract the questionnaire module to other Angular projects

param(
    [Parameter(Mandatory=$true)]
    [string]$TargetDirectory
)

# Colors for output
$Red = "Red"
$Green = "Green"
$Yellow = "Yellow"

Write-Host "Questionnaire Module Extraction Script" -ForegroundColor $Green
Write-Host "==============================================" -ForegroundColor $Green

$SourceDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Source directory: $SourceDirectory" -ForegroundColor $Yellow
Write-Host "Target directory: $TargetDirectory" -ForegroundColor $Yellow

# Check if target directory exists
if (-not (Test-Path $TargetDirectory)) {
    Write-Host "Error: Target directory does not exist" -ForegroundColor $Red
    exit 1
}

# Create questionnaire directory in target
$TargetQuestionnaireDir = Join-Path $TargetDirectory "questionnaire"
New-Item -ItemType Directory -Path $TargetQuestionnaireDir -Force | Out-Null

Write-Host "Copying questionnaire module..." -ForegroundColor $Green

# Copy all files and directories
Copy-Item -Path "$SourceDirectory\*" -Destination $TargetQuestionnaireDir -Recurse -Force

Write-Host "✅ Questionnaire module extracted successfully!" -ForegroundColor $Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor $Yellow
Write-Host "1. Import the module in your app.module.ts:"
Write-Host "   import { QuestionnaireModule } from './questionnaire';"
Write-Host ""
Write-Host "2. Add it to your imports:"
Write-Host "   imports: [QuestionnaireModule]"
Write-Host ""
Write-Host "3. Install required dependencies:"
Write-Host "   npm install @angular/material @angular/cdk"
Write-Host ""
Write-Host "The questionnaire module is now ready to use!" -ForegroundColor $Green 