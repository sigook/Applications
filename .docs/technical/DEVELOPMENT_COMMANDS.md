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

Key: shared cloud PostgreSQL (no local DB setup), Azure Service Bus for messaging.

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

Uses **pnpm** (pinned via `packageManager` field). Enable via `corepack enable` or `npm i -g pnpm`.

```bash
cd Sigook.Web
pnpm install && pnpm run dev     # Dev (Vite)
pnpm run staging                 # Build staging (Vite)
pnpm run production              # Build production (Vite)
pnpm run type-check              # TypeScript check
pnpm run lint                    # ESLint
pnpm run preview                 # Preview production build
```

Note: output dir is `wwwroot/` (not `dist/`). Build tool is Vite; state is Pinia; UI is `@ntohq/buefy-next`.

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

## Covenant.IdentityServer (.NET 6)

```bash
dotnet build Covenant.IdentityServer/Covenant.IdentityServer.sln
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer
dotnet test Covenant.IdentityServer/Covenant.IdentityServer.Tests
```

No dependency on `Covenant.Common` — IdentityServer vendors its own copies of shared types.

## Sigook.Functions (.NET 8 Azure Functions)

```bash
dotnet build Sigook.Functions/Sigook.Functions.sln
cd Sigook.Functions/Sigook.Functions && func start   # Local run (requires Azure Functions Core Tools v4)
```

Functions (both Timer triggers, defined in `Sigook.Functions/Functions/ScheduleTasks.cs`): `NotificationSinExpiration`, `WarnLicensesExpiration`.
