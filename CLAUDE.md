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
| Architecture & stack | `.docs/technical/ARCHITECTURE.md` |
| API endpoints | `.docs/technical/API_ENDPOINTS.md` |
| Data model & relationships | `.docs/technical/ENTITIES_RELATIONSHIPS.md` |
| Development commands | `.docs/technical/DEVELOPMENT_COMMANDS.md` |
| CI/CD pipelines | `.docs/technical/PIPELINES.md` |
| Full index | `.docs/README.md` |

If you change business rules, update the corresponding `.docs/` file.

## Mandatory Rules

- **All code, comments, variable names, and commits must be in English**
- **Git workflow**: feature branches from `dev` → PR to `dev` → merge `dev` to `main` for production
- Follow existing patterns in each project (repository pattern, DI, service layer)
- Run tests before committing (`dotnet test` for .NET, `flutter test` for Flutter)

## Code Navigation

### Covenant.Api (.NET 8)

```
Controllers:     Covenant.Api/Covenant.Api/Controllers/Sigook/          (root: Catalog, File, Location)
                 Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/   (Agency, AgencyLocation)
                 Covenant.Api/Covenant.Api/Controllers/Sigook/Agency/Accounting/ (Invoices, PayStubs, Reports)
Module controllers: Covenant.Api/Covenant.Api/{Module}Module/           (AccountingModule, AgencyModule, CompanyModule, WorkerModule, ManagerModule)
Services:        Covenant.Api/Covenant.Core.BL/Services/                (PayStubService, RequestService, WorkerService, etc.)
                 Covenant.Api/Covenant.Core.BL/Services/Invoices/       (CanadaInvoiceService, UsaInvoiceService)
Entities:        Covenant.Api/Covenant.Common/Entities/{Domain}/        (Accounting/, Agency/, Company/, Request/, Worker/, Candidate/)
Models/DTOs:     Covenant.Api/Covenant.Common/Models/{Domain}/          (mirrors Entities structure)
Repo interfaces: Covenant.Api/Covenant.Common/Repositories/{Domain}/
Repo impls:      Covenant.Api/Covenant.Infrastructure/Repositories/{Domain}/
EF configs:      Covenant.Api/Covenant.Infrastructure/Configurations/{Domain}/
Migrations:      Covenant.Api/Covenant.Infrastructure/Migrations/
DI registration: Covenant.Api/Covenant.Api/Configuration/ApiServicesConfiguration.cs
Tests:           Covenant.Api/Covenant.Tests/
```

### SigookApp (Flutter)

```
Features:        SigookApp/lib/features/{feature}/                      (auth, registration, jobs, profile, history, catalog)
  Each feature:    domain/ (entities, repositories, usecases)
                   data/ (models, datasources, repositories impl)
                   presentation/ (pages, widgets, viewmodels, providers)
Core:            SigookApp/lib/core/                                    (config, network, routing, theme, providers, error, widgets)
```

### Sigook.Web (Vue 2)

```
Components:      Sigook.Web/src/components/{domain}/
Pages:           Sigook.Web/src/pages/
Store:           Sigook.Web/src/store/modules/
Auth:            Sigook.Web/src/security/
i18n:            Sigook.Web/src/lang/
```

### Covenant.Web (Vue 3)

```
Components:      Covenant.Web/src/components/{feature}/
Views:           Covenant.Web/src/views/
Stores:          Covenant.Web/src/stores/
Composables:     Covenant.Web/src/composables/
Services:        Covenant.Web/src/services/
```

## Naming Conventions

### Covenant.Api

| Type | Pattern | Example |
|------|---------|---------|
| Entity | `{Name}.cs` | `PayStub.cs`, `Invoice.cs` |
| Child entity | `{Parent}{Child}.cs` | `PayStubItem.cs`, `InvoiceDiscount.cs` |
| Service interface | `I{Name}Service.cs` | `IPayStubService.cs` |
| Service impl | `{Name}Service.cs` | `PayStubService.cs` |
| Repository interface | `I{Name}Repository.cs` | `IPayStubRepository.cs` |
| Repository impl | `{Name}Repository.cs` | `PayStubRepository.cs` |
| EF configuration | `{Entity}Configuration.cs` | `PayStubHistoryConfiguration.cs` |
| Create model | `Create{Name}Model.cs` | `CreatePayStubModel.cs` |
| Detail model | `{Name}DetailModel.cs` | `PayStubDetailModel.cs` |
| List model | `{Name}ListModel.cs` | `InvoiceListModel.cs` |
| Filter model | `Get{Name}Filter.cs` | `GetPayStubsFilter.cs` |
| Versioned controller | `{Module}{Resource}V{N}Controller.cs` | `AccountingPayStubV4Controller.cs` |

All services/repos registered as `AddScoped<>` in `ApiServicesConfiguration.cs`.

### SigookApp (Flutter)

| Type | Pattern | Example |
|------|---------|---------|
| Model (Freezed) | `{name}_model.dart` | `job_model.dart` |
| Entity | `{name}.dart` | `job.dart`, `timesheet_entry.dart` |
| Provider | `{name}_provider.dart` | `core_providers.dart` |
| ViewModel | `{name}_viewmodel.dart` | `registration_viewmodel.dart` |

### Sigook.Web / Covenant.Web (Vue)

| Type | Pattern | Example |
|------|---------|---------|
| Component | `PascalCase.vue` | `ProfileForm.vue`, `HeroSection.vue` |
| Store (Vuex/Pinia) | `camelCase.js/.ts` | `workers.js`, `jobs.ts` |

## User Preferences

- Respond always in Spanish
- Do not auto-commit; only commit when explicitly asked
- Show the plan before executing large changes (3+ files)
- When working on payroll or billing, always run `dotnet test Covenant.Api/Covenant.Tests/Covenant.Tests.csproj` before presenting the result
