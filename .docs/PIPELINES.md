# CI/CD Pipelines - Azure DevOps

Documentation for all Azure DevOps pipelines in `.azure-pipelines/`.

## Overview

All pipelines run on the **self-hosted agent pool** `covenant-build-pool` and use **path-based triggers** so each app deploys independently.

### Environment Strategy

| Branch | Environment | Deploy |
|--------|-------------|--------|
| `dev` | Staging | Auto-deploy after build+test |
| `main` | Production | Auto-deploy after build+test |

### PR Validation Strategy

- **PRs to `dev`**: Full validation (build, test, lint). Primary quality gate.
- **PRs to `main`**: Pipeline does NOT run (already validated on dev). Exception: Sigook.Functions and CognitiveServices validate PRs to `main` since they have no staging.
- **Direct push to `dev`/`main`**: Full flow (build, test, deploy).

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
| Covenant.Api | `covenant-api-pipeline.yml` | `Covenant.Api/**` | Build+Test → Docker+Deploy | `sigook-api-staging` / `sigook-api` |
| Sigook.Web | `sigook-web-pipeline.yml` | `Sigook.Web/**` | Lint → Docker+Deploy | `sigook-web-staging` / `sigook` |
| Covenant.Web | `covenant-web-pipeline.yml` | `Covenant.Web/**` | Type-check+Lint+Build → Deploy | `covenantgroup-staging` / `covenantgroup` |
| IdentityServer | `covenant-identityserver-pipeline.yml` | `Covenant.IdentityServer/**` | Build+Test → Docker+Deploy | `sigook-accounts-staging` / `sigook-accounts` |
| SigookApp | `sigookapp-pipeline.yml` | `SigookApp/**` | Analyze+Validate → Build AAB → Google Play | Google Play (internal/production) |
| Sigook.Functions | `sigook-functions-pipeline.yml` | `Sigook.Functions/**` | Build → Publish+Deploy | `sigook-functions` (production only) |
| CognitiveServices | `cognitiveservices-pipeline.yml` | `Sigook.CognitiveServices/**` | Build → Publish+Deploy | `sigook-cognitive-services` (production only) |
| Covenant.Common | `covenant-common-nuget-pipeline.yml` | Manual only | Build+Test → Pack+Publish | Azure Artifacts feed `sigook/Covenant.Common` |

**Note:** All pipelines exclude `**/*.md` from triggers (documentation changes don't trigger builds).

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

### Sigook.Web (Vue.js 2 + Docker + Nginx)

**Build naming:** `SigookWeb-YYYYMMDDr`

**Stage 1 - Build and Validate:**
- Node.js 16 via NVM (alias `sigook-web`)
- Cache `node_modules` (by `package-lock.json`)
- Lint with ESLint

**Stage 2 - Docker and Deploy** (only on push to dev/main):
- Token replacement in `public/**/*.html` and `public/**/*.json` (version injection using `#{...}#` tokens)
- Multi-stage Docker: Node.js 16 build → Nginx alpine
- Build arg: `--build-arg ENV=staging|production`
- Image: `sigook.azurecr.io/web:<tag>`
- Staging: `https://sigook-web-staging.azurewebsites.net`
- Production: `https://sigook.azurewebsites.net`

### Covenant.Web (Vue.js 3 + Azure App Service)

**Build naming:** `CovenantWeb-YYYYMMDDr`

**Stage 1 - Build and Test:**
- Node.js 20 via NVM (alias `covenant-web`)
- Cache `node_modules` (by `package-lock.json`)
- Type checking: `npm run type-check`
- Linting: `npm run lint`
- Build: `npm run build:staging` or `npm run build:production`
- Verify `dist/index.html` exists
- Archive as zip artifact (only on direct push, not PRs)

**Stage 2 - Deploy** (only on direct push to dev/main):
- Deploy zip to Azure App Service (Linux)
- Runtime: Node.js 20 LTS, startup: `npm start`
- Staging: `https://covenantgroup-staging.azurewebsites.net`
- Production: `https://covenantgroup.azurewebsites.net`

### Covenant.IdentityServer (.NET 6)

**Build naming:** `CovenantIdentityServer-YYYYMMDDr`

**Stage 1 - Build and Test:**
- .NET SDK 6.0.400
- Build + unit tests (`Covenant.IdentityServer.Tests`)
- No integration tests

**Stage 2 - Docker and Deploy** (only on direct push, not PRs):
- Docker build with `--build-arg PAT=$(PatSigookPackages)` (required for Covenant.Common NuGet package from Azure Artifacts)
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

**Required Variable Groups:**
- `SigookApp-Staging`: `AUTH_AUTHORITY`, `API_BASE_URL`, `CLIENT_ID`, `REDIRECT_URI`, `POST_LOGOUT_REDIRECT_URI`, `SCOPES`, `APP_NAME`, `GOOGLE_PLAY_JSON_KEY`
- `SigookApp-Production`: Same variables with production values
- `SigookApp-Android`: `KEYSTORE_FILE`, `KEY_PASSWORD`, `KEY_ALIAS`

**Required Secure Files:**
- `sigook.jks` - Android keystore for app signing

### Sigook.Functions (.NET 8 Azure Functions)

**Build naming:** `Sigook.Functions-YYYY.M.D.r`

**Trigger:** Only `main` branch (production-only deployment).

**Stage 1 - Build:**
- .NET SDK 8.0.415
- Build solution (no tests - project has none)

**Stage 2 - Deploy to Production** (not on PRs):
- NuGet authenticate for Covenant.Common package
- `dotnet publish` with zip
- Deploy: `AzureFunctionApp@2` to `sigook-functions`
- Production: `https://sigook-functions.azurewebsites.net`

### Sigook.CognitiveServices (.NET 8 Web App)

**Build naming:** `CognitiveServices-YYYY.M.D.r`

**Trigger:** Only `main` branch (production-only deployment).

**Stage 1 - Build:**
- .NET SDK 8.0.415
- Build solution (no tests)

**Stage 2 - Deploy to Production** (not on PRs):
- `dotnet publish` of `Sigook.CognitiveServices.UI`
- Deploy: `AzureWebApp@1` (Linux) to `sigook-cognitive-services`
- Production: `https://sigook-cognitive-services.azurewebsites.net`

### Covenant.Common (NuGet Package)

**Build naming:** `CovenantCommon-YYYY.M.D.r`

**Trigger:** Manual only (`trigger: none`, `pr: none`).

**Stage 1 - Build, Test, and Publish:**
- Quality gate: Build full Covenant.Api solution + unit tests
- Pack `Covenant.Common.csproj` (version from build number)
- Push to Azure Artifacts feed: `sigook/Covenant.Common`

---

## Reusable Templates

Located in `.azure-pipelines/templates/`:

### dotnet-setup.yml
Installs .NET SDK via `UseDotNet@2` task.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `sdkVersion` | string | required | .NET SDK version (e.g., `8.0.415`) |
| `includePreviewVersions` | boolean | `false` | Include preview SDK versions |

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
| Covenant.Web | `covenantgroup-staging.azurewebsites.net` | `covenantgroup.azurewebsites.net` |
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
- **`PatSigookPackages`** - Azure Artifacts PAT (used by IdentityServer Dockerfile for NuGet restore)
- **`SigookApp-Staging`** / **`SigookApp-Production`** - Flutter app env vars + Google Play key
- **`SigookApp-Android`** - Android keystore signing credentials

### Secure Files
- **`sigook.jks`** - Android keystore for SigookApp signing

---

## Troubleshooting

### Pipeline not triggering
- Verify the change is in the correct path (e.g., `Covenant.Api/**`)
- Check that the file is not excluded (e.g., `*.md` files are excluded)
- For Sigook.Functions and CognitiveServices, only `main` branch triggers deployment

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
