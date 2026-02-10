#!/bin/bash
# Build Android Production AAB with automatic date-based versioning
# Usage: ./scripts/build_android_production.sh

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
echo "Building Android Production AAB"
echo "========================================"
echo "Version Name: $VERSION_NAME"
echo "Version Code: $VERSION_CODE"
echo "========================================"

# Build the AAB
flutter build appbundle \
    --flavor production \
    -t lib/main_production.dart \
    --release \
    --build-name="$VERSION_NAME" \
    --build-number="$VERSION_CODE"

echo ""
echo "========================================"
echo "BUILD SUCCESSFUL!"
echo "========================================"
echo "AAB Location: build/app/outputs/bundle/productionRelease/app-production-release.aab"
echo "Version: $VERSION_NAME ($VERSION_CODE)"
