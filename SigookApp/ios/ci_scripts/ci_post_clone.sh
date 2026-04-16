#!/usr/bin/env bash
set -euo pipefail

echo "==============================="
echo " Xcode Cloud Post-Clone Script"
echo "==============================="

echo "Working directory: $(pwd)"
echo "CI Branch: ${CI_BRANCH:-unknown}"

# ---------------------------------------
# Resolve project directory automatically
# ---------------------------------------
REPO_PATH="$CI_PRIMARY_REPOSITORY_PATH"

echo "Repository path: $REPO_PATH"
echo "Listing repository contents:"
ls -la "$REPO_PATH"

# If project is inside a subfolder, detect it automatically
if [ -d "$REPO_PATH/SigookApp" ]; then
  PROJECT_PATH="$REPO_PATH/SigookApp"
else
  PROJECT_PATH="$REPO_PATH"
fi

echo "Using project path: $PROJECT_PATH"
cd "$PROJECT_PATH"

# ---------------------------------------
# Environment selection
# ---------------------------------------
if [[ "${CI_BRANCH:-}" == "main" ]]; then
  ENVIRONMENT="production"
  ENTRY_POINT="lib/main_production.dart"
else
  ENVIRONMENT="staging"
  ENTRY_POINT="lib/main_staging.dart"
fi

echo "Environment: $ENVIRONMENT"
echo "Entry point: $ENTRY_POINT"

# ---------------------------------------
# Validate environment variables
# ---------------------------------------
REQUIRED_VARS=(
  AUTH_AUTHORITY
  CLIENT_ID
  REDIRECT_URI
  POST_LOGOUT_REDIRECT_URI
  API_BASE_URL
  SCOPES
  APP_NAME
  APP_INSIGHTS_CONNECTION_STRING
)

echo "Validating environment variables..."

for VAR in "${REQUIRED_VARS[@]}"; do
  eval "VALUE=\${$VAR:-}"
  if [[ -z "$VALUE" ]]; then
    echo "❌ ERROR: Missing environment variable: $VAR"
    exit 1
  fi
done

echo "All environment variables are present ✅"

# ---------------------------------------
# Setup Flutter
# ---------------------------------------
echo "Checking Flutter installation..."

if ! command -v flutter >/dev/null 2>&1; then
  echo "Flutter not found – installing stable Flutter SDK..."
  git clone https://github.com/flutter/flutter.git --depth 1 -b stable "$HOME/flutter"
  export PATH="$PATH:$HOME/flutter/bin"
else
  echo "Flutter already available"
fi

flutter --version

# ---------------------------------------
# Pre-build steps
# ---------------------------------------
flutter config --enable-ios
flutter precache --ios
flutter pub get

# ---------------------------------------
# pod install with retry (handles transient GitHub/CDN 502 errors)
# ---------------------------------------
pod_install_with_retry() {
  local attempt=1
  local max_attempts=3
  until (cd "$PROJECT_PATH/ios" && pod install) ; do
    if [ $attempt -ge $max_attempts ]; then
      echo "❌ pod install failed after $max_attempts attempts"
      exit 1
    fi
    echo "⚠️  pod install failed (attempt $attempt/$max_attempts), retrying in 15s..."
    attempt=$((attempt + 1))
    sleep 15
  done
  echo "✅ pod install succeeded (attempt $attempt)"
}

pod_install_with_retry

# ---------------------------------------
# Build versioning
# ---------------------------------------
VERSION_NAME="$(date +%Y).$(date +%-m).$(date +%-d)"
VERSION_CODE="${CI_BUILD_NUMBER:-1}"

echo "Build Version: $VERSION_NAME ($VERSION_CODE)"

# ---------------------------------------
# Flutter iOS build
# ---------------------------------------
echo "Starting Flutter iOS build..."

flutter build ios --release \
  -t "$ENTRY_POINT" \
  --no-codesign \
  --build-name="$VERSION_NAME" \
  --build-number="$VERSION_CODE" \
  --dart-define=ENVIRONMENT="$ENVIRONMENT" \
  --dart-define=AUTH_AUTHORITY="$AUTH_AUTHORITY" \
  --dart-define=API_BASE_URL="$API_BASE_URL" \
  --dart-define=CLIENT_ID="$CLIENT_ID" \
  --dart-define=REDIRECT_URI="$REDIRECT_URI" \
  --dart-define=POST_LOGOUT_REDIRECT_URI="$POST_LOGOUT_REDIRECT_URI" \
  --dart-define=SCOPES="$SCOPES" \
  --dart-define=APP_NAME="$APP_NAME" \
  --dart-define=APP_INSIGHTS_CONNECTION_STRING="$APP_INSIGHTS_CONNECTION_STRING"



echo "================================"
echo " Flutter iOS build completed ✅"
echo "================================"
