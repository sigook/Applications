#!/usr/bin/env bash
# Captures the store screenshots on one device by driving
# integration_test/store_screenshots_test.dart against staging.
#
# usage: scripts/store_screenshots.sh <android|ios> <device-id> <output-dir> [file-prefix]
#   device-id   adb serial (emulator-5554) or simulator UDID
#   output-dir  relative to Sigook.App/ or a native absolute path (the driver writes there)
#   file-prefix optional, e.g. "ipad_" so iPhone and iPad files can share a folder
# env: SCREENSHOT_USER, SCREENSHOT_PASSWORD (staging worker account)
#      SCREENSHOT_GEO  "lat,lon" the device reports (the punch card needs a fix; use the first job's address)
#      GOOGLE_MAPS_API_KEY  optional; without it the job detail capture shows no map image
#      ENV_FILE        default .env.staging
#      SCREENSHOT_SKIP_BUILD  "true" reuses the binary of a previous call (same
#                      platform, same dart-defines), e.g. iPad after iPhone
#      SCREENSHOT_ANDROID_SIZE     Android display size, default 1080x1920 (phone);
#                      1600x2560 for the tablet slots
#      SCREENSHOT_ANDROID_DENSITY  optional Android display density, e.g. 320 so a
#                      1600x2560 display is 800 dp wide and the app lays out as a tablet
#
# The app is built and installed before `flutter drive` so the location
# permission can be granted up front: the punch card tab requests it, and the
# native permission dialog pauses the app, which stalls the test for good.
set -euo pipefail

PLATFORM="${1:?android|ios}"
DEVICE="${2:?device id}"
OUT="${3:?output directory}"
PREFIX="${4:-}"
: "${SCREENSHOT_USER:?SCREENSHOT_USER is required}"
: "${SCREENSHOT_PASSWORD:?SCREENSHOT_PASSWORD is required}"

cd "$(dirname "$0")/.."
ENV_FILE="${ENV_FILE:-.env.staging}"
GEO="${SCREENSHOT_GEO:-37.697173,-77.669072}"
LAT="${GEO%,*}"
LON="${GEO#*,}"
mkdir -p "$OUT"

DART_DEFINES=(
  --dart-define-from-file="$ENV_FILE"
  --dart-define=SCREENSHOT_MODE=true
  --dart-define=SCREENSHOT_USER="$SCREENSHOT_USER"
  --dart-define=SCREENSHOT_PASSWORD="$SCREENSHOT_PASSWORD"
  --dart-define=GOOGLE_MAPS_API_KEY="${GOOGLE_MAPS_API_KEY:-}"
)
TARGET=integration_test/store_screenshots_test.dart
SKIP_BUILD="${SCREENSHOT_SKIP_BUILD:-false}"

build() {
  if [ "$SKIP_BUILD" = "true" ] && [ -e "$BINARY" ]; then
    echo "Reusing $BINARY"
  else
    "$@"
  fi
}

case "$PLATFORM" in
  android)
    PACKAGE=com.sigook.sigook
    BINARY=build/app/outputs/flutter-apk/app-debug.apk
    build flutter build apk --debug -t "$TARGET" "${DART_DEFINES[@]}"
    adb -s "$DEVICE" install -r "$BINARY"
    adb -s "$DEVICE" shell pm clear "$PACKAGE"
    adb -s "$DEVICE" shell pm grant "$PACKAGE" android.permission.ACCESS_FINE_LOCATION
    adb -s "$DEVICE" shell pm grant "$PACKAGE" android.permission.ACCESS_COARSE_LOCATION
    adb -s "$DEVICE" emu geo fix "$LON" "$LAT" || echo "geo fix skipped (not an emulator)"
    # The Flutter surface follows the window, so a temporary display size
    # (and density) yields captures of that size on any emulator
    adb -s "$DEVICE" shell wm size "${SCREENSHOT_ANDROID_SIZE:-1080x1920}"
    if [ -n "${SCREENSHOT_ANDROID_DENSITY:-}" ]; then
      adb -s "$DEVICE" shell wm density "$SCREENSHOT_ANDROID_DENSITY"
    fi
    trap 'adb -s "$DEVICE" shell wm size reset; adb -s "$DEVICE" shell wm density reset' EXIT
    ;;
  ios)
    BUNDLE=com.all2job.all2job
    BINARY=build/ios/iphonesimulator/Runner.app
    build flutter build ios --simulator --debug -t "$TARGET" "${DART_DEFINES[@]}"
    xcrun simctl uninstall "$DEVICE" "$BUNDLE" >/dev/null 2>&1 || true
    xcrun simctl install "$DEVICE" "$BINARY"
    xcrun simctl privacy "$DEVICE" grant location "$BUNDLE"
    xcrun simctl location "$DEVICE" set "$LAT,$LON"
    ;;
  *)
    echo "unknown platform '$PLATFORM' (android|ios)" >&2
    exit 1
    ;;
esac

SCREENSHOT_DIR="$OUT" SCREENSHOT_PREFIX="$PREFIX" flutter drive \
  -d "$DEVICE" \
  --use-application-binary="$BINARY" \
  --driver=test_driver/integration_test.dart \
  --target="$TARGET" \
  "${DART_DEFINES[@]}"

echo "Screenshots in $OUT:"
ls -la "$OUT"
