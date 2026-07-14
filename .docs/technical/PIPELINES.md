# CI/CD Pipelines - Azure DevOps

Documentation for all Azure DevOps pipelines in `.azure-pipelines/`.

## Overview

All pipelines run on the **self-hosted agent pool** `covenant-build-pool` and use **path-based triggers** so each app deploys independently.

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
| Sigook.Web | `sigook-web-pipeline.yml` | `Sigook.Web/**` | Lint → Docker+Deploy → Notify | `sigook-web-staging` / `sigook` |
| Covenant.Web | `covenant-web-pipeline.yml` | `Covenant.Web/**` | Type-check+Lint+Build → Deploy → Notify | Static Web Apps: `covenantgroup-staging-swa` / `covenantgroup-swa` |
| IdentityServer | `covenant-identityserver-pipeline.yml` | `Covenant.IdentityServer/**` | Build+Test → Docker+Deploy | `sigook-accounts-staging` / `sigook-accounts` |
| SigookApp | `sigookapp-pipeline.yml` | `SigookApp/**` | Analyze+Validate → Build AAB → Google Play → Notify | Google Play (internal/production) |
| Sigook.Functions | `sigook-functions-pipeline.yml` | `Sigook.Functions/**` | Build → Publish+Deploy | `sigook-functions` (production only) |
| CognitiveServices | `cognitiveservices-pipeline.yml` | `Sigook.CognitiveServices/**` | Build → Publish+Deploy | `sigook-cognitive-services` (production only) |

**Note:** All pipelines exclude `**/*.md` from triggers (documentation changes don't trigger builds).

**Warning:** A legacy `Covenant.IdentityServer/azure-pipelines.yml` still exists alongside the documented `.azure-pipelines/covenant-identityserver-pipeline.yml`, with its own `main/master/dev` trigger. It is superseded — do not extend it; candidate for deletion.

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
- pnpm via corepack (pinned by `packageManager` field)
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

**Stage 1 - Build and Test:**
- Node.js 22 via shared template
- pnpm via corepack (pinned by `packageManager` field)
- Cache pnpm content-addressable store (by `pnpm-lock.yaml`)
- Type checking: `pnpm run type-check`
- Linting: `pnpm run lint`
- Build: `pnpm run build:staging` or `pnpm run build:production`
- Verify `dist/index.html` exists
- Publish `dist/` as artifact `covenantweb-dist` (only on direct push, not PRs)

**Stage 2 - Deploy** (only on direct push to dev/main):
- Deploy prebuilt `dist/` via `AzureStaticWebApp@0` (`skip_app_build: true`)
- Deployment token fetched at deploy time via `AzureCLI@2` + `SigookPipelines` service connection (`az staticwebapp secrets list`) — no manual pipeline variables needed
- SPA routing handled by `Covenant.Web/public/staticwebapp.config.json` (navigationFallback to `index.html`)
- Staging: `https://lively-island-020c8260f.7.azurestaticapps.net` (SWA `covenantgroup-staging-swa`, Free tier)
- Production: `https://www.covenantgroupl.com` (SWA `covenantgroup-swa`, Free tier, default host `ambitious-bush-0eb4f540f.7.azurestaticapps.net`)

**Stage 3 - Notify** (production only, uses `Sigook-Notifications` variable group):
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

### SigookApp (Flutter Android + Google Play)

**Build naming:** `SigookApp-YYYYMMDDr`

**Note:** iOS builds are handled by Xcode Cloud. This pipeline handles Android only.

**Stage 1 - Validate & Test** (all pushes and PRs):
- Flutter verify (`flutter --version`, `flutter doctor`)
- `flutter analyze --no-fatal-infos`
- Verify build config: `flutter build apk --debug --dry-run`

**Stage 2 - Build Android** (only on push to dev/main, not PRs):
- Version: `YYYY.M.D` (name), `Build.BuildId` (code)
- Variable groups: `SigookApp-Staging` or `SigookApp-Production` (env vars) + `SigookApp-Android` (signing)
- Download keystore from secure files (`sigook.jks`)
- Cache: Gradle + Flutter pub
- Android NDK 28.2.13676358 installation
- Build: `flutter build appbundle --flavor <env> -t <entry> --release` with `--dart-define` for all env vars
- AAB signing verification with `jarsigner`
- Publish artifact: `sigookapp-android-<env>`

**Stage 3 - Deploy to Google Play** (only on push to dev/main):
- Download AAB artifact
- Deploy via Fastlane: `fastlane android deploy`
- Uses `GOOGLE_PLAY_JSON_KEY` from variable group
- `continueOnError: true` (pipeline doesn't fail if Play Store deployment fails)

**Stage 4 - Notify** (production only, uses `Sigook-Notifications` variable group):
- Sends deployment email via Microsoft Graph API (template: `notify-deployment.yml`, appType: `mobile`)

**Required Variable Groups:**
- `SigookApp-Staging`: `AUTH_AUTHORITY`, `API_BASE_URL`, `CLIENT_ID`, `REDIRECT_URI`, `POST_LOGOUT_REDIRECT_URI`, `SCOPES`, `APP_NAME`, `GOOGLE_PLAY_JSON_KEY`
- `SigookApp-Production`: Same variables with production values
- `SigookApp-Android`: `KEYSTORE_FILE`, `KEY_PASSWORD`, `KEY_ALIAS`

**Required Secure Files:**
- `sigook.jks` - Android keystore for app signing

### Sigook.Functions (.NET 8 Azure Functions)

**Build naming:** `Sigook.Functions-YYYY.M.D.r`

**Trigger:** Manual only (production-only deployment).

**Stage 1 - Build:**
- .NET SDK 8.0.415
- Build solution (no tests - project has none)

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
Installs Flutter SDK, creates placeholder `.env` files, runs `pub get` and `build_runner`.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `flutterVersion` | string | `stable` | Flutter channel or version |
| `workingDirectory` | string | `$(System.DefaultWorkingDirectory)` | Flutter project directory |

**Note:** Currently SigookApp pipeline uses pre-installed Flutter on the VM instead of this template.

### fastlane-setup.yml
Installs Fastlane via Bundler with gem caching.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `workingDirectory` | string | required | Directory containing `Gemfile` |
| `continueOnError` | boolean | `false` | Continue on installation error |

**Note:** Currently SigookApp pipeline uses pre-installed Fastlane on the VM instead of this template.

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
| SigookApp | Google Play (internal track) | Google Play (production track) |

---

## Required Secrets & Service Connections

### Azure DevOps Service Connections
- **`SigookPipelines`** - Azure subscription for deployments
- **`acrServiceConnectionSigook`** - Azure Container Registry (`sigook.azurecr.io`)

### Pipeline Variables / Variable Groups
- **`SigookApp-Staging`** / **`SigookApp-Production`** - Flutter app env vars + Google Play key
- **`SigookApp-Android`** - Android keystore signing credentials
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
