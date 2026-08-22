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

# The app resolves the worker role from /connect/userinfo, which only returns
# the role claim when the 'roles' scope was requested. Without it sign-in still
# succeeds but the worker-only gate silently degrades, so fail the build here.
for REQUIRED_SCOPE in openid profile api1 roles offline_access; do
  if [[ ",${SCOPES}," != *",${REQUIRED_SCOPE},"* ]]; then
    echo "❌ ERROR: SCOPES is missing '${REQUIRED_SCOPE}' (current: ${SCOPES})"
    echo "   Expected: openid,profile,api1,roles,offline_access"
    exit 1
  fi
done
echo "SCOPES validated ✅"

# ---------------------------------------
# Setup Flutter (pinned - do NOT use 'stable')
# ---------------------------------------
# Pinned on purpose: an unpinned 'stable' clone silently upgraded CI to a
# Flutter release whose engine requires a higher iOS deployment target than
# the one declared in ios/Podfile, breaking pod install. Bump this only
# together with the iOS deployment target.
FLUTTER_VERSION="3.41.3"
FLUTTER_HOME="$HOME/flutter"

echo "Ensuring Flutter $FLUTTER_VERSION is installed..."

if [ ! -x "$FLUTTER_HOME/bin/flutter" ]; then
  echo "Installing Flutter $FLUTTER_VERSION..."
  rm -rf "$FLUTTER_HOME"
  git clone https://github.com/flutter/flutter.git --depth 1 -b "$FLUTTER_VERSION" "$FLUTTER_HOME"
else
  echo "Reusing existing Flutter at $FLUTTER_HOME"
fi

# Prepend so the pinned SDK wins over any preinstalled Flutter on the runner
export PATH="$FLUTTER_HOME/bin:$PATH"

flutter --version

INSTALLED_VERSION="$(flutter --version --machine | grep -o '"frameworkVersion":[^,]*' | head -1 | cut -d'"' -f4)"
if [ "$INSTALLED_VERSION" != "$FLUTTER_VERSION" ]; then
  echo "❌ ERROR: expected Flutter $FLUTTER_VERSION but found ${INSTALLED_VERSION:-unknown}"
  exit 1
fi
echo "✅ Flutter $FLUTTER_VERSION confirmed"

# ---------------------------------------
# Pre-build steps
# ---------------------------------------
echo "Cleaning and resetting dependencies to fix enum matching..."
flutter clean
flutter pub cache clean --force
flutter config --enable-ios
flutter precache --ios
flutter pub get

# ---------------------------------------
# Run Flutter tests
# ---------------------------------------
echo "Running Flutter tests..."
flutter test --reporter=expanded
echo "✅ Flutter tests passed"

# ---------------------------------------
# pod install with retry (handles transient GitHub/CDN 502 errors)
# ---------------------------------------
pod_install_with_retry() {
  local attempt=1
  local max_attempts=3
  local output
  while true; do
    if output="$(cd "$PROJECT_PATH/ios" && pod install 2>&1)"; then
      echo "$output"
      echo "✅ pod install succeeded (attempt $attempt)"
      return 0
    fi
    echo "$output"
    # Dependency-resolution errors are deterministic: retrying only wastes time
    if ! echo "$output" | grep -qiE "timed out|timeout|502|503|could not connect|network is unreachable|failed to fetch|temporarily unavailable"; then
      echo "❌ pod install failed with a non-transient error - not retrying"
      exit 1
    fi
    if [ $attempt -ge $max_attempts ]; then
      echo "❌ pod install failed after $max_attempts attempts"
      exit 1
    fi
    echo "⚠️  transient pod install failure (attempt $attempt/$max_attempts), retrying in 15s..."
    attempt=$((attempt + 1))
    sleep 15
  done
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