#!/bin/bash
# Build iOS Production with automatic date-based versioning
# Usage: ./scripts/build_ios_production.sh
# Note: Must be run on macOS

set -e

# Calculate date-based version with time for unique builds
YEAR=$(date +%Y)
MONTH=$(date +%-m)
DAY=$(date +%-d)
HOUR=$(date +%H)
MINUTE=$(date +%M)

# Version name: YYYY.M.DHHMM (e.g., 2026.2.61430) - no dot before time
VERSION_NAME="${YEAR}.${MONTH}.${DAY}${HOUR}${MINUTE}"

# Version code: YYYYMMDDHH (e.g., 2026020614) - max 2.1B, hourly granularity
VERSION_CODE=$(date +%Y%m%d%H)

echo "========================================"
echo "Building iOS Production"
echo "========================================"
echo "Version Name: $VERSION_NAME"
echo "Version Code: $VERSION_CODE"
echo "========================================"

# Build iOS (creates the Xcode project with correct version)
flutter build ios \
    --flavor production \
    -t lib/main_production.dart \
    --release \
    --build-name="$VERSION_NAME" \
    --build-number="$VERSION_CODE"

echo ""
echo "========================================"
echo "BUILD SUCCESSFUL!"
echo "========================================"
echo "Now open Xcode to archive and upload to App Store:"
echo "  open ios/Runner.xcworkspace"
echo "Version: $VERSION_NAME ($VERSION_CODE)"
