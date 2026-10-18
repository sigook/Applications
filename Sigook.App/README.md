# Sigook.App

Flutter mobile app for **workers** of the Sigook staffing platform (Android + iOS). A worker registers, completes their profile, browses and applies to jobs, clocks in and out of shifts, and follows their assignment history. It talks to `Covenant.Api`, which also acts as the OAuth/OpenID server.

This file covers **installing, running and building** the app. For how the code is organized see [`.docs/technical/SIGOOK_APP_STRUCTURE.md`](../.docs/technical/SIGOOK_APP_STRUCTURE.md); for the architecture rules every change must follow see [`CLAUDE.md`](./CLAUDE.md).

---

## 1. Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| Flutter SDK | **3.47.4** exactly | The pipeline pins this version and fails on any other. Download the zip from flutter.dev and add `flutter/bin` to `PATH`. |
| JDK | 17 | If your global `JAVA_HOME` is another version, point Flutter at it: `flutter config --jdk-dir "<path-to-jdk-17>"`. |
| Android Studio | latest | Brings the Android SDK, command-line tools, platform-tools and the emulator. In *SDK Manager* install **Android SDK Platform 37**, **Build-Tools**, **Command-line Tools** and **Android Emulator**. |
| Xcode + CocoaPods | latest | macOS only, needed for the iOS simulator and iOS builds. |
| Windows Developer Mode | on | Windows only. Without it `flutter pub get` aborts with *"Building with plugins requires symlink support"*. Enable it with `start ms-settings:developers`. |

Check everything at once:

```bash
flutter doctor
```

`[✓] Flutter`, `[✓] Android toolchain` and `[✓] Android Studio` must be green. The *Visual Studio* entry only matters for Windows desktop builds and can be ignored. If Android licences are pending run `flutter doctor --android-licenses`.

## 2. Quick Start — run the app on an Android emulator

Day-to-day development runs against **staging**: no backend to start, real data, orange "Staging" banner on top of every screen.

### 2.1 Get the dependencies

```bash
cd Sigook.App
flutter pub get
```

### 2.2 Create an emulator (once)

Easiest path: Android Studio → **Device Manager** → **Create Virtual Device** → pick a phone (Pixel 7) → pick a system image for **API 36** (download it if needed) → Finish.

Same thing from the terminal:

```bash
sdkmanager "system-images;android-36;google_apis;x86_64"
avdmanager create avd -n sigook_pixel -k "system-images;android-36;google_apis;x86_64" -d pixel_7
```

### 2.3 Boot the emulator

```bash
flutter emulators                          # lists the AVDs you created
flutter emulators --launch sigook_pixel    # boots one; wait until Android reaches the home screen
flutter devices                            # the emulator must appear here (e.g. "emulator-5554")
```

You can also press ▶ next to the device in Android Studio's Device Manager.

### 2.4 Run

```bash
flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart
```

The first build takes a few minutes (Gradle). When the app opens, sign in with a staging worker account.

While it runs: `r` hot reload, `R` hot restart, `q` quit. With several devices connected add `-d emulator-5554` (id from `flutter devices`).

If a red **"Initialization Failed"** screen lists missing keys, the `--dart-define-from-file` flag was omitted or does not match the entry point. The entry point and the `.env` file always go together (see §3).

### 2.5 Emulator tips

- A phone created from the terminal can come with a poor default config. In `~/.android/avd/<name>.avd/config.ini` set `hw.keyboard=yes` (host keyboard works inside Android) and `hw.gpu.enabled=yes` + `hw.gpu.mode=host` (hardware rendering), and raise `hw.ramSize` to 4096.
- `Ctrl+←` / `Ctrl+→` rotates the screen.
- A physical Android phone works the same way once *Developer options → USB debugging* is on; it shows up in `flutter devices`.

### iOS simulator (macOS)

```bash
open -a Simulator
flutter devices                            # pick the simulator id
flutter run -d <simulator-id> --dart-define-from-file=.env.staging -t lib/main_staging.dart
```

`flutter run` installs the CocoaPods on the first build.

## 3. Environments

The environment is chosen by **entry point + `.env` file**. There are no Android flavors and iOS has a single scheme; the pair below is the whole switch.

| Environment | Command | Backend | Banner |
|-------------|---------|---------|--------|
| Staging (default for development) | `flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart` | `staging.api.sigook.ca` | orange "Staging" |
| Local | `flutter run --dart-define-from-file=.env.local -t lib/main_local.dart` | your `Covenant.Api` (§4) | orange "Local" |
| Production (build only) | `flutter build apk --release --dart-define-from-file=.env.production -t lib/main_production.dart` | `api.sigook.ca` | none |

`.env.local`, `.env.staging` and `.env.production` are committed. They only hold public values (URLs, client id, scopes) that end up inside the binary anyway. `APP_INSIGHTS_CONNECTION_STRING` and `GOOGLE_MAPS_API_KEY` stay empty in them; CI injects them from the variable groups. Without the maps key the job location card shows the address and the "Open in Maps" button but no map image.

## 4. Running against a local backend

1. Start `Covenant.Api` with its default launch profile. It listens on `https://localhost:44307` **and** `http://localhost:5000`; the app uses the HTTP one. The Api also serves the OAuth endpoints, so nothing else is needed.
2. Run the app with the local pair:

   ```bash
   flutter run --dart-define-from-file=.env.local -t lib/main_local.dart
   ```

`.env.local` points at `http://10.0.2.2:5000`, which is how the Android emulator reaches the host machine. It is plain HTTP on purpose: the emulator does not trust the Kestrel development certificate, so HTTPS on `44307` fails with a connection error. Cleartext to `10.0.2.2` is allowed only in debug builds (`android/app/src/debug/res/xml/debug_network_security_config.xml`).

For other targets edit `.env.local` locally **without committing** the change:

| Target | `API_BASE_URL` | `AUTH_AUTHORITY` |
|--------|----------------|------------------|
| Android emulator | `http://10.0.2.2:5000/api/` | `http://10.0.2.2:5000/` |
| iOS simulator | `http://localhost:5000/api/` | `http://localhost:5000/` |
| Physical device | `http://<your-LAN-IP>:5000/api/` | `http://<your-LAN-IP>:5000/` |

A physical device also needs the Api listening on all interfaces and the LAN IP added to the debug network security config.

## 5. Debugging with VS Code

`.vscode/` is gitignored; create `Sigook.App/.vscode/launch.json` with the block below, open *Run & Debug* (`Ctrl+Shift+D` / `Cmd+Shift+D`), pick a configuration and press `F5`. **Debug — Staging** is the everyday one; **Debug — Local** needs the backend from §4; **Attach to Device** hooks the debugger to an app already running.

<details>
<summary><code>.vscode/launch.json</code></summary>

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Debug — Staging",
      "request": "launch",
      "type": "dart",
      "program": "lib/main_staging.dart",
      "args": ["--dart-define-from-file=.env.staging"],
      "flutterMode": "debug",
      "toolArgs": ["--device-timeout=60"]
    },
    {
      "name": "Profile — Staging",
      "request": "launch",
      "type": "dart",
      "program": "lib/main_staging.dart",
      "args": ["--dart-define-from-file=.env.staging"],
      "flutterMode": "profile",
      "toolArgs": ["--device-timeout=60"]
    },
    {
      "name": "Debug — Local",
      "request": "launch",
      "type": "dart",
      "program": "lib/main_local.dart",
      "args": ["--dart-define-from-file=.env.local"],
      "flutterMode": "debug",
      "toolArgs": ["--device-timeout=60"]
    },
    {
      "name": "Debug — Staging (Android)",
      "request": "launch",
      "type": "dart",
      "program": "lib/main_staging.dart",
      "args": ["--dart-define-from-file=.env.staging"],
      "flutterMode": "debug",
      "toolArgs": ["--device-timeout=60"],
      "deviceId": "android"
    },
    {
      "name": "Debug — Staging (iOS)",
      "request": "launch",
      "type": "dart",
      "program": "lib/main_staging.dart",
      "args": ["--dart-define-from-file=.env.staging"],
      "flutterMode": "debug",
      "toolArgs": ["--device-timeout=60"],
      "deviceId": "ios"
    },
    {
      "name": "Attach to Device",
      "request": "attach",
      "type": "dart"
    }
  ]
}
```

</details>

## 6. Everyday commands

```bash
flutter analyze                                             # static analysis (CI runs it)
flutter test                                                # unit tests under test/
dart run build_runner build --delete-conflicting-outputs    # regenerate Freezed / json_serializable / Riverpod files
flutter clean && flutter pub get                            # when Gradle or the generated files get into a weird state
```

Run `flutter analyze` and `flutter test` before committing; the pipeline fails on either.

## 7. Release build on your machine

Only needed to check something that behaves differently under R8 (minification, obfuscation, resource shrinking): a change to `android/app/proguard-rules.pro`, the Gradle toolchain, or a crash reported by Play Console with obfuscated class names.

<details>
<summary>Steps</summary>

The `release` signing config reads `android/key.properties` (gitignored) and fails with *"Keystore file … not found"* without it. The real keystore only lives in the CI pipeline; locally, sign with the debug key:

```properties
# android/key.properties
storeFile=C:/Users/<you>/.android/debug.keystore
storePassword=android
keyAlias=androiddebugkey
keyPassword=android
```

On macOS/Linux the keystore is at `~/.android/debug.keystore`. It is created the first time you run a debug build.

```bash
# Install and run on the connected device
flutter run --release --dart-define-from-file=.env.staging -t lib/main_staging.dart

# Or build the artifact Play Console receives
flutter build appbundle --release --dart-define-from-file=.env.staging -t lib/main_staging.dart
```

- The R8 mapping lands in `build/app/outputs/mapping/release/mapping.txt`; use it to resolve obfuscated class names reported by Play Console (e.g. `c.u.b`). `seeds.txt` next to it lists the classes kept by `-keep` rules.
- A build signed with the debug key cannot be uploaded to Play and cannot update an install that came from the store; uninstall that one first.
- Delete `android/key.properties` when you are done.

</details>

Store builds are produced by the Azure DevOps pipeline, see [`.docs/technical/PIPELINES.md`](../.docs/technical/PIPELINES.md).

## 8. Troubleshooting

| Symptom | Cause / fix |
|---------|-------------|
| `flutter pub get` → *Building with plugins requires symlink support* | Windows Developer Mode is off (§1). |
| Red "Initialization Failed" screen listing `AUTH_AUTHORITY`, `API_BASE_URL`… | Missing or mismatched `--dart-define-from-file`. Use the entry point + `.env` pair from §3. |
| Local run: connection error on sign-in or first request | Api not running, or `.env.local` points at HTTPS/`localhost` instead of `http://10.0.2.2:5000` (§4). |
| `flutter emulators` shows nothing | No AVD created yet (§2.2), or the Android Emulator package is not installed in SDK Manager. |
| Emulator ignores the host keyboard / feels frozen | Fix `hw.keyboard` and `hw.gpu.*` in the AVD `config.ini` (§2.5). |
| Release build → *Keystore file … not found* | Create `android/key.properties` with the debug key (§7). |
| Pipeline fails on the Flutter version | Local and CI must both be `3.47.4`; `flutter --version` to check. |

## 9. Project layout

```
Sigook.App/
├── lib/
│   ├── main_local.dart / main_staging.dart / main_production.dart   # entry points → main_common.dart
│   ├── core/        # config, network (Dio), routing (go_router), theme, services, shared widgets
│   └── features/    # one folder per feature: auth, registration, jobs, history, profile, ...
├── test/            # unit tests mirroring lib/
├── android/  ios/   # platform projects (no flavors; env comes from --dart-define)
├── fastlane/        # iOS build + upload used by the pipeline
└── .env.*           # per-environment compile-time config
```

Every feature is split into `domain/`, `data/` and `presentation/` (Clean Architecture, Riverpod viewmodels, Freezed models). The full map, the core widget catalogue and a step-by-step feature walkthrough are in [`.docs/technical/SIGOOK_APP_STRUCTURE.md`](../.docs/technical/SIGOOK_APP_STRUCTURE.md); the rules are in [`CLAUDE.md`](./CLAUDE.md).
