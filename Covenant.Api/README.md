# Covenant API

.NET 10 backend for the Covenant/Sigook staffing platform: REST API, OpenIddict authorization server
(OpenID Connect / OAuth 2.0) and the Azure Functions that run the scheduled jobs, all in one solution
(`Covenant.Api.slnx`).

## Projects

| Project | Purpose |
|---|---|
| `Covenant.Api` | Web API, identity host (`/connect/*`, `/Account/*`), Razor views, DI, background service |
| `Covenant.Common` | Entities, enums, models/DTOs, repository and service interfaces, constants |
| `Covenant.Core.BL` | Business services (agency, company, worker, accounting, identity) and Service Bus consumers |
| `Covenant.Infrastructure` | EF Core contexts (`CovenantContext`, `IdentityContext`), repositories, integrations |
| `Covenant.Documents` | Excel/PDF generators (MediatR handlers) |
| `Sigook.Functions` | Azure Functions (isolated worker): timers + CRA table blob trigger |
| `Covenant.Tests` / `Covenant.Integration.Tests` / `Sigook.Functions.Tests` | Tests |

Architecture, conventions and business rules: [`../.docs/`](../.docs/README.md) and [`CLAUDE.md`](CLAUDE.md).

## Prerequisites

- .NET SDK **10.0.401** (`global.json`)
- Azure CLI signed in (`az login`) — configuration comes from Key Vault `sigook` (prefix `staging-api` outside Production)
- Docker (integration tests only)
- Azure Functions Core Tools v4 (to run `Sigook.Functions` locally)

No local database: development runs against the shared cloud PostgreSQL databases (API and identity)
and the staging Service Bus, all resolved from Key Vault.

## Build, run, test

```bash
dotnet build Covenant.Api.slnx
dotnet run --project Covenant.Api/Covenant.Api.csproj          # https://localhost:44307
dotnet watch run --project Covenant.Api/Covenant.Api.csproj

dotnet test Covenant.Tests/Covenant.Tests.csproj
dotnet test Sigook.Functions.Tests/Sigook.Functions.Tests.csproj
dotnet test Covenant.Integration.Tests/Covenant.Integration.Tests.csproj   # needs Docker

cd Sigook.Functions && func start
```

Useful URLs in Development and Staging:

| URL | What |
|---|---|
| `/scalar` | API reference (Scalar) over the OpenAPI document at `/openapi/v1.json` |
| `/.well-known/openid-configuration` | OpenID Connect discovery |
| `/Account/Login` | Razor login page |
| `/health`, `/healthz`, `/ready`, `/live` | Health checks |

The OpenAPI document is also written to `.docs/technical/openapi.json` on every local build.

## Migrations

Migrations are applied automatically at startup by `SigookBackgroundService`. To add one:

```bash
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api -c CovenantContext
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api -c IdentityContext -o Migrations/Identity
```

## Docker

```bash
# from this folder — the Dockerfile only packages a published output
dotnet publish Covenant.Api/Covenant.Api.csproj -c Release -o publish
docker build -f Dockerfile -t covenant-api publish
```

The image serves HTTP on port 80 and reads its configuration from Key Vault through the App Service
managed identity.
