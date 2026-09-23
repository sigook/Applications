# Development Commands

## Covenant.Api solution (.NET 10)

One solution, `Covenant.Api/Covenant.Api.slnx`, holds the API (which also hosts the OpenIddict
authorization server), the shared libraries and the Azure Functions project. `global.json` pins
SDK `10.0.401`.

```bash
dotnet build Covenant.Api/Covenant.Api.slnx
dotnet run --project Covenant.Api/Covenant.Api
dotnet watch run --project Covenant.Api/Covenant.Api
dotnet test Covenant.Api/Covenant.Api.slnx                                            # All tests
dotnet test Covenant.Api/Covenant.Tests/Covenant.Tests.csproj                         # API unit tests
dotnet test Covenant.Api/Covenant.Integration.Tests/Covenant.Integration.Tests.csproj # Needs Docker (Testcontainers Postgres)
dotnet test Covenant.Api/Sigook.Functions.Tests/Sigook.Functions.Tests.csproj         # Functions unit tests

# Migrations (run from the repo root; the API is the startup project for both contexts)
dotnet ef migrations add <Name> --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api --context CovenantContext
dotnet ef migrations add <Name> --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api --context IdentityContext --output-dir Migrations/Identity
```

Key: shared cloud PostgreSQL (no local DB setup), Azure Service Bus for messaging. The API listens on
`https://localhost:44307`; the OpenID Connect discovery document is at
`https://localhost:44307/.well-known/openid-configuration` and the Scalar API reference at
`https://localhost:44307/scalar` (Development and Staging only).

## Sigook.Functions (.NET 10 Azure Functions, inside the Covenant.Api solution)

```bash
dotnet build Covenant.Api/Sigook.Functions/Sigook.Functions.csproj
dotnet test Covenant.Api/Sigook.Functions.Tests/Sigook.Functions.Tests.csproj
cd Covenant.Api/Sigook.Functions && func start   # Local run (requires Azure Functions Core Tools v4)
```

Functions (three, in two files): `NotificationSinExpiration` and `WarnLicensesExpiration` (Timer triggers, `Covenant.Api/Sigook.Functions/Functions/ScheduleTasks.cs`); `CraTableUploaded` (Blob trigger, `Covenant.Api/Sigook.Functions/Functions/CraTables.cs`). They call the API over HTTP with a client-credentials token issued by the API itself (`ScheduleTasks:AccountsUrl`).

## SigookApp (Flutter) - Requires Flutter `3.47.4`, JDK 17, Android SDK 36

```bash
cd SigookApp
flutter pub get
cp .env.example .env.staging              # Gitignored. Defaults already point at staging.
flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart
flutter run --dart-define-from-file=.env.local -t lib/main_local.dart        # Needs Api + IdentityServer running locally
dart run build_runner build --delete-conflicting-outputs  # Code gen (Freezed, Riverpod)
flutter test
flutter analyze
```

Each entry point reads its configuration from the matching `.env` file via `--dart-define-from-file`; without that flag the app starts with no API or auth URLs. See `.env.example` for the full variable list and the `10.0.2.2` host alias the Android emulator needs to reach local services.

**Flavors are iOS-only.** `ios/Runner.xcodeproj` defines `staging` and `production` schemes, so `--flavor` works there. `android/app/build.gradle.kts` declares no `productFlavors`, so passing `--flavor` to an Android build fails.

Android toolchain floor is enforced by Flutter itself and bumps with each release — 3.47.4 requires Gradle >= 8.14.0 (`android/gradle/wrapper/gradle-wrapper.properties`) and Kotlin >= 2.2.20 (`android/settings.gradle.kts`). Building with an older Flutter than the pinned one is untested.

First-time Windows setup also needs **Developer Mode** enabled (`start ms-settings:developers`), otherwise `flutter pub get` aborts with *"Building with plugins requires symlink support"*.

## Sigook.Web (Vue.js 3) - Requires Node `^20.19.0 || >=22.12.0` (engine-strict: older 20.x hard-fails)

Uses **pnpm** (pinned via `packageManager` field). Enable via `corepack enable` or `npm i -g pnpm`.

```bash
cd Sigook.Web
pnpm install && pnpm run dev     # Dev (Vite)
pnpm run build                   # Build (Vite)
pnpm run staging                 # Build staging (Vite)
pnpm run production              # Build production (Vite)
pnpm run type-check              # TypeScript check
pnpm run lint                    # ESLint
pnpm run format                  # Prettier
pnpm run preview                 # Preview production build
```

Note: output dir is `wwwroot/` (not `dist/`). Build tool is Vite; state is Pinia; UI is `buefy` 3.x.
Locally `VUE_APP_SECURITY_SERVER` points at the API (`https://localhost:44307`), which is also the
OpenID Connect authority.

## Covenant.Web (Vue.js 3) - Requires Node ^20.19.0 or >=22.12.0

Uses **pnpm** (pinned via `packageManager` field). Enable via `corepack enable` or `npm i -g pnpm`.

```bash
cd Covenant.Web
pnpm install && pnpm run dev     # Dev
pnpm run build:staging           # Build staging
pnpm run build:production        # Build production
pnpm run type-check              # TypeScript check
pnpm run lint                    # ESLint
```

Note: `build:staging`/`build:production` do NOT run vue-tsc (only plain `build` does) — type safety depends on the separate `type-check` step.
