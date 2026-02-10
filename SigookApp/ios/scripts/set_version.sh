#!/bin/bash

# This script sets the app version based on the current date
# Format: YYYY.M.D (e.g., 2026.2.5)
# Build number: Uses git commit count or timestamp for uniqueness

# Calculate date-based version
YEAR=$(date +%Y)
MONTH=$(date +%-m)
DAY=$(date +%-d)
APP_VERSION="${YEAR}.${MONTH}.${DAY}"

# Calculate build number (git commit count, or timestamp if not in git)
if git rev-parse --git-dir > /dev/null 2>&1; then
    BUILD_NUMBER=$(git rev-list --count HEAD 2>/dev/null || echo "1")
else
    BUILD_NUMBER=$(date +%s)
fi

echo "Setting version to ${APP_VERSION} (${BUILD_NUMBER})"

# Update Info.plist
INFO_PLIST="${BUILT_PRODUCTS_DIR}/${INFOPLIST_PATH}"
if [ -f "$INFO_PLIST" ]; then
    /usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString ${APP_VERSION}" "${INFO_PLIST}"
    /usr/libexec/PlistBuddy -c "Set :CFBundleVersion ${BUILD_NUMBER}" "${INFO_PLIST}"
    echo "Updated ${INFO_PLIST}"
else
    echo "Info.plist not found at ${INFO_PLIST}, will be set during build"
fi

# Also update the source Info.plist for archiving
SOURCE_INFO_PLIST="${PROJECT_DIR}/Runner/Info.plist"
if [ -f "$SOURCE_INFO_PLIST" ]; then
    /usr/libexec/PlistBuddy -c "Set :CFBundleShortVersionString ${APP_VERSION}" "${SOURCE_INFO_PLIST}"
    /usr/libexec/PlistBuddy -c "Set :CFBundleVersion ${BUILD_NUMBER}" "${SOURCE_INFO_PLIST}"
    echo "Updated source Info.plist: ${SOURCE_INFO_PLIST}"
fi
