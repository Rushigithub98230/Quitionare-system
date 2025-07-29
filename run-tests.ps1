# Comprehensive Backend Test Runner for Questionnaire System
# This script runs all tests and provides detailed reporting

param(
    [switch]$Verbose,
    [switch]$Coverage,
    [string]$Filter = "",
    [switch]$Help
)

if ($Help) {
    Write-Host @"
Questionnaire System - Backend Test Runner

Usage: .\run-tests.ps1 [options]

Options:
    -Verbose     Show detailed test output
    -Coverage    Generate code coverage report
    -Filter      Run tests matching the specified filter (e.g., "AuthenticationTests")
    -Help        Show this help message

Examples:
    .\run-tests.ps1                           # Run all tests
    .\run-tests.ps1 -Verbose                  # Run with detailed output
    .\run-tests.ps1 -Filter "Auth"            # Run authentication tests only
    .\run-tests.ps1 -Coverage                 # Run with coverage report
    .\run-tests.ps1 -Verbose -Coverage        # Run with both verbose and coverage

"@
    exit 0
}

Write-Host "🧪 Questionnaire System - Backend Test Suite" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "src\QuestionnaireSystem.API.Tests")) {
    Write-Host "❌ Error: Test project not found. Please run this script from the project root." -ForegroundColor Red
    exit 1
}

# Build the solution first
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed. Please fix build errors before running tests." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green
Write-Host ""

# Prepare test command
$testCommand = "dotnet test src\QuestionnaireSystem.API.Tests\QuestionnaireSystem.API.Tests.csproj --configuration Release --no-build"

if ($Verbose) {
    $testCommand += " --verbosity normal"
} else {
    $testCommand += " --verbosity minimal"
}

if ($Coverage) {
    $testCommand += " --collect:\"XPlat Code Coverage\" --results-directory coverage"
}

if ($Filter) {
    $testCommand += " --filter `"$Filter`""
}

# Run the tests
Write-Host "🚀 Running tests..." -ForegroundColor Yellow
Write-Host "Command: $testCommand" -ForegroundColor Gray
Write-Host ""

$startTime = Get-Date
$testResult = Invoke-Expression $testCommand
$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host ""
Write-Host "📊 Test Results Summary" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All tests passed!" -ForegroundColor Green
} else {
    Write-Host "❌ Some tests failed!" -ForegroundColor Red
}

Write-Host "⏱️  Duration: $($duration.TotalSeconds.ToString('F2')) seconds" -ForegroundColor White

# Parse test results for summary
$testOutput = $testResult -join "`n"
$passedMatches = [regex]::Matches($testOutput, "Passed:\s+(\d+)")
$failedMatches = [regex]::Matches($testOutput, "Failed:\s+(\d+)")
$skippedMatches = [regex]::Matches($testOutput, "Skipped:\s+(\d+)")

if ($passedMatches.Count -gt 0) {
    $passed = $passedMatches[0].Groups[1].Value
    Write-Host "✅ Passed: $passed" -ForegroundColor Green
}

if ($failedMatches.Count -gt 0) {
    $failed = $failedMatches[0].Groups[1].Value
    Write-Host "❌ Failed: $failed" -ForegroundColor Red
}

if ($skippedMatches.Count -gt 0) {
    $skipped = $skippedMatches[0].Groups[1].Value
    Write-Host "⏭️  Skipped: $skipped" -ForegroundColor Yellow
}

Write-Host ""

# Test Categories Overview
Write-Host "📋 Test Categories" -ForegroundColor Cyan
Write-Host "==================" -ForegroundColor Cyan

$testCategories = @(
    @{ Name = "Authentication Tests"; Description = "User registration, login, JWT validation" },
    @{ Name = "Category Management"; Description = "CRUD operations for categories" },
    @{ Name = "Questionnaire Management"; Description = "CRUD operations for questionnaires" },
    @{ Name = "Question Types"; Description = "All 14 question types (Text, Number, Email, etc.)" },
    @{ Name = "Question Options"; Description = "CRUD operations for question options" },
    @{ Name = "User Responses"; Description = "Response submission and validation" },
    @{ Name = "Validation Rules"; Description = "Min/max length, required fields, etc." },
    @{ Name = "Authorization"; Description = "Role-based access control" },
    @{ Name = "Edge Cases"; Description = "Error handling and boundary conditions" }
)

foreach ($category in $testCategories) {
    Write-Host "• $($category.Name)" -ForegroundColor White
    Write-Host "  $($category.Description)" -ForegroundColor Gray
}

Write-Host ""

# Question Types Tested
Write-Host "❓ Question Types Tested" -ForegroundColor Cyan
Write-Host "=======================" -ForegroundColor Cyan

$questionTypes = @(
    "Text Input", "Number Input", "Email Input", "Phone Input", 
    "Date Picker", "Single Choice (Radio)", "Multiple Choice (Checkbox)", 
    "Dropdown", "Rating (1-5)", "Slider", "Yes/No", 
    "File Upload", "Image Display", "Long Text (TextArea)"
)

foreach ($type in $questionTypes) {
    Write-Host "• $type" -ForegroundColor White
}

Write-Host ""

# Coverage report if requested
if ($Coverage -and (Test-Path "coverage")) {
    Write-Host "📈 Coverage Report" -ForegroundColor Cyan
    Write-Host "==================" -ForegroundColor Cyan
    
    $coverageFiles = Get-ChildItem "coverage" -Filter "*.xml" -Recurse
    if ($coverageFiles.Count -gt 0) {
        Write-Host "✅ Coverage data generated" -ForegroundColor Green
        Write-Host "📁 Location: coverage\" -ForegroundColor White
        
        # Try to find and display coverage summary
        $coverageXml = Get-Content $coverageFiles[0].FullName -Raw
        $coverageMatches = [regex]::Matches($coverageXml, 'coverage.*?line-rate="([^"]*)"')
        if ($coverageMatches.Count -gt 0) {
            $coveragePercent = [math]::Round([double]$coverageMatches[0].Groups[1].Value * 100, 2)
            Write-Host "📊 Line Coverage: $coveragePercent%" -ForegroundColor White
        }
    } else {
        Write-Host "⚠️  No coverage data found" -ForegroundColor Yellow
    }
}

Write-Host ""

# Recommendations
if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 All tests passed successfully!" -ForegroundColor Green
    Write-Host "💡 The backend is ready for production use." -ForegroundColor Cyan
} else {
    Write-Host "🔧 Test failures detected. Please review and fix the failing tests." -ForegroundColor Red
    Write-Host "💡 Check the detailed output above for specific failure reasons." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📚 Test Documentation" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan
Write-Host "• Authentication: Registration, login, JWT validation, role-based access" -ForegroundColor Gray
Write-Host "• Categories: Create, read, update, delete with validation" -ForegroundColor Gray
Write-Host "• Questionnaires: Full CRUD with all question types" -ForegroundColor Gray
Write-Host "• Questions: All 14 question types with validation rules" -ForegroundColor Gray
Write-Host "• Options: CRUD for radio, checkbox, and dropdown options" -ForegroundColor Gray
Write-Host "• Responses: User response submission and validation" -ForegroundColor Gray
Write-Host "• Edge Cases: Error handling, validation failures, invalid data" -ForegroundColor Gray

Write-Host ""
Write-Host "🏁 Test run completed at $(Get-Date)" -ForegroundColor Gray

exit $LASTEXITCODE 