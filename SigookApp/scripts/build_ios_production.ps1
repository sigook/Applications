# Build iOS Production with automatic date-based versioning
# Usage: .\scripts\build_ios_production.ps1
# Note: Must be run on macOS for actual iOS builds

$ErrorActionPreference = "Stop"

# Calculate date-based version with time for unique builds
$now = Get-Date
$year = $now.Year
$month = $now.Month
$day = $now.Day
$hour = $now.Hour
$minute = $now.Minute

# Version name: YYYY.M.DHHMM (e.g., 2026.2.61430) - no dot before time
$versionName = "$year.$month.$day$("{0:D2}{1:D2}" -f $hour, $minute)"

# Version code: YYYYMMDDHH (e.g., 2026020614) - max 2.1B, hourly granularity
$versionCode = "{0:D4}{1:D2}{2:D2}{3:D2}" -f $year, $month, $day, $hour

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
