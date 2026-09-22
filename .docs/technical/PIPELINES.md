# CI/CD Pipelines - Azure DevOps

Documentation for all Azure DevOps pipelines in `.azure-pipelines/`.

## Overview

All pipelines run on the **self-hosted agent pool** `covenant-build-pool` and use **path-based triggers** so each app deploys independently. The only exception is the iOS build/upload of SigookApp, which runs on **Microsoft-hosted macOS** agents (`vmImage`, free tier: 1 parallel job, 1800 min/month, 60 min per job).

### Environment Strategy

| Branch | Environment | Deploy |
|--------|-------------|--------|
| `dev` | Staging | Auto-deploy after build+test |
| `main` | Production | Manual trigger only (run from Azure DevOps) |

### PR Validation Strategy

- **PRs to `dev`**: Full validation (build, test, lint). Primary quality gate.
- **PRs to `main`**: Pipeline does NOT run (already validated on dev). Exception: Sigook.Functions and CognitiveServices validate PRs to `main` since they have no staging.
- **Direct push to `dev`**: Full flow (build, test, deploy).
- **Direct push to `main`**: No automatic trigger. Production deployments must be run manually from Azure DevOps.

### Shared Azure Resources

| Resource | Value |
|----------|-------|
| Container Registry | `sigook.azurecr.io` |
| ACR Service Connection | `acrServiceConnectionSigook` |
| Azure Subscription | `SigookPipelines` |

---

## Pipeline Summary

| Pipeline | File | Trigger Path | Stages | Deploy Target |
|----------|------|-------------|--------|---------------|
| Covenant.Api | `covenant-api-pipeline.yml` | `Covenant.Api/**` | Build+Test → Docker+Deploy → Notify | `sigook-api-staging` / `sigook-api` |
| Sigook.Web | `sigook-web-pipeline.yml` | `Sigook.Web/**` | Lint+Type-check → Docker+Deploy → Notify | `sigook-web-staging` / `sigook` |
| Covenant.Web | `covenant-web-pipeline.yml` | `Covenant.Web/**` | CI_CD (Build and Test job → Deploy job) → Notify | Static Web Apps: `covenantgroup-staging-swa` / `covenantgroup-swa` |
| IdentityServer | `covenant-identityserver-pipeline.yml` | `Covenant.IdentityServer/**` | Build+Test → Docker+Deploy | `sigook-accounts-staging` / `sigook-accounts` |
| SigookApp | `sigookapp-pipeline.yml` | `SigookApp/**` | Analyze+Validate → Version → Build AAB ‖ Build IPA → Google Play ‖ App Store Connect → Notify | Google Play (alpha/production) + TestFlight/App Store |
| SigookApp iOS Bootstrap | `sigookapp-ios-bootstrap-pipeline.yml` | Manual only (no trigger) | Migrate iOS project | Artifact `ios-bootstrap` (no deploy) |
| Sigook.Functions | `sigook-functions-pipeline.yml` | `Sigook.Functions/**` | Build → Publish+Deploy | `sigook-functions` (production only) |
| CognitiveServices | `cognitiveservices-pipeline.yml` | `Sigook.CognitiveServices/**` | Build → Publish+Deploy | `sigook-cognitive-services` (production only) |
| Database Refresh | `database-refresh-pipeline.yml` | Manual only (no trigger) | Refresh | Postgres `sigook` (`CovenantCoreStaging`, `CovenantSecurityStaging`) |

**Note:** Only the CI `trigger:` blocks exclude `**/*.md` (documentation pushes don't trigger builds). The `pr:` blocks of both web pipelines have no exclude, so docs-only PRs still run validation.

**Warning:** `Covenant.IdentityServer/azure-pipelines.yml` exists alongside the documented `.azure-pipelines/covenant-identityserver-pipeline.yml` and carries its own `main/master/dev` trigger. It is not the pipeline in use — do not extend it.

---

## Pipeline Details

### Covenant.Api (.NET 8)

**Build naming:** `CovenantApi-YYYY.M.D.r`

**Stage 1 - Build and Test:**
- .NET SDK 8.0.415 (template: `dotnet-setup.yml`)
- Build solution + NuGet auth (template: `dotnet-build-test.yml`)
- Unit tests: `Covenant.Tests`
- Integration tests: `Covenant.Integration.Tests`

**Stage 2 - Docker and Deploy** (only on push to dev/main, not PRs):
- Docker tag: `latest_staging` (dev) or `latest_production` (main)
- Image: `sigook.azurecr.io/api:<tag>`
- Dockerfile: `Covenant.Api/Dockerfile`
- Deploy: `AzureWebAppContainer@1` to Azure App Service
- Staging: `https://sigook-api-staging.azurewebsites.net`
- Production: `https://sigook-api.azurewebsites.net`

**Stage 3 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `api`)

### Sigook.Web (Vue.js 3 + Docker + Nginx)

**Build naming:** `SigookWeb-YYYYMMDDr`

**Stage 1 - Build and Validate:**
- Node.js 22 via shared template
- pnpm via corepack, version resolved from the `packageManager` field in package.json (the `npm i -g` fallback reads the same field)
- Cache pnpm content-addressable store (by `pnpm-lock.yaml`)
- Lint with ESLint, TypeScript type-check

**Stage 2 - Docker and Deploy** (only on push to dev/main):
- Token replacement in `index.html`, `public/**/*.html` and `public/**/*.json` (version injection using `#{...}#` tokens)
- Multi-stage Docker: Node.js 22 + pnpm build → Nginx alpine
- Build arg: `--build-arg ENV=staging|production`
- Image: `sigook.azurecr.io/web:<tag>`
- Staging: `https://sigook-web-staging.azurewebsites.net`
- Production: `https://sigook.azurewebsites.net`

**Stage 3 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `agency-portal`)

### Covenant.Web (Vue.js 3 + Azure Static Web Apps)

**Build naming:** `CovenantWeb-YYYYMMDDr`

Two stages: Stage 1 `CI_CD` (job 1 "Build and Test", job 2 "Deploy"), Stage 2 "Notify".

**Stage 1 (CI_CD) - Job 1: Build and Test:**
- Node.js 22 via shared template (the YAML also declares a stale, unused `nodeVersion: '20.x'` variable)
- pnpm via corepack, version resolved from the `packageManager` field in package.json (the `npm i -g` fallback reads the same field)
- Cache pnpm content-addressable store (by `pnpm-lock.yaml`)
- Type checking: `pnpm run type-check`
- Linting: `pnpm run lint`
- Build: `pnpm run build:staging` or `pnpm run build:production` — for PRs the environment resolves from the PR *target* branch, so a PR into `main` builds with production config
- Verify `dist/index.html` exists
- Publish `dist/` as artifact `covenantweb-dist` (only on direct push, not PRs)

**Stage 1 (CI_CD) - Job 2: Deploy** (only on direct push to dev/main):
- Deploy prebuilt `dist/` via `AzureStaticWebApp@0` (`skip_app_build: true`)
- Deployment token fetched at deploy time via `AzureCLI@2` + `SigookPipelines` service connection (`az staticwebapp secrets list`) — no manual pipeline variables needed
- SPA routing handled by `Covenant.Web/public/staticwebapp.config.json` (navigationFallback to `index.html`)
- Staging: `https://lively-island-020c8260f.7.azurestaticapps.net` (SWA `covenantgroup-staging-swa`, Free tier)
- Production: `https://www.covenantgroupl.com` (SWA `covenantgroup-swa`, Free tier, default host `ambitious-bush-0eb4f540f.7.azurestaticapps.net`)

**Stage 2 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `website`)

### Covenant.IdentityServer (.NET 6)

**Build naming:** `CovenantIdentityServer-YYYYMMDDr`

**Stage 1 - Build and Test:**
- .NET SDK 6.0.400
- Build + unit tests (`Covenant.IdentityServer.Tests`)
- No integration tests

**Stage 2 - Docker and Deploy** (only on direct push, not PRs):
- Docker build (no private feed: IdentityServer has no Covenant.Common dependency)
- Image: `sigook.azurecr.io/identityserver:<tag>`
- Staging: `https://sigook-accounts-staging.azurewebsites.net`
- Production: `https://sigook-accounts.azurewebsites.net`

### SigookApp (Flutter iOS + Android)

**Build naming:** `SigookApp-YYYYMMDDr`

**Environment mapping** — a single app identity per store (`com.all2job.all2job` / `com.sigook.sigook`); staging and production differ only in the entry point and the `--dart-define` values, and are separated in the stores by track/group:

| Branch | Environment | Android | iOS |
|--------|-------------|---------|-----|
| `dev` (auto) | staging (`lib/main_staging.dart`, group `SigookApp-Staging`) | Google Play closed testing track `alpha` | TestFlight internal group `Staging` |
| `main` (manual run) | production (`lib/main_production.dart`, group `SigookApp-Production`) | Google Play `production` (live after Google review) | App Store, submitted for review automatically (`automatic_release`) |

**Stage 1 - Validate & Test** (all pushes and PRs, Linux):
- Pinned Flutter via `templates/flutter-setup.yml` (fails if the version on PATH is not `flutterVersion`)
- `flutter analyze --no-fatal-infos`, `flutter test`
- Verify build config: `flutter build apk --debug --dry-run`

**Stage 2 - Version** (push to dev/main only, Linux, no checkout). One job computes and exposes:
- `appVersionName` = `YYYY.M.D`
- `appVersionCode` = `YYYYMMDDHH` (Android `versionCode`, cap 2100000000)
- `iosBuildNumber` = `YYYYMMDDHHMM` (`CFBundleVersion`; minute precision so a same-hour dev + main upload never collides in App Store Connect)

Both build stages read them as `stageDependencies.Version.Calculate.outputs['CalculateVersion.<name>']` at job level.

**Stage 3 - Build Android** (Linux, parallel with Build iOS):
- Variable groups: `SigookApp-Staging` or `SigookApp-Production` + `SigookApp-Android` (signing)
- Download keystore from secure files (`sigook.jks`)
- Cache: Gradle + Flutter pub
- Android platform read from `compileSdk`/`compileSdkMinor` in `build.gradle.kts`; NDK 28.2.13676358
- `SCOPES` must equal `openid,profile,api1,offline_access` exactly (extra scopes make IdentityServer answer `invalid_scope`)
- Build: `flutter build appbundle -t <entry> --release` with one `--dart-define` per env var
- AAB signing verification with `jarsigner`; publish artifact `sigookapp-android-<env>`

**Stage 4 - Build iOS** (hosted macOS `macosImage`, parallel with Build Android, `timeoutInMinutes: 60`):
- Variable groups: `SigookApp-Staging` or `SigookApp-Production` + `SigookApp-iOS`
- `xcode-select` to `xcodeVersion` (fails listing the installed versions when the image no longer ships it)
- Cache: Flutter pub + CocoaPods; `flutter-setup.yml` + `fastlane-setup.yml` (Bundler)
- `pod install` with a retry that only re-runs on transient network errors
- Same `SCOPES` check, then `bundle exec fastlane ios build entry_point: version: build_number: match_readonly:` (see Fastlane below)
- Publish artifact `sigookapp-ios-<env>`
- Pipeline parameter `matchReadonly` (default `true`): set to `false` only on the first run so match creates the certificate and profile

**Stage 5 - Deploy Android to Google Play** (Linux): downloads the AAB and runs `fastlane android deploy aab:<path> track:<alpha|production>` with `GOOGLE_PLAY_JSON_KEY`. `continueOnError: true`; the "Deploy Summary" step prints `deployStatus`.

**Stage 6 - Deploy iOS to App Store Connect** (hosted macOS — fastlane uploads through Apple's Transporter, which does not exist on Linux): downloads the IPA and runs `bundle exec fastlane ios <beta|release> ipa:<path> version:<appVersionName>`. `continueOnError: true`; the "Deploy Summary" step prints `deployStatus`.

**Stage 7 - Notify** (production only, `Sigook-Notifications` variable group): deployment email via Microsoft Graph (template `notify-deployment.yml`, appType `mobile`, version `appVersionName`).

**Fastlane** (`SigookApp/fastlane/`): `Appfile` (bundle id, team, package), `Matchfile` (git storage, `appstore` type, `readonly`), `Fastfile`:
- `android deploy track:` — `upload_to_play_store` with `release_status: completed`, no metadata/screenshots
- `ios build` — `setup_ci` (temporary keychain) → `match` (`git_basic_authorization` derived from `System.AccessToken`) → `update_code_signing_settings` on `Runner`/`Release` (manual signing, `Apple Distribution`, match profile; edits the checkout only) → `flutter build ios --release --no-codesign` with `--build-name/--build-number` and the nine `--dart-define` values from the environment → `build_app` (`app-store` export, `manageAppVersionAndBuildNumber: false`)
- `ios beta` — `upload_to_testflight` to the internal group `Staging` (`distribute_external: false`; an external group would trigger Beta App Review for every daily version)
- `ios release` — `upload_to_app_store` with `submit_for_review`, `automatic_release`, `reject_if_possible`, release notes from `IOS_RELEASE_NOTES` (default text), export compliance = no encryption
- iOS plugins are kept on CocoaPods (`config: enable-swift-package-manager: false` in `pubspec.yaml`) because `image_cropper` and `file_picker`'s `DKImagePickerController` require incompatible `TOCropViewController` majors under SPM

**Required Variable Groups:**
- `SigookApp-Staging`: `AUTH_AUTHORITY`, `API_BASE_URL`, `CLIENT_ID`, `REDIRECT_URI`, `POST_LOGOUT_REDIRECT_URI`, `SCOPES`, `APP_NAME`, `APP_INSIGHTS_CONNECTION_STRING`, `GOOGLE_PLAY_JSON_KEY`
- `SigookApp-Production`: Same variables with production values
- `SigookApp-Android`: `KEYSTORE_FILE`, `KEY_PASSWORD`, `KEY_ALIAS`
- `SigookApp-iOS`: `APP_STORE_CONNECT_KEY_ID`, `APP_STORE_CONNECT_ISSUER_ID`, `APP_STORE_CONNECT_API_KEY` (secret, base64 of the `.p8`, App Manager role), `MATCH_GIT_URL` (private Azure Repos git repo holding the encrypted certificate/profile), `MATCH_PASSWORD` (secret, match encryption passphrase), `IOS_RELEASE_NOTES` (optional)

**Required Secure Files:**
- `sigook.jks` - Android keystore for app signing

**Apple / Google prerequisites outside the repo:**
- App Store Connect API key (App Manager); the project build service needs `Contribute` on the `MATCH_GIT_URL` repo
- TestFlight internal group `Staging` with automatic distribution **off** (otherwise production uploads flow to staging testers)
- Google Play closed testing track `alpha`; the service account behind `GOOGLE_PLAY_JSON_KEY` needs "Release to production" and "Manage testing tracks"; Managed publishing off
- Apple allows 3 active Apple Distribution certificates per team; match creates one on the first non-readonly run

### SigookApp iOS Bootstrap (manual)

`sigookapp-ios-bootstrap-pipeline.yml` runs `flutter build ios --release --no-codesign` on a hosted macOS agent with the pinned Flutter and publishes the artifact `ios-bootstrap` (`migration.patch` = `git diff --binary`, `untracked-files.txt`, `Podfile.lock`, `Gemfile.lock`, `toolchain.txt`). Use it whenever a Flutter upgrade rewrites the Xcode project (nobody on the team has a Mac): apply the patch locally with `git apply --index`, copy the lockfiles and commit, so CI never mutates an unreviewed checkout. Parameters: `macosImage`, `xcodeVersion`, `flutterVersion`.

### Sigook.Functions (.NET 8 Azure Functions)

**Build naming:** `Sigook.Functions-YYYY.M.D.r`

**Trigger:** Manual only (production-only deployment).

**Stage 1 - Build:**
- .NET SDK 8.0.415
- Build solution + unit tests (`Sigook.Functions.Tests`, via `runUnitTests: true`)

**Stage 2 - Deploy to Production** (not on PRs):
- `dotnet publish` with zip
- Deploy: `AzureFunctionApp@2` to `sigook-functions`
- Production: `https://sigook-functions.azurewebsites.net`

### Sigook.CognitiveServices (.NET 8 Web App)

**Build naming:** `CognitiveServices-YYYY.M.D.r`

**Trigger:** Manual only (production-only deployment).

**Stage 1 - Build:**
- .NET SDK 8.0.415
- Build solution (no tests)

**Stage 2 - Deploy to Production** (not on PRs):
- `dotnet publish` of `Sigook.CognitiveServices.UI`
- Deploy: `AzureWebApp@1` (Linux) to `sigook-cognitive-services`
- Production: `https://sigook-cognitive-services.azurewebsites.net`

### Database Refresh (Staging)

**Build naming:** `DatabaseRefresh-YYYY.M.D.r`

**Trigger:** Manual only (run from Azure DevOps). Refreshes **both** databases in one run.

**Stage 1 - Refresh Staging Databases:**
- Fetches secrets from Key Vault `Sigook` via `AzureKeyVault@2` (`SigookPipelines` service connection): production connection strings for API (`CovenantCore`) and IdentityServer (`CovenantSecurity`), plus `pipelines--DbRefresh--StagingPasswordHash`
- Runs `Sigook.Database/Scripts/database-refresh.sh` once per database. The script parses the Npgsql connection string (Server, Port, User Id, Password, Database), re-registers the extracted password with `##vso[task.setsecret]` so it stays masked in logs, and derives the target name as `<Database>Staging`
- Inside a `postgres:latest` container on the agent: `pg_dump` (tar) → `DROP DATABASE ... WITH (FORCE)` + `CREATE DATABASE` → `pg_restore --no-owner`
- `CovenantSecurity` only: post-restore `UPDATE "User"` sets all `PasswordHash` to the shared staging hash (from Key Vault) and `EmailConfirmed = TRUE`
- Final step restarts `sigook-api-staging` and `sigook-accounts-staging` (resource group `SigookStaging`): the restore leaves staging with the production schema, and both apps apply pending EF migrations on startup (`SigookBackgroundService` / `SigookIdentityBackgroundService`)

**Requirements:**
- The `SigookPipelines` service principal has the `Key Vault Secrets User` role on the `Sigook` vault (RBAC model)
- The agent (`sigook-build-vm`, Azure VM) reaches the Postgres server through the existing "Allow Azure services" firewall rule
- No connection data or credentials live in the repo — everything is resolved from Key Vault at runtime

## Reusable Templates

Located in `.azure-pipelines/templates/`:

### dotnet-setup.yml
Installs .NET SDK via `UseDotNet@2` task.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `sdkVersion` | string | required | .NET SDK version (e.g., `8.0.415`) |
| `includePreviewVersions` | boolean | `false` | Include preview SDK versions |

### node-setup.yml
Installs Node.js via the official `NodeTool@0` task (no NVM dependency) and verifies the installation. Used by the Sigook.Web and Covenant.Web pipelines.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `nodeVersion` | string | `22.x` | Node.js version spec (e.g. `22.x`, `20.19.0`) |

### dotnet-build-test.yml
Builds solution, authenticates NuGet, and optionally runs tests.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `buildProjects` | string | required | Glob pattern for projects/solutions |
| `buildConfiguration` | string | `Release` | Build config |
| `runUnitTests` | boolean | `true` | Run unit tests |
| `unitTestProjects` | string | `''` | Unit test project pattern |
| `runIntegrationTests` | boolean | `false` | Run integration tests |
| `integrationTestProjects` | string | `''` | Integration test project pattern |

Automatically handles `NuGetAuthenticate@1` for Azure Artifacts private feeds.

### calculate-docker-tag.yml
Sets Docker tag and environment name based on branch.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `tagVariableName` | string | `tag` | Output variable name for the tag |
| `environmentVariableName` | string | `environment` | Output variable name for env |
| `stagingTag` | string | `latest_staging` | Tag for dev branch |
| `productionTag` | string | `latest_production` | Tag for main branch |
| `stepName` | string | `SetTag` | Step name for cross-job output reference |

Reads `isDev` pipeline variable to determine branch. Sets both job-scoped and output variables.

### calculate-azure-appname.yml
Determines Azure App Service name based on branch.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `appNameVariableName` | string | `azureAppName` | Output variable name |
| `stagingAppName` | string | required | App Service name for staging |
| `productionAppName` | string | required | App Service name for production |
| `stepName` | string | `SetAppName` | Step name for cross-job output reference |

### cleanup.yml
Frees disk space on self-hosted agents after builds.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `cleanDocker` | boolean | `false` | Remove dangling images, build cache, unused volumes |
| `cleanDotnet` | boolean | `false` | Remove `bin/`, `obj/`, NuGet temp cache |
| `cleanNodeModules` | boolean | `false` | Remove `node_modules/`, `dist/`, `wwwroot/` |
| `workingDirectory` | string | `$(System.DefaultWorkingDirectory)` | Root dir for cleanup |

Always cleans `$(Build.ArtifactStagingDirectory)` regardless of parameters.

### flutter-setup.yml
Installs an exact Flutter version with `FlutterInstall@0`, prepends `$(FlutterToolPath)` to `PATH` (the task alone does not touch `PATH`, so later `script:` steps would pick up the agent's own Flutter) and fails when `flutter --version` on `PATH` differs from the requested one.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `flutterVersion` | string | required | Exact Flutter version (e.g. `3.47.4`) |

Used by every Flutter job of the SigookApp pipelines.

### fastlane-setup.yml
Installs Fastlane via Bundler with gem caching.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `workingDirectory` | string | required | Directory containing `Gemfile` |
| `continueOnError` | boolean | `false` | Continue on installation error |

Used by the hosted macOS jobs of SigookApp (`bundle exec fastlane`). The Linux Android jobs use the Fastlane pre-installed on the VM (`/home/azureuser/gems`).

### notify-deployment.yml
Sends a deployment notification email via Microsoft Graph API using Azure AD OAuth authentication. No SMTP credentials needed — authenticates with an Azure AD App Registration.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `appType` | string | required | `api`, `agency-portal`, `website`, or `mobile` |
| `recipients` | string | required | Semicolon-separated email recipients |
| `version` | string | `$(Build.BuildId)` | Version identifier for the email |
| `senderEmail` | string | `it@covenantgroupl.com` | Sender email (must match Azure AD user) |

**App type email bodies:**

| appType | Label | Description |
|---------|-------|-------------|
| `api` | Covenant API | Backend API services — covenant.sigook.ca |
| `agency-portal` | Sigook Web Portal | Agency web portal — covenant.sigook.ca |
| `website` | Covenant Group Website | Corporate website — covenantgroup.com |
| `mobile` | Sigook Mobile App | Mobile app — Google Play Store & App Store links |

**Required variables** (from `Sigook-Notifications` variable group): `GraphTenantId`, `GraphClientId`, `GraphClientSecret`.

**Azure AD prerequisites:**
- App Registration with `Mail.Send` (Application) permission and admin consent granted
- The sender email (`it@covenantgroupl.com`) must be a valid Azure AD user

---

## Common Pipeline Variables

All pipelines use these branch-detection variables:

```yaml
variables:
  isMain: $[in(variables['Build.SourceBranch'], 'refs/heads/main','refs/heads/master')]
  isDev: $[in(variables['Build.SourceBranch'], 'refs/heads/dev','refs/heads/development')]
  isPR: $[eq(variables['Build.Reason'], 'PullRequest')]
```

---

## Deployment URLs Summary

| App | Staging | Production |
|-----|---------|------------|
| Covenant.Api | `sigook-api-staging.azurewebsites.net` | `sigook-api.azurewebsites.net` |
| Sigook.Web | `sigook-web-staging.azurewebsites.net` | `sigook.azurewebsites.net` |
| Covenant.Web | `lively-island-020c8260f.7.azurestaticapps.net` | `www.covenantgroupl.com` (SWA) |
| IdentityServer | `sigook-accounts-staging.azurewebsites.net` | `sigook-accounts.azurewebsites.net` |
| Sigook.Functions | N/A | `sigook-functions.azurewebsites.net` |
| CognitiveServices | N/A | `sigook-cognitive-services.azurewebsites.net` |
| SigookApp | Google Play closed testing (`alpha`) + TestFlight group `Staging` | Google Play `production` + App Store |

---

## Required Secrets & Service Connections

### Azure DevOps Service Connections
- **`SigookPipelines`** - Azure subscription for deployments
- **`acrServiceConnectionSigook`** - Azure Container Registry (`sigook.azurecr.io`)

### Pipeline Variables / Variable Groups
- **`SigookApp-Staging`** / **`SigookApp-Production`** - Flutter app env vars + Google Play key
- **`SigookApp-Android`** - Android keystore signing credentials
- **`SigookApp-iOS`** - App Store Connect API key + fastlane match settings (`MATCH_GIT_URL`, `MATCH_PASSWORD`)
- **`Sigook-Notifications`** - Microsoft Graph API credentials for deployment email notifications:
  - `GraphTenantId` - Azure AD tenant ID
  - `GraphClientId` - App Registration client ID
  - `GraphClientSecret` - App Registration client secret (secret)
  - `NotificationRecipients` - Semicolon-separated list of email recipients (use a Distribution List for company-wide notifications)

### Secure Files
- **`sigook.jks`** - Android keystore for SigookApp signing

---

## Troubleshooting

### Pipeline not triggering
- Verify the change is in the correct path (e.g., `Covenant.Api/**`)
- Check that the file is not excluded (e.g., `*.md` files are excluded)
- Sigook.Functions and CognitiveServices are manual-only (no automatic triggers)

### NuGet restore fails
- Ensure `NuGetAuthenticate@1` runs before `dotnet build`
- For IdentityServer Docker build, verify `PatSigookPackages` variable is set

### Docker build fails
- Check disk space on self-hosted agent (cleanup template should help)
- Verify `Dockerfile` path and build context are correct

### SigookApp build fails
- Verify Android SDK licenses are accepted
- Check NDK installation (version 28.2.13676358)
- Ensure keystore file (`sigook.jks`) is in secure files
- Verify variable groups have all required variables
- "expected Flutter X but PATH resolves Y": `FlutterInstall@0` succeeded but a different Flutter won on `PATH`; the template's prepend step must run before any `flutter` call
- "Xcode_X.app not found": Microsoft removed that Xcode from the hosted image; pick one from the listed versions and bump `xcodeVersion`
- match "no profile/certificate found" on a fresh signing repo: run the pipeline once with `matchReadonly = false`; a 403 from Apple while creating them means the API key role is too low (use Admin)
- App Store Connect rejects the upload with a duplicate build number: the same `iosBuildNumber` was already uploaded (a re-run of Build iOS reuses the Version stage outputs); run the pipeline again instead of re-running the stage
- Google Play rejects the AAB: `versionCode` must exceed every code previously uploaded on any track; a dev and a main run in the same hour collide on `YYYYMMDDHH`
