# Build iOS Production with automatic date-based versioning
# Usage: .\scripts\build_ios_production.ps1
# Note: Must be run on macOS for actual iOS builds

$ErrorActionPreference = "Stop"

# Calculate date-based version
$year = (Get-Date).Year
$month = (Get-Date).Month
$day = (Get-Date).Day

# Version name: YYYY.M.D (e.g., 2026.2.6)
$versionName = "$year.$month.$day"

# Version code: YYYYMMDD (e.g., 20260206) - always increasing
$versionCode = "{0:D4}{1:D2}{2:D2}" -f $year, $month, $day

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building iOS Production" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Version Name: $versionName" -ForegroundColor Green
Write-Host "Version Code: $versionCode" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# Build iOS (creates the Xcode project with correct version)
flutter build ios `
    --flavor production `
    -t lib/main_production.dart `
    --release `
    --build-name="$versionName" `
    --build-number="$versionCode"

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "BUILD SUCCESSFUL!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Now open Xcode to archive and upload to App Store:" -ForegroundColor Yellow
    Write-Host "  open ios/Runner.xcworkspace" -ForegroundColor Yellow
    Write-Host "Version: $versionName ($versionCode)" -ForegroundColor Yellow
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
