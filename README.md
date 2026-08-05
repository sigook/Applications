# Covenant / Sigook

Monorepo for the Covenant/Sigook **staffing and recruitment platform** for the Canadian market. It connects temporary staffing agencies with companies that need workers and manages the full lifecycle: recruitment, job matching, time tracking, payroll (CPP, EI, federal/provincial taxes), invoicing, and compliance.

**Core actors**: Agencies (intermediaries), Companies (clients), Workers (via mobile app), and Candidates (prospects managed by agencies).

**Business flow**: Agency registers a Company with positions and rates → Worker registers via the mobile app → Agency approves the Worker → a Request (job order) is created → Worker applies or is assigned → Worker clocks in/out daily → Agency approves timesheets → the system generates pay stubs with deductions and invoices the Company with markup.

**Revenue model**: agencies profit from the spread between the AgencyRate (billed to the Company) and the WorkerRate (paid to the Worker).

## Applications

| Application | Stack | Description |
|-------------|-------|-------------|
| [`Covenant.Api/`](Covenant.Api/) | .NET 8 | Backend API (REST, EF Core, PostgreSQL) |
| [`SigookApp/`](SigookApp/) | Flutter | Worker mobile app (iOS/Android) |
| [`Sigook.Web/`](Sigook.Web/) | Vue 3 + Vite | Agency web portal (main platform) |
| [`Covenant.Web/`](Covenant.Web/) | Vue 3 + Vite | Marketing website (public-facing) |
| [`Covenant.IdentityServer/`](Covenant.IdentityServer/) | .NET 6 | Authentication server (IdentityServer4) |
| [`Sigook.CognitiveServices/`](Sigook.CognitiveServices/) | .NET | AI/ML services (Azure Cognitive) |
| [`Sigook.Functions/`](Sigook.Functions/) | Azure Functions (.NET 8) | Background jobs (timers, blob triggers) |
| `Sigook.Database/` | SQL | Database project |

## Documentation

Business rules and technical documentation live in [`.docs/`](.docs/README.md). Start here:

- [Business model](.docs/business/BUSINESS_MODEL.md) — actors, value proposition, system flow
- [Workflows](.docs/business/WORKFLOWS.md) — step-by-step flows (registration, matching, payroll)
- [Architecture](.docs/technical/ARCHITECTURE.md) — tech stack, layers, modules
- [Development commands](.docs/technical/DEVELOPMENT_COMMANDS.md) — build, run, and test commands for every project
- [CI/CD pipelines](.docs/technical/PIPELINES.md) — Azure DevOps pipelines and deployment URLs
- `.docs/technical/openapi.json` — OpenAPI spec, source of truth for every endpoint (regenerated on API build)

## Getting started

### Prerequisites

| Tool | Version | Used by |
|------|---------|---------|
| .NET SDK | 8.0 (and 6.0 for IdentityServer) | `Covenant.Api`, `Sigook.Functions`, `Covenant.IdentityServer` |
| Node.js | `^20.19.0 \|\| >=22.12.0` (engine-strict) | `Sigook.Web`, `Covenant.Web` |
| pnpm | pinned via `packageManager` (`corepack enable`) | `Sigook.Web`, `Covenant.Web` |
| Flutter SDK | stable | `SigookApp` |
| Azure Functions Core Tools | v4 | `Sigook.Functions` (local run) |

No local database setup is required: development runs against a shared cloud PostgreSQL, and messaging uses Azure Service Bus (staging).

### Quick start per app

```bash
# Backend API
dotnet run --project Covenant.Api/Covenant.Api

# Agency portal
cd Sigook.Web && pnpm install && pnpm run dev

# Marketing site
cd Covenant.Web && pnpm install && pnpm run dev

# Worker mobile app
cd SigookApp && flutter pub get && flutter run --flavor staging -t lib/main_staging.dart

# Identity server
dotnet run --project Covenant.IdentityServer/Covenant.IdentityServer
```

Full command reference (tests, migrations, builds per environment): [DEVELOPMENT_COMMANDS.md](.docs/technical/DEVELOPMENT_COMMANDS.md).

### Running tests

```bash
dotnet test                 # .NET (from the solution folder)
flutter test                # SigookApp
pnpm run type-check && pnpm run lint   # Vue apps
```

Run the relevant tests before committing.

## Git workflow

- Feature branches are created from `dev` and merged back via PR.
- `dev` is merged into `main` for production releases.
- All code, variable names, and commit messages are in English.

## Developer setup

### CodeGraph (recommended)

The repo ships a [CodeGraph](https://www.npmjs.com/package/@colbymchenry/codegraph) setup so any AI assistant
(Claude Code, Cursor, Codex CLI, opencode) can answer structural questions — who calls what, where a symbol is
defined, what breaks if you change it — from a tree-sitter index instead of grepping.

What is committed: the MCP server registration (`.mcp.json`), the shared parser config (`.codegraph/config.json`)
and the tool permissions (`.claude/settings.json`).
What is not: the index itself (`.codegraph/codegraph.db`, ~80 MB, machine-local and regenerated).

```bash
npm install -g @colbymchenry/codegraph
codegraph index            # build the local index, first run takes a few minutes
codegraph status           # verify
```

Run these from the repo root. Do **not** run `codegraph init` — it would overwrite the shared `config.json`.
The MCP server starts automatically when you open the repo in Claude Code; the file watcher keeps the index in
sync from then on. If a query ever looks stale, re-run `codegraph index`.
