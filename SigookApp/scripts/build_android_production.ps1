# Build Android Production AAB with automatic date-based versioning
# Usage: .\scripts\build_android_production.ps1

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
Write-Host "Building Android Production AAB" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Version Name: $versionName" -ForegroundColor Green
Write-Host "Version Code: $versionCode" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

# Build the AAB
flutter build appbundle `
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
    Write-Host "AAB Location: build\app\outputs\bundle\productionRelease\app-production-release.aab" -ForegroundColor Yellow
    Write-Host "Version: $versionName ($versionCode)" -ForegroundColor Yellow
} else {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
