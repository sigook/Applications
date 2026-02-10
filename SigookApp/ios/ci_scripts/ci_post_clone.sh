#!/bin/sh
set -e

echo "=== Xcode Cloud Post-Clone Script ==="

# Determine environment based on branch
if [ "$CI_BRANCH" = "main" ]; then
  ENVIRONMENT="production"
  ENTRY_POINT="lib/main_production.dart"
else
  ENVIRONMENT="staging"
  ENTRY_POINT="lib/main_staging.dart"
fi

echo "Branch: $CI_BRANCH"
echo "Environment: $ENVIRONMENT"
echo "Entry Point: $ENTRY_POINT"

# Navigate to Flutter project root
cd "$CI_PRIMARY_REPOSITORY_PATH/SigookApp"

# Generate .env file from Xcode Cloud environment variables
ENV_FILE=".env.${ENVIRONMENT}"
echo "Generating $ENV_FILE..."
cat > "$ENV_FILE" <<EOF
ENVIRONMENT=$ENVIRONMENT
AUTH_AUTHORITY=$AUTH_AUTHORITY
CLIENT_ID=$CLIENT_ID
REDIRECT_URI=$REDIRECT_URI
POST_LOGOUT_REDIRECT_URI=$POST_LOGOUT_REDIRECT_URI
API_BASE_URL=$API_BASE_URL
SCOPES=$SCOPES
APP_NAME=$APP_NAME
EOF

echo "Generated $ENV_FILE"

# Install Flutter
git clone https://github.com/flutter/flutter.git --depth 1 -b stable "$HOME/flutter"
export PATH="$PATH:$HOME/flutter/bin"

flutter precache --ios
flutter pub get

# Calculate version (same format as Android pipeline)
VERSION_NAME="$(date +%Y).$(date +%-m).$(date +%-d)"
VERSION_CODE="$CI_BUILD_NUMBER"
echo "Version: $VERSION_NAME ($VERSION_CODE)"

# Build Flutter for iOS (no codesign — Xcode Cloud handles signing)
flutter build ios --release \
  --flavor production \
  -t "$ENTRY_POINT" \
  --no-codesign \
  --build-name="$VERSION_NAME" \
  --build-number="$VERSION_CODE"
  
echo "=== Flutter build complete ==="
