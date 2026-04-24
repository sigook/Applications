# Development Commands

## Covenant.Api (.NET 8)

```bash
dotnet build Covenant.Api/Covenant.Api.sln
dotnet run --project Covenant.Api/Covenant.Api
dotnet watch run --project Covenant.Api/Covenant.Api
dotnet test                                                    # All tests
dotnet test Covenant.Api/Covenant.Tests/Covenant.Tests.csproj  # Unit only
dotnet ef migrations add <Name> --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api
```

Key: shared cloud PostgreSQL (no local DB setup), Azure Service Bus for messaging, publishes `Covenant.Common` NuGet package.

## SigookApp (Flutter)

```bash
cd SigookApp
flutter pub get
flutter run --flavor staging -t lib/main_staging.dart
flutter run --flavor production -t lib/main_production.dart
flutter pub run build_runner build --delete-conflicting-outputs  # Code gen (Freezed, Riverpod)
flutter test
flutter analyze
```

## Sigook.Web (Vue.js 3) - Requires Node 20+

```bash
cd Sigook.Web
npm ci && npm run dev            # Dev (Vite)
npm run staging                  # Build staging (Vite)
npm run production               # Build production (Vite)
npm run type-check               # TypeScript check
npm run lint                     # ESLint
npm run preview                  # Preview production build
```

Note: output dir is `wwwroot/` (not `dist/`). Build tool is Vite; state is Pinia; UI is `@ntohq/buefy-next`.

## Covenant.Web (Vue.js 3) - Requires Node ^20.19.0 or >=22.12.0

```bash
cd Covenant.Web
npm install && npm run dev       # Dev
npm run build:staging            # Build staging
npm run build:production         # Build production
npm run type-check               # TypeScript check
```

## Covenant.IdentityServer (.NET 6)

```bash
dotnet build Covenant.IdentityServer/Covenant.IdentityServer.sln
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer
dotnet test Covenant.IdentityServer/Covenant.IdentityServer.Tests
```

Requires `PatSigookPackages` env var for Azure Artifacts NuGet restore.

## Sigook.Functions (.NET 8 Azure Functions)

```bash
dotnet build Sigook.Functions/Sigook.Functions.sln
cd Sigook.Functions/Sigook.Functions && func start   # Local run (requires Azure Functions Core Tools v4)
```

Functions: `SendEmail` (HTTP), `SendInvitationToApply` (Queue), `NotificationSinExpiration` (Timer), `WarnLicensesExpiration` (Timer).
