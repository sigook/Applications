#!/bin/bash
# Build Android Production AAB with automatic date-based versioning
# Usage: ./scripts/build_android_production.sh

set -e

# Calculate date-based version
YEAR=$(date +%Y)
MONTH=$(date +%-m)
DAY=$(date +%-d)

# Version name: YYYY.M.D (e.g., 2026.2.6)
VERSION_NAME="${YEAR}.${MONTH}.${DAY}"

# Version code: YYYYMMDD (e.g., 20260206) - always increasing
VERSION_CODE=$(date +%Y%m%d)

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
