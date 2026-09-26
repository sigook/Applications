# CI/CD Pipelines - Azure DevOps

Documentation for all Azure DevOps pipelines in `.azure-pipelines/`.

## Overview

All pipelines run on the **self-hosted agent pool** `covenant-build-pool` and use **path-based triggers** so each app deploys independently. The only exception is the iOS build/upload of SigookApp, which runs on **Microsoft-hosted macOS** agents (`vmImage`, free tier: 1 parallel job, 1800 min/month, 60 min per job).

### Environment Strategy

| Branch | Environment | Deploy |
|--------|-------------|--------|
| `dev` | Staging | Auto-deploy after build+test |
| `main` | Production | Manual trigger only (run from Azure DevOps) |

Covenant.Api, Sigook.Web and Covenant.Web resolve their targets (environment, Docker tag, App Service / Static Web App, URL) as compile-time variables: `main`/`master` → production values, any other ref (PRs and feature branches included) → staging values. Their deploy runs as a `deployment` job bound to the Azure DevOps environment `staging` or `production` (Pipelines → Environments), which records the deploy history per environment. Both environments are shared by the three apps and have no approvals or checks; adding one in the UI gates every production deploy of those apps without YAML changes. The three pipelines are authorized on both environments.

All checkouts are shallow (`fetchDepth: 1`).

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
| Covenant.Api | `covenant-api-pipeline.yml` | `Covenant.Api/**` | Build+Test+Publish+Docker → Deploy → Notify | `sigook-api-staging` / `sigook-api` |
| Sigook.Web | `sigook-web-pipeline.yml` | `Sigook.Web/**` | Lint+Type-check+Build+Docker → Deploy → Notify | `sigook-web-staging` / `sigook` |
| Covenant.Web | `covenant-web-pipeline.yml` | `Covenant.Web/**` | CI_CD (Build and Test job → Deploy job) → Notify | Static Web Apps: `covenantgroup-staging-swa` / `covenantgroup-swa` |
| SigookApp | `sigookapp-pipeline.yml` | `SigookApp/**` | Analyze+Validate → Version → Build AAB ‖ Build IPA+App Store Connect → Google Play → Notify | Google Play (closed testing/production) + TestFlight/App Store |
| SigookApp iOS Bootstrap | `sigookapp-ios-bootstrap-pipeline.yml` | Manual only (no trigger) | Migrate iOS project | Artifact `ios-bootstrap` (no deploy) |
| Sigook.Functions | `sigook-functions-pipeline.yml` | `Covenant.Api/Sigook.Functions/**` (PRs to main only) | Build+Test+Publish+Deploy (single job) | `sigook-functions` (production only) |
| CognitiveServices | `cognitiveservices-pipeline.yml` | `Sigook.CognitiveServices/**` | Build+Publish+Deploy (single job) | `sigook-cognitive-services` (production only) |
| Database Refresh | `database-refresh-pipeline.yml` | Manual only (no trigger) | Refresh | Postgres `sigook` (`CovenantCoreStaging`, `CovenantSecurityStaging`) |

**Note:** Only the CI `trigger:` blocks exclude `**/*.md` (documentation pushes don't trigger builds). The `pr:` blocks of both web pipelines have no exclude, so docs-only PRs still run validation.

The identity server is part of `Covenant.Api` (OpenIddict), so it ships with the API image and has no pipeline of its own. The `covenant-api-pipeline.yml` trigger excludes `Covenant.Api/Sigook.Functions/**` and `Covenant.Api/Sigook.Functions.Tests/**`; those folders belong to the Functions pipeline.

---

## Pipeline Details

### Covenant.Api (.NET 10)

**Build naming:** `CovenantApi-YYYY.M.D.r`

**Stage 1 - Build and Test** (one job):
- .NET SDK 10.0.401 (template: `dotnet-setup.yml`)
- Build `Covenant.Api/Covenant.Api.slnx` (template: `dotnet-build-test.yml`; packages come from nuget.org only, see `Covenant.Api/nuget.config`)
- Unit tests: `Covenant.Tests`, `Sigook.Functions.Tests`
- Integration tests: `Covenant.Integration.Tests`
- dev/main only (the steps are left out of PR runs at compile time): `dotnet publish --no-build` of `Covenant.Api.csproj` into `$(Build.ArtifactStagingDirectory)/api`, then Docker build + push with that folder as build context
- Docker tags: `latest_staging` (dev) or `latest_production` (main), plus the immutable `$(Build.BuildId)`
- Dockerfile: `Covenant.Api/Dockerfile` — runtime-only (`aspnet:10.0` + the published output); the solution is compiled once, on the agent
- Cleanup of `bin/`/`obj/` runs on every run, PRs included

**Stage 2 - Deploy** (only on push to dev/main, not PRs; `deployment` job on environment `staging`/`production`):
- Deploy `sigook.azurecr.io/api:$(Build.BuildId)` with `AzureWebAppContainer@1` to Azure App Service
- Staging: `https://sigook-api-staging.azurewebsites.net`
- Production: `https://sigook-api.azurewebsites.net`

**Stage 3 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `api`)

### Sigook.Web (Vue.js 3 + Docker + Nginx)

**Build naming:** `SigookWeb-YYYYMMDDr`

**Stage 1 - Build and Validate** (one job):
- Node.js 22 via shared template
- pnpm via corepack, version resolved from the `packageManager` field in package.json (the `npm i -g` fallback reads the same field)
- Cache pnpm content-addressable store (by `pnpm-lock.yaml`)
- Lint with ESLint, TypeScript type-check (the only `vue-tsc` run)
- dev/main only: token replacement in `index.html`, `public/**/*.html` and `public/**/*.json` (version injection using `#{...}#` tokens)
- `pnpm exec vite build --mode staging|production` into `wwwroot/` — runs on PRs too (always `staging`) so bundling errors fail the PR
- dev/main only: Docker build + push. `Sigook.Web/Dockerfile` is Nginx alpine copying the prebuilt `wwwroot/`; `.dockerignore` allowlists only `wwwroot` and `nginx.conf`
- Image: `sigook.azurecr.io/web:<tag>` plus the immutable `$(Build.BuildId)`

**Stage 2 - Deploy** (only on push to dev/main; `deployment` job on environment `staging`/`production`):
- Deploy `sigook.azurecr.io/web:$(Build.BuildId)` with `AzureWebAppContainer@1`
- Staging: `https://sigook-web-staging.azurewebsites.net`
- Production: `https://sigook.azurewebsites.net`

**Stage 3 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `agency-portal`)

### Covenant.Web (Vue.js 3 + Azure Static Web Apps)

**Build naming:** `CovenantWeb-YYYYMMDDr`

Two stages: Stage 1 `CI_CD` (job 1 "Build and Test", job 2 "Deploy"), Stage 2 "Notify".

**Stage 1 (CI_CD) - Job 1: Build and Test:**
- Node.js 22 via shared template
- pnpm via corepack, version resolved from the `packageManager` field in package.json (the `npm i -g` fallback reads the same field)
- Cache pnpm content-addressable store (by `pnpm-lock.yaml`, with an OS-only restore key)
- Type checking: `pnpm run type-check`
- Linting: `pnpm run lint`
- Build: `pnpm run build:staging` (dev and PRs) or `pnpm run build:production` (main)
- Verify `dist/index.html` exists
- Publish `dist/` as artifact `covenantweb-dist` (only on direct push, not PRs)

**Stage 1 (CI_CD) - Job 2: Deploy** (only on direct push to dev/main; `deployment` job on environment `staging`/`production`):
- Deploy prebuilt `dist/` via `AzureStaticWebApp@0` (`skip_app_build: true`)
- Target SWA name and resource group are compile-time variables; the deployment token is fetched at deploy time via `AzureCLI@2` + `SigookPipelines` service connection (`az staticwebapp secrets list`) — no manual pipeline variables needed
- SPA routing handled by `Covenant.Web/public/staticwebapp.config.json` (navigationFallback to `index.html`)
- Staging: `https://lively-island-020c8260f.7.azurestaticapps.net` (SWA `covenantgroup-staging-swa`, Free tier)
- Production: `https://www.covenantgroupl.com` (SWA `covenantgroup-swa`, Free tier, default host `ambitious-bush-0eb4f540f.7.azurestaticapps.net`)

**Stage 2 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `website`)

### SigookApp (Flutter iOS + Android)

**Build naming:** `SigookApp-YYYYMMDDr`

**Environment mapping** — a single app identity per store (`com.all2job.all2job` / `com.sigook.sigook`); staging and production differ only in the entry point and the `--dart-define` values, and are separated in the stores by track/group:

| Branch | Environment | Android | iOS |
|--------|-------------|---------|-----|
| `dev` (auto) | staging (`lib/main_staging.dart`, group `SigookApp-Staging`) | Google Play closed testing track `Closed Testing - SIGOOK V2` (custom track, addressed by its display name) | TestFlight internal group `Staging` |
| `main` (manual run) | production (`lib/main_production.dart`, group `SigookApp-Production`) | Google Play `production` (live after Google review) | App Store, submitted for review automatically (`automatic_release`) |

The CI trigger uses `batch: true`: pushes that land while a run is in progress are grouped into one next run, so a burst of commits spends a single iOS build of hosted macOS minutes. All checkouts are shallow (`fetchDepth: 1`).

**Stage 1 - Validate & Test** (all pushes and PRs, Linux):
- Pinned Flutter via `templates/flutter-setup.yml` (fails if the version on PATH is not `flutterVersion`)
- `flutter analyze --no-fatal-infos`, `flutter test`
- Verify build config: `flutter build apk --debug --dry-run`

**Stage 2 - Version** (push to dev/main only, Linux, no checkout). One job computes and exposes:
- `appVersionName` = `YYYY.M.D`
- `appVersionCode` = `YYYYMMDDFF` where `FF` is the fraction of the UTC day (`minuteOfDay * 100 / 1440`, 0-99, 14.4-minute slots). The Android `versionCode` cap is 2100000000, so `YYYYMMDD` leaves exactly two digits; slots beat hours because a re-run in the same slot is the only collision left
- `iosBuildNumber` = `YYYYMMDDHHMM` (`CFBundleVersion`; minute precision so a same-hour dev + main upload never collides in App Store Connect)

Both build stages read them as `stageDependencies.Version.Calculate.outputs['CalculateVersion.<name>']` at job level.

**Stage 3 - Build Android** (Linux, parallel with Build iOS):
- Variable groups: `SigookApp-Staging` or `SigookApp-Production` + `SigookApp-Android` (signing)
- Download keystore from secure files (`sigook.jks`)
- No pipeline caching: the self-hosted VM keeps `~/.gradle` and `~/.pub-cache` on disk between runs, so `Cache@2` only added upload/download time
- Android platform read from `compileSdk`/`compileSdkMinor` in `build.gradle.kts`; NDK 28.2.13676358
- `SCOPES` must equal `openid,profile,api1,offline_access` exactly (extra scopes make the authorization server answer `invalid_scope`)
- Build: `flutter build appbundle -t <entry> --release` with one `--dart-define` per env var
- AAB signing verification with `jarsigner`; publish artifact `sigookapp-android-<env>`

**Stage 4 - Build and Deploy iOS** (hosted macOS `macosImage`, parallel with Build Android, `timeoutInMinutes: 60`; a single job, so the upload does not boot a second hosted agent):
- Variable groups: `SigookApp-Staging` or `SigookApp-Production` + `SigookApp-iOS`
- `xcode-select` to `xcodeVersion` (fails listing the installed versions when the image no longer ships it)
- Cache: Flutter SDK (`flutter-setup.yml` with `cacheSdk: true`), Flutter pub (by `pubspec.lock`), CocoaPods (by `Podfile.lock`); `fastlane-setup.yml` (Bundler)
- `pod install` with a retry that only re-runs on transient network errors
- Same `SCOPES` check, then `bundle exec fastlane ios build entry_point: version: build_number: match_readonly:` (see Fastlane below)
- Publish artifact `sigookapp-ios-<env>`
- Upload step `DeployAppStoreConnect` (fastlane uploads through Apple's Transporter, which does not exist on Linux): `bundle exec fastlane ios <beta|release> ipa:<path> version:<appVersionName>`. It never fails the run (`continueOnError`) and exposes `deployStatus` (`success`/`failed`) as an output variable
- Pipeline parameter `matchReadonly` (default `true`): set to `false` only on the first run so match creates the certificate and profile

**Stage 5 - Deploy Android to Google Play** (Linux): downloads the AAB and runs `fastlane android deploy aab:<path> track:"<Closed Testing - SIGOOK V2|production>"` with `GOOGLE_PLAY_JSON_KEY`. The step `DeployGooglePlay` never fails the run (`continueOnError`) and exposes `deployStatus` (`success`/`failed`) as an output variable.

**Stage 6 - Notify** (production only, `Sigook-Notifications` variable group): deployment email via Microsoft Graph (template `notify-deployment.yml`, appType `mobile`, version `appVersionName`). Runs only when both `deployStatus` outputs are `success`: since the upload steps never fail the run, `succeeded()` alone would also email after a rejected upload.

**Fastlane** (`SigookApp/fastlane/`): `Appfile` (bundle id, team, package), `Matchfile` (git storage, `appstore` type, `readonly`), `Fastfile`:
- `android deploy track:` — `upload_to_play_store` with `release_status: completed`, no metadata/screenshots
- `ios build` — `setup_ci` (temporary keychain) → `match` (`git_basic_authorization` derived from `System.AccessToken`) → `update_code_signing_settings` on `Runner`/`Release` (manual signing, `Apple Distribution`, match profile; edits the checkout only) → `flutter build ios --release --no-codesign` with `--build-name/--build-number` and the nine `--dart-define` values from the environment → `build_app` (`app-store` export, `manageAppVersionAndBuildNumber: false`)
- `ios beta` — `upload_to_testflight` to the internal group `Staging` (`distribute_external: false`; an external group would trigger Beta App Review for every daily version)
- `ios release` — `upload_to_app_store` with `submit_for_review`, `automatic_release`, `reject_if_possible`, export compliance = no encryption. Release notes come from `IOS_RELEASE_NOTES` (default text) and are written to every locale the App Store listing already has (`store_locales` private lane, resolved through the App Store Connect API): submission fails with `You must provide a value for the attribute 'whatsNew'` when any locale is left empty
- iOS plugins are kept on CocoaPods (`config: enable-swift-package-manager: false` in `pubspec.yaml`) because `image_cropper` and `file_picker`'s `DKImagePickerController` require incompatible `TOCropViewController` majors under SPM

**Required Variable Groups:**
- `SigookApp-Staging`: `AUTH_AUTHORITY`, `API_BASE_URL`, `CLIENT_ID`, `REDIRECT_URI`, `POST_LOGOUT_REDIRECT_URI`, `SCOPES`, `APP_NAME`, `APP_INSIGHTS_CONNECTION_STRING`, `GOOGLE_PLAY_JSON_KEY`
- `SigookApp-Production`: Same variables with production values
- `SigookApp-Android`: `KEYSTORE_FILE`, `KEY_PASSWORD`, `KEY_ALIAS`
- `SigookApp-iOS`: `APP_STORE_CONNECT_KEY_ID`, `APP_STORE_CONNECT_ISSUER_ID`, `APP_STORE_CONNECT_API_KEY` (secret, base64 of the `.p8`, App Manager role), `MATCH_GIT_URL` (private Azure Repos git repo holding the encrypted certificate/profile), `MATCH_PASSWORD` (secret, match encryption passphrase), `IOS_RELEASE_NOTES` (optional)

**Required Secure Files:**
- `sigook.jks` - Android keystore for app signing

**Apple / Google prerequisites outside the repo:**
- App Store Connect API key (App Manager); the build service the jobs run as (the organization-level one unless the job authorization scope is limited to the project) needs `Contribute` on the `MATCH_GIT_URL` repo
- TestFlight internal group `Staging` with automatic distribution **off** (otherwise production uploads flow to staging testers)
- Google Play closed testing track `Closed Testing - SIGOOK V2` with the tester list (a release on any other track is invisible to them); the service account behind `GOOGLE_PLAY_JSON_KEY` needs "Release to production" and "Manage testing tracks"; Managed publishing off
- Play serves every user the highest `versionCode` across all tracks they are enrolled in and never downgrades, so an enrolled tester runs the staging build whenever `dev` was released after `main` (and sees the "(Beta)" suffix in the store). Only people who accept that belong in the closed track; anyone who needs the production app must be removed from it and reinstall. Staging and local builds show an orange environment banner, so a phone running the wrong backend is obvious
- Apple allows 3 active Apple Distribution certificates per team; match creates one on the first non-readonly run

### SigookApp iOS Bootstrap (manual)

`sigookapp-ios-bootstrap-pipeline.yml` runs `flutter build ios --release --no-codesign` on a hosted macOS agent with the pinned Flutter and publishes the artifact `ios-bootstrap` (`migration.patch` = `git diff --binary`, `untracked-files.txt`, `Podfile.lock`, `Gemfile.lock`, `toolchain.txt`). Use it whenever `flutterVersion` changes (nobody on the team has a Mac), so CI never mutates an unreviewed checkout and the lockfiles stay reproducible. Parameters: `macosImage`, `xcodeVersion`, `flutterVersion` (keep them equal to `sigookapp-pipeline.yml`).

Applying the artifact from the repo root (Git Bash):

```bash
git apply --exclude='SigookApp/macos/*' ~/Downloads/ios-bootstrap/migration.patch
cp ~/Downloads/ios-bootstrap/Podfile.lock SigookApp/ios/Podfile.lock
cp ~/Downloads/ios-bootstrap/Gemfile.lock SigookApp/Gemfile.lock
git diff --stat
```

Review the diff under `SigookApp/ios/` (the app has no macOS target, hence the exclude), then commit and push.

### Sigook.Functions (.NET 10 Azure Functions, isolated worker)

**Build naming:** `Sigook.Functions-YYYY.M.D.r`

**Trigger:** Manual only (production-only deployment). PRs to `main` touching `Covenant.Api/Sigook.Functions/**`, `Covenant.Api/Sigook.Functions.Tests/**` or `Covenant.Api/Covenant.Common/**` run the build stage.

**Single stage and job - Build and Deploy to Production:**
- .NET SDK 10.0.401
- Build `Covenant.Api/Sigook.Functions.Tests/Sigook.Functions.Tests.csproj` (pulls the Functions project and `Covenant.Common`) + unit tests
- Not on PRs (left out at compile time): `dotnet publish --no-build` of `Covenant.Api/Sigook.Functions/Sigook.Functions.csproj` with zip
- Deploy: `AzureFunctionApp@2` to `sigook-functions` (runtime stack `DOTNET-ISOLATED|10.0`, configured on the Function App, not in the repo)
- Production: `https://sigook-functions.azurewebsites.net`

### Sigook.CognitiveServices (.NET 8 Web App)

**Build naming:** `CognitiveServices-YYYY.M.D.r`

**Trigger:** Manual only (production-only deployment).

**Single stage and job - Build and Deploy to Production:**
- .NET SDK 8.0.415
- Build solution (no tests)
- Not on PRs (left out at compile time): `dotnet publish --no-build` of `Sigook.CognitiveServices.UI`
- Deploy: `AzureWebApp@1` (Linux) to `sigook-cognitive-services`
- Production: `https://sigook-cognitive-services.azurewebsites.net`

### Database Refresh (Staging)

**Build naming:** `DatabaseRefresh-YYYY.M.D.r`

**Trigger:** Manual only (run from Azure DevOps). Refreshes **both** databases in one run.

**Stage 1 - Refresh Staging Databases:**
- Fetches secrets from Key Vault `Sigook` via `AzureKeyVault@2` (`SigookPipelines` service connection): `production-api--ConnectionStrings--DefaultConnection` (`CovenantCore`), `production-api--ConnectionStrings--IdentityConnection` (`CovenantSecurity`) and `pipelines--DbRefresh--StagingPasswordHash`
- Runs `Sigook.Database/Scripts/database-refresh.sh` once per database. The script parses the Npgsql connection string (Server, Port, User Id, Password, Database), re-registers the extracted password with `##vso[task.setsecret]` so it stays masked in logs, and derives the target name as `<Database>Staging`
- Inside a `postgres:latest` container on the agent: `pg_dump` (tar) → `DROP DATABASE ... WITH (FORCE)` + `CREATE DATABASE` → `pg_restore --no-owner`
- `CovenantSecurity` only: post-restore `UPDATE "User"` sets all `PasswordHash` to the shared staging hash (from Key Vault) and `EmailConfirmed = TRUE`
- Final step restarts `sigook-api-staging` (resource group `SigookStaging`): the restore leaves staging with the production schema, and the Api applies pending EF migrations for both `CovenantContext` and `IdentityContext` on startup (`SigookBackgroundService`)

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
| `sdkVersion` | string | required | .NET SDK version (e.g., `10.0.401`) |
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

Reads `isDev` pipeline variable to determine branch. Sets both job-scoped and output variables. Not referenced by any pipeline (they all use compile-time variables).

### calculate-azure-appname.yml
Determines Azure App Service name based on branch. Not referenced by any pipeline.

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
Installs an exact Flutter version with `FlutterInstall@0`, prepends `$(FlutterToolPath)` to `PATH` (the task alone does not touch `PATH`, so later `script:` steps would pick up the agent's own Flutter) and fails when `flutter --version` on `PATH` differs from the requested one. No `flutter doctor` (it cost ~2.5 min per hosted run and gated nothing).

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `flutterVersion` | string | required | Exact Flutter version (e.g. `3.47.4`) |
| `cacheSdk` | boolean | `false` | Restore `$(Agent.ToolsDirectory)/Flutter` through `Cache@2` (key: OS + arch + version) so `FlutterInstall@0` finds the SDK in its tool cache. Hosted agents only; self-hosted agents keep the tool cache on disk |

Used by every Flutter job of the SigookApp pipelines.

### fastlane-setup.yml
Installs Fastlane via Bundler with gem caching (key: `Gemfile.lock`).

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
| Identity (OpenIddict, same App Service as the API) | `staging.accounts.sigook.ca` | `accounts.sigook.ca` |
| Sigook.Web | `sigook-web-staging.azurewebsites.net` | `sigook.azurewebsites.net` |
| Covenant.Web | `lively-island-020c8260f.7.azurestaticapps.net` | `www.covenantgroupl.com` (SWA) |
| Sigook.Functions | N/A | `sigook-functions.azurewebsites.net` |
| CognitiveServices | N/A | `sigook-cognitive-services.azurewebsites.net` |
| SigookApp | Google Play closed testing (`Closed Testing - SIGOOK V2`) + TestFlight group `Staging` | Google Play `production` + App Store |

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
- All packages come from nuget.org (`Covenant.Api/nuget.config`); there is no private feed, so no authentication step is needed
- Versions are pinned in `Covenant.Api/Directory.Packages.props` (central package management)

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
- Google Play rejects the AAB with `Version code N has already been used`: `versionCode` must exceed every code previously uploaded on any track, so a re-run (or a dev + main run) inside the same 14.4-minute slot collides. Wait for the next slot and re-run; the earlier upload already reached its track
- Testers do not see a staging build: Play only offers a release to the testers of the track it was uploaded to, and each track has its own opt-in link; check the release landed on `Closed Testing - SIGOOK V2` and is "Available to testers" (the first release on a new closed track waits for Google review)
