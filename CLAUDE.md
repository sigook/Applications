# CLAUDE.md

## Business Rules & Documentation

Before implementing any feature, read the relevant document from `.docs/`:

| Area | Document |
|------|----------|
| Business model & actors | `.docs/BUSINESS_MODEL.md` |
| Architecture & stack | `.docs/ARCHITECTURE.md` |
| Data model & relationships | `.docs/ENTITIES_RELATIONSHIPS.md` |
| API endpoints | `.docs/API_ENDPOINTS.md` |
| Payroll (CPP, EI, taxes) | `.docs/PAYROLL_RULES.md` |
| Billing (rates, HST/GST) | `.docs/BILLING_RULES.md` |
| Timesheets (OT, night, holiday) | `.docs/TIMESHEET_RULES.md` |
| Workflows step-by-step | `.docs/WORKFLOWS.md` |
| Request state management | `.docs/REQUEST_STATE_MANAGEMENT.md` |
| CI/CD pipelines | `.docs/PIPELINES.md` |
| Full index | `.docs/README.md` |

If you change business rules, update the corresponding `.docs/` file.

## Monorepo Structure

| Project | Tech | Purpose |
|---------|------|---------|
| `SigookApp/` | Flutter (Dart 3.9) | Mobile app - worker registration & job matching |
| `Sigook.Web/` | Vue.js 2, Node 16 | Main web platform |
| `Covenant.Web/` | Vue.js 3, TypeScript, Vite | Marketing website |
| `Covenant.Api/` | .NET 8, PostgreSQL, EF Core | Backend API (15+ projects) |
| `Covenant.IdentityServer/` | .NET 6, IdentityServer4 | Auth server (OIDC/OAuth 2.0) |
| `Sigook.Functions/` | .NET 8, Azure Functions v4 | Background jobs & scheduled tasks |
| `Sigook.CognitiveServices/` | .NET 8 | AI/Cognitive services |
| `.azure-pipelines/` | YAML | CI/CD (see `.docs/PIPELINES.md`) |

## Mandatory Rules

- **All code, comments, variable names, and commits must be in English**
- **Git workflow**: feature branches from `dev` → PR to `dev` → merge `dev` to `main` for production
- Follow existing patterns in each project (repository pattern, DI, service layer)
- Run tests before committing (`dotnet test` for .NET, `flutter test` for Flutter)

## Development Commands

### Covenant.Api (.NET 8)

```bash
dotnet build Covenant.Api/Covenant.Api.sln
dotnet run --project Covenant.Api/Covenant.Api
dotnet watch run --project Covenant.Api/Covenant.Api
dotnet test                                                    # All tests
dotnet test Covenant.Api/Covenant.Tests/Covenant.Tests.csproj  # Unit only
dotnet ef migrations add <Name> --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api
```

Key: shared cloud PostgreSQL (no local DB setup), Azure Service Bus for messaging, publishes `Covenant.Common` NuGet package.

### SigookApp (Flutter)

```bash
cd SigookApp
flutter pub get
flutter run --flavor staging -t lib/main_staging.dart
flutter run --flavor production -t lib/main_production.dart
flutter pub run build_runner build --delete-conflicting-outputs  # Code gen (Freezed, Riverpod)
flutter test
flutter analyze
```

### Sigook.Web (Vue.js 2)

```bash
cd Sigook.Web
npm ci && npm run serve          # Dev
npm run staging                  # Build staging
npm run production               # Build production
```

Note: output dir is `wwwroot/` (not `dist/`).

### Covenant.Web (Vue.js 3) - Requires Node ^20.19.0 or >=22.12.0

```bash
cd Covenant.Web
npm install && npm run dev       # Dev
npm run build:staging            # Build staging
npm run build:production         # Build production
npm run type-check               # TypeScript check
```

### Covenant.IdentityServer (.NET 6)

```bash
dotnet build Covenant.IdentityServer/Covenant.IdentityServer.sln
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer
dotnet test Covenant.IdentityServer/Covenant.IdentityServer.Tests
```

Requires `PatSigookPackages` env var for Azure Artifacts NuGet restore.

### Sigook.Functions (.NET 8 Azure Functions)

```bash
dotnet build Sigook.Functions/Sigook.Functions.sln
cd Sigook.Functions/Sigook.Functions && func start   # Local run (requires Azure Functions Core Tools v4)
```

Functions: `SendEmail` (HTTP), `SendInvitationToApply` (Queue), `NotificationSinExpiration` (Timer), `WarnLicensesExpiration` (Timer).

## Architecture Quick Reference

### Covenant.Api
- **Modules**: Agency, Company, Worker, Accounting (each with controller hierarchy)
- **Projects**: `Covenant.Common` (shared entities/interfaces), `Covenant.Infrastructure` (EF Core/repos), `Covenant.Core.BL` (business logic), `Covenant.Deductions` (tax calc), `Covenant.Documents` (Excel/PDF), `Covenant.PayStubs`, `Covenant.TimeSheetTotal`, `Covenant.Subcontractor`
- **Patterns**: Repository + DI, CQRS with MediatR, Azure Service Bus consumers, AutoMapper, FluentValidation
- **Config**: `ConnectionStrings__DefaultConnection` (PostgreSQL), Azure Storage, Azure Service Bus, SendGrid, Teams webhooks

### SigookApp
- **Clean Architecture**: Domain (entities, use cases, value objects) → Data (Freezed models/DTOs, datasources, repos) → Presentation (pages, widgets, Riverpod providers/viewmodels)
- **Key libs**: Riverpod (state), GoRouter (routing), Freezed (immutability), Dartz (`Either<Failure, Success>` error handling), Dio (HTTP), flutter_appauth (OIDC)
- **Features**: organized by domain in `lib/features/` (registration, auth, jobs)
- **Env**: `.env.staging` / `.env.production` via flutter_dotenv, build flavors in `android/app/build.gradle.kts`

### Sigook.Web
- **Stack**: Vue 2.6 + Vuex + Vue Router + Buefy + Axios + OIDC Client
- **Auth**: OIDC-based (`src/security/`), i18n via `vue-i18n` (`src/lang/`)

### Covenant.Web
- **Stack**: Vue 3.5 + TypeScript + Pinia + Vue Router + Vite
- **Alias**: `@` maps to `src/` (configured in `vite.config.ts`)

## CI/CD

Pipelines use path-based triggers (each app deploys independently). Branch `dev` → Staging, `main` → Production. PRs to `dev` run full validation; PRs to `main` skip validation (already tested on dev). See `.docs/PIPELINES.md` for full details.
