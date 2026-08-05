# CLAUDE.md

## What is Covenant/Sigook?

Covenant/Sigook is a **staffing and recruitment platform** built for the Canadian market. It connects temporary staffing agencies with companies that need workers, managing the full lifecycle: worker recruitment, job matching, time tracking, payroll (with Canadian tax deductions — CPP, EI, Federal/Provincial taxes), invoicing, and compliance.

**Core actors**: Agencies (intermediaries), Companies (clients needing staff), Workers (job seekers via mobile app), and Candidates (pre-registration prospects managed by agencies).

**Business flow**: Agency registers Company with job positions and rates → Worker registers via Flutter app → Agency approves Worker → Company/Agency creates a Request (job order) → Worker applies or gets assigned → Worker clocks in/out daily → Agency approves timesheets → System calculates pay stubs with deductions → System generates invoices for Company with markup.

**Revenue model**: Agencies profit from the markup between AgencyRate (billed to Company) and WorkerRate (paid to Worker).

## Business Rules & Documentation

Before implementing any feature, read the relevant document from `.docs/`:

| Area | Document |
|------|----------|
| Business model & actors | `.docs/business/BUSINESS_MODEL.md` |
| Billing (rates, HST/GST) | `.docs/business/BILLING_RULES.md` |
| Payroll (CPP, EI, taxes) | `.docs/business/PAYROLL_RULES.md` |
| Pay stub generation flow | `.docs/business/PAYSTUB_GENERATION.md` |
| Timesheets (OT, night, holiday) | `.docs/business/TIMESHEET_RULES.md` |
| Workflows step-by-step | `.docs/business/WORKFLOWS.md` |
| Request state management | `.docs/business/REQUEST_STATE_MANAGEMENT.md` |
| Roles & permissions (7 roles, sales scoping) | `.docs/business/ROLES_PERMISSIONS.md` |
| Architecture & stack | `.docs/technical/ARCHITECTURE.md` |
| API endpoints | `.docs/technical/openapi.json` (OpenAPI spec, generated on build) |
| Data model & relationships | `.docs/technical/ENTITIES_RELATIONSHIPS.md` |
| Development commands | `.docs/technical/DEVELOPMENT_COMMANDS.md` |
| CI/CD pipelines | `.docs/technical/PIPELINES.md` |
| Full index | `.docs/README.md` |

If you change business rules, update the corresponding `.docs/` file.

> All documents in `.docs/` are in English.

## Mandatory Rules

- **All code, variable names, and commits must be in English**
- **Git workflow**: feature branches from `dev` → PR to `dev` → merge `dev` to `main` for production
- Follow existing patterns in each project (repository pattern, DI, service layer)
- Run tests before committing (`dotnet test` for .NET, `flutter test` for Flutter)
- Don't generate code with comments, only make comments when they are requested
- Update codegraph when user confirm that everything is ok

## Monorepo Structure

| Application | Stack | Description |
|-------------|-------|-------------|
| `Covenant.Api/` | .NET 8 | Backend API — see `Covenant.Api/CLAUDE.md` |
| `SigookApp/` | Flutter | Worker mobile app — see `SigookApp/CLAUDE.md` |
| `Sigook.Web/` | Vue 3 | Agency web portal (main platform) — see `Sigook.Web/CLAUDE.md` |
| `Covenant.Web/` | Vue 3 | Marketing website (public-facing) — see `Covenant.Web/CLAUDE.md` |
| `Covenant.IdentityServer/` | .NET 6 | Authentication server (IdentityServer4) — see `Covenant.IdentityServer/CLAUDE.md` |
| `Sigook.CognitiveServices/` | .NET | AI/ML services (Azure Cognitive) — see `Sigook.CognitiveServices/CLAUDE.md` |
| `Sigook.Functions/` | Azure Functions (.NET 8) | Background jobs — see `Sigook.Functions/CLAUDE.md` |

## User Preferences

- Respond always in Spanish
- Do not auto-commit; only commit when explicitly asked
- Show the plan before executing large changes (3+ files)
- Respond in caveman mode unless I say "normal mode"
- Use the latest language features (C# file-scoped namespaces, primary constructors, etc.)