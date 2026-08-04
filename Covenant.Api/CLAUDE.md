# Covenant.Api — .NET 8 Backend

## Code Navigation

```
Controllers:     Covenant.Api/Controllers/Sigook/                       (root: Catalog, File, Location)
                 Covenant.Api/Controllers/Sigook/Agency/                (Agency, AgencyLocation)
                 Covenant.Api/Controllers/Sigook/Agency/Accounting/     (Invoices, PayStubs, Reports, LocationTax)
                 Covenant.Api/Controllers/Sigook/Agency/Sales/          (Requests, CompanyProfiles — scoped to the sales rep)
                 Covenant.Api/Controllers/Sigook/Agency/Candidates/     (Candidates, Notes, PhoneNumbers, Skills, Documents)
                 Covenant.Api/Controllers/Sigook/Agency/Workers/        (Workers, Notes, Comments, Holidays, RequestHistory)
                 Covenant.Api/Controllers/Sigook/Agency/Personnel/      (Personnel, Agencies)
                 Covenant.Api/Controllers/Jobs/                         (ScheduleTasks — called by Sigook.Functions timers)
Module controllers: Covenant.Api/{Module}Module/                        (CompanyModule, WorkerModule)
Services:        Covenant.Core.BL/Services/                             (PayStubService, RequestService, WorkerService, etc.)
                 Covenant.Core.BL/Services/Shared/                      (TimesheetCalculatorService — hours breakdown + deductions)
                 Covenant.Core.BL/Services/Invoices/                    (CanadaInvoiceService, UsaInvoiceService)
Bus consumers:   Covenant.Core.BL/Consumers/                            (Email, Invitation, NewCandidate, RequestApplicant, Teams, BulkPayStubEmail)
Entities:        Covenant.Common/Entities/{Domain}/                     (Accounting/, Agency/, Company/, Request/, Worker/, Candidate/)
Models/DTOs:     Covenant.Common/Models/{Domain}/                       (mirrors Entities structure — ALL of them, no exceptions)
Repo interfaces: Covenant.Common/Repositories/{Domain}/
Repo impls:      Covenant.Infrastructure/Repositories/{Domain}/
EF configs:      Covenant.Infrastructure/Configurations/{Domain}/
Migrations:      Covenant.Infrastructure/Migrations/
DI registration: Covenant.Api/Configuration/ApiServicesConfiguration.cs
Tests:           Covenant.Tests/
```

## Naming Conventions

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

## Patterns

- **Every model/DTO lives in `Covenant.Common/Models/{Domain}/`** — request bodies, responses, filters, view models. No `Models/` folders inside `Covenant.Api`, even for a DTO used by a single endpoint. Keep ASP.NET types (`IFormFile`) out of them: bind files as a separate controller parameter (see `InvoicesController.SendInvoiceEmail`).
- **Validators live in `Covenant.Api/Validators/{Domain}/`**, one per file, named `{Model}Validator`. Never inline them next to the model.
- All services/repos registered as `AddScoped<>` in `ApiServicesConfiguration.cs`
- Repository pattern with interfaces in `Covenant.Common`, implementations in `Covenant.Infrastructure`
- Services in `Covenant.Core.BL` depend only on repository interfaces
- EF Core configurations in `Covenant.Infrastructure/Configurations/`

## Domain Gotchas

- **`RequestStatus` enum has only 3 values:** `Open = 1`, `Filled = 3`, `Cancelled = 4` (value `2` intentionally skipped). `Status` is the single source of truth — there is no `IsOpen` flag. Transitions happen automatically inside `Request.AddWorker` / `RejectWorker` / `Cancel` / `Open`; never set `Status` directly.
- **Cancellation rule:** `Request.Cancel()` only succeeds when `Status == Open` AND `WorkersQuantityWorking == 0`. To cancel a request with assignees, reject every worker first.
- **Controller routes** use the pattern `public const string RouteName = "api/..."` + `[Route(RouteName)]`. To locate an endpoint, grep for `RouteName =`.
- **Deductions are DB table lookups, not formulas.** `TimesheetCalculatorService.CalculateDeductions` → `DeductionsRepository` range lookups by earnings/year over **two** tables: `CppDeductions` and `TaxDeductions`, both discriminated by a `PayPeriod` enum (`Weekly`/`BiWeekly`/`SemiMonthly`/`Monthly`), and `TaxDeductions` additionally by `TaxType` (`Federal`/`Provincial`). The old 12 per-period tables are gone. EI is the only computed one (`totalEarnings × rates.EmploymentInsurance`, no cap). There are no `CppCalculator`-style classes. Per-worker `WorkerProfileTaxCategory` overrides zero out deductions (subcontractors).
- **Night shift is deprecated.** Never computed: `PayStubService` hardcodes `nightShift: 0`; invoices set `NightShiftRate = 0`. Don't add night-shift logic.
- **Holiday asymmetry invoice vs pay stub:** invoices hardcode `holidayIsPaid: true` (worked holidays always billed at holiday rate); pay stubs honor the timesheet's `HolidayIsPaid` flag. Worked vs not-worked holidays are two separate flows in both.
- **Invoices do NOT bill vacations or bonus** — `VacationsRate`/`BonusRate` are stored on the entity but never enter the totals. Vacation 4% is a pay-stub concept. HST is a single global config rate (`rates.Hst`), not per-province.
- **Messaging is custom Azure Service Bus** (`SigookBusClient` + `SigookBackgroundService` + consumers in `Covenant.Core.BL/Consumers/`). There is no MassTransit. Locally the app connects to the staging Service Bus — inject `ISigookBusClient` (mockable).
- **Roles:** exactly 7, lowercase, in `Covenant.Common/Constants/CovenantConstants.cs` — `superadmin, admin, recruiting, sales, company, company.user, worker`. The old `agency`/`agency.personnel` roles were deleted; reference via `CovenantConstants.Role.*`, never string literals.

## Commands

```bash
# Build
dotnet build Covenant.Api/Covenant.Api.csproj

# Run tests
dotnet test Covenant.Tests/Covenant.Tests.csproj

# Add migration
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api
```
