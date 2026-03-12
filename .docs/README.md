# Covenant/Sigook Platform - Documentation Index

## Business

| Document | Description |
|----------|-------------|
| [BUSINESS_MODEL.md](./business/BUSINESS_MODEL.md) | Business model, actors, value proposition, and system flow |
| [BILLING_RULES.md](./business/BILLING_RULES.md) | Invoice generation, HST/GST, rates, markup calculations |
| [PAYROLL_RULES.md](./business/PAYROLL_RULES.md) | Payroll calculations, deductions (CPP, EI), federal and provincial taxes |
| [PAYSTUB_GENERATION.md](./business/PAYSTUB_GENERATION.md) | Step-by-step pay stub generation flow (`GeneratePayStubForWorker`) |
| [TIMESHEET_RULES.md](./business/TIMESHEET_RULES.md) | Hours calculations (regular, overtime, night shift, holiday), validations |
| [WORKFLOWS.md](./business/WORKFLOWS.md) | Detailed step-by-step flows (Worker Registration, Job Matching, Payroll, etc.) |
| [REQUEST_STATE_MANAGEMENT.md](./business/REQUEST_STATE_MANAGEMENT.md) | Request lifecycle and state transitions |

## Technical

| Document | Description |
|----------|-------------|
| [ARCHITECTURE.md](./technical/ARCHITECTURE.md) | Tech stack, layers, modules, and project organization |
| [API_ENDPOINTS.md](./technical/API_ENDPOINTS.md) | Complete endpoint documentation by module (Agency, Company, Worker, Accounting) |
| [ENTITIES_RELATIONSHIPS.md](./technical/ENTITIES_RELATIONSHIPS.md) | Main entities, relationships, and data model diagrams |
| [DEVELOPMENT_COMMANDS.md](./technical/DEVELOPMENT_COMMANDS.md) | Build, run, and test commands for each project |
| [PIPELINES.md](./technical/PIPELINES.md) | Azure DevOps CI/CD pipelines, triggers, templates, deployment URLs |

---

## Quick Start

**Understand the business:**
1. [BUSINESS_MODEL.md](./business/BUSINESS_MODEL.md) — what problem the platform solves
2. [WORKFLOWS.md](./business/WORKFLOWS.md) — main flows

**Backend development:**
1. [ARCHITECTURE.md](./technical/ARCHITECTURE.md) — layers and modules
2. [ENTITIES_RELATIONSHIPS.md](./technical/ENTITIES_RELATIONSHIPS.md) — data model
3. [API_ENDPOINTS.md](./technical/API_ENDPOINTS.md) — available endpoints

**Modify payroll:**
1. [PAYROLL_RULES.md](./business/PAYROLL_RULES.md) — calculation rules
2. Code: `Covenant.Api/Covenant.PayStubs/` and `Covenant.Api/Covenant.Deductions/`

**Modify billing:**
1. [BILLING_RULES.md](./business/BILLING_RULES.md) — billing rules
2. Code: `Covenant.Api/Covenant.Core.BL/Services/Invoices/` (CanadaInvoiceService, UsaInvoiceService)

**Modify timesheets:**
1. [TIMESHEET_RULES.md](./business/TIMESHEET_RULES.md) — hours calculation rules
2. Code: `Covenant.Api/Covenant.Core.BL/Services/TimeSheetService.cs`

---

## Maintenance

Keep this documentation updated when you change business rules, add modules/services, modify the architecture, add endpoints, or change the data model.
