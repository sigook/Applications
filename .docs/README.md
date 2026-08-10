# Covenant/Sigook Platform - Documentation Index

## Business

| Document | Description |
|----------|-------------|
| [BUSINESS_MODEL.md](./business/BUSINESS_MODEL.md) | Business model, actors, value proposition, and system flow |
| [BILLING_RULES.md](./business/BILLING_RULES.md) | Invoice generation, HST/GST, rates, markup calculations |
| [PAYROLL_RULES.md](./business/PAYROLL_RULES.md) | Pay stub generation flow, payroll calculations, deductions (CPP, EI), federal and provincial taxes |
| [TIMESHEET_RULES.md](./business/TIMESHEET_RULES.md) | Hours calculations (regular, overtime, night shift, holiday), validations |
| [WORKFLOWS.md](./business/WORKFLOWS.md) | Detailed step-by-step flows (Worker Registration, Job Matching, Payroll, etc.) |
| [REQUEST_STATE_MANAGEMENT.md](./business/REQUEST_STATE_MANAGEMENT.md) | Request lifecycle and state transitions |
| [ROLES_PERMISSIONS.md](./business/ROLES_PERMISSIONS.md) | The 7 roles, role groups, sales data scoping, user creation rules |

## Technical

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](./technical/ARCHITECTURE.md) | Tech stack, layers, modules, and project organization |
| `technical/openapi.json` | OpenAPI 3.0 specification — the source of truth for every endpoint. Generated on each API build and committed; regenerate after adding endpoints. Browse it via Swagger UI at the API site root in Dev/Staging |
| [ENTITIES_RELATIONSHIPS.md](./technical/ENTITIES_RELATIONSHIPS.md) | Main entities, relationships, and data model diagrams |
| [DEVELOPMENT_COMMANDS.md](./technical/DEVELOPMENT_COMMANDS.md) | Build, run, and test commands for each project |
| [PIPELINES.md](./technical/PIPELINES.md) | Azure DevOps CI/CD pipelines, triggers, templates, deployment URLs |
| [SIGOOK_WEB_API_MAP.md](./technical/SIGOOK_WEB_API_MAP.md) | Sigook.Web (Vue 3 agency portal) — every `src/api/*.ts` file mapped to backend endpoints, types, and Pinia stores |
| [SIGOOK_WEB_STRUCTURE.md](./technical/SIGOOK_WEB_STRUCTURE.md) | Sigook.Web — folder layout, routes, views grouped by feature, Pinia stores, global plumbing |

---

## Quick Start

**Understand the business:**
1. [BUSINESS_MODEL.md](./business/BUSINESS_MODEL.md) — what problem the platform solves
2. [WORKFLOWS.md](./business/WORKFLOWS.md) — main flows

**Backend development:**
1. [ARCHITECTURE.md](./technical/ARCHITECTURE.md) — layers and modules
2. [ENTITIES_RELATIONSHIPS.md](./technical/ENTITIES_RELATIONSHIPS.md) — data model
3. `technical/openapi.json` — available endpoints (OpenAPI spec; build it, or use Swagger UI)

**Modify payroll:**
1. [PAYROLL_RULES.md](./business/PAYROLL_RULES.md) — calculation rules
2. Code: `Covenant.Api/Covenant.Core.BL/Services/Accounting/` (PayStubService, DeductionImportService)

**Modify billing:**
1. [BILLING_RULES.md](./business/BILLING_RULES.md) — billing rules
2. Code: `Covenant.Api/Covenant.Core.BL/Services/Invoices/` (CanadaInvoiceService, UsaInvoiceService)

**Modify timesheets:**
1. [TIMESHEET_RULES.md](./business/TIMESHEET_RULES.md) — hours calculation rules
2. Code: `Covenant.Api/Covenant.Core.BL/Services/TimeSheetService.cs`

---

## Maintenance

Keep this documentation updated when you change business rules, add modules/services, modify the architecture, add endpoints, or change the data model.
