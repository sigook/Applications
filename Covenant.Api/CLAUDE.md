# Covenant.Api — .NET 10 Backend + OpenIddict identity server + Azure Functions

One solution (`Covenant.Api.slnx`, SDK `10.0.401` pinned in `global.json`, `net10.0` for every project
through `Directory.Build.props`, package versions in `Directory.Packages.props`, packages from
nuget.org only through `nuget.config`).

## Code Navigation

```
Controllers:     Covenant.Api/Controllers/Identity/                     (AuthorizationController = OpenIddict /connect/* passthrough; AccountController = Razor login/confirm/reset pages; ExternalController = Microsoft 365 sign-in; PasswordController = /Password/forgot|reset; HomeController = /, /Home/Success, /Home/InvalidUser, /Home/Error)
                 Covenant.Api/Controllers/Sigook/                       (root: Catalog, File, Location, EmailPreferences)
                 Covenant.Api/Controllers/Sigook/Agency/                (Agency, AgencyLocation)
                 Covenant.Api/Controllers/Sigook/Agency/Accounting/     (Invoices, PayStubs, Reports, LocationTax, Deductions)
                 Covenant.Api/Controllers/Sigook/Agency/Sales/          (Requests, CompanyProfiles, Deals, CompanyInteractions, Dashboard — scoped to the sales rep)
                 Covenant.Api/Controllers/Sigook/Agency/Candidates/     (Candidates, Notes, PhoneNumbers, Skills, Documents)
                 Covenant.Api/Controllers/Sigook/Agency/Recruiting/     (Requests, CompanyProfiles, WeeklyBoard — scoped to recruiting)
                 Covenant.Api/Controllers/Sigook/Agency/Requests/       (Requests, Applicants, Notes, Runners, Shift, Skills, Workers, TimeSheets, WorkersReport)
                 Covenant.Api/Controllers/Sigook/Agency/Workers/        (Workers, Notes, Comments, Holidays, RequestHistory, WageHistory, TimeSheetHistory)
                 Covenant.Api/Controllers/Sigook/Agency/Personnel/      (Personnel, Agencies)
                 Covenant.Api/Controllers/Sigook/Company/               (Company, Users)
                 Covenant.Api/Controllers/Sigook/Company/Accounting/    (Invoices)
                 Covenant.Api/Controllers/Sigook/Company/Profile/       (ContactPeople, JobPositions, Locations)
                 Covenant.Api/Controllers/Sigook/Company/Requests/      (Requests, Shift, Workers, WorkerTimeSheets)
                 Covenant.Api/Controllers/Sigook/Company/Workers/       (Comments)
                 Covenant.Api/Controllers/Jobs/                         (ScheduleTasks — called by Sigook.Functions timers)
                 Covenant.Api/Security/Controllers/                     (ApiAccountController = api/Account, Identity, UserNotification)
Module controllers: Covenant.Api/{Module}Module/                        (WorkerModule — the worker's own endpoints, Policy=Worker)
Identity config: Covenant.Api/Configuration/OpenIddictConfiguration.cs  (server + local validation, certificates)
                 Covenant.Api/Configuration/Microsoft365OpenIdConnect.cs (scheme "oidc")
                 Covenant.Api/Configuration/OpenIddictSeeder.cs         (scopes, Functions client, dev clients)
Razor views:     Covenant.Api/Views/Account/, Views/Home/, Views/External/ (login UI, layout _LayoutLogin)
                 Covenant.Api/Views/Notifications/Identity/            (account emails) + Billing/, Notifications/, Website/
Static assets:   Covenant.Api/wwwroot/                                  (css/login.css, js/site.js, assets/ for the login pages)
OpenAPI:         Covenant.Api/Configuration/OpenApi/                    (document/operation transformers; UI = Scalar at /scalar)
Services:        Covenant.Core.BL/Services/                             (RequestService, WorkerService, etc.)
                 Covenant.Core.BL/Services/Identity/                    (UserAdministrationService, AccountNotificationService, UserSessionValidator, PasswordResetService)
                 Covenant.Core.BL/Services/Accounting/                  (PayStubService)
                 Covenant.Core.BL/Services/Accounting/Shared/           (TimesheetCalculatorService — hours breakdown + deductions)
                 Covenant.Core.BL/Services/Accounting/Invoices/         (CanadaInvoiceService, UsaInvoiceService)
Bus consumers:   Covenant.Core.BL/Consumers/                            (Invitation, NewCandidate, RequestApplicant, Teams, BulkPayStubEmail)
Entities:        Covenant.Common/Entities/{Domain}/                     (Accounting/, Agency/, Company/, Identity/, Request/, Worker/, Candidate/)
Models/DTOs:     Covenant.Common/Models/{Domain}/                       (mirrors Entities structure — ALL of them, no exceptions; Models/Identity = login view models)
Repo interfaces: Covenant.Common/Repositories/{Domain}/                 (Identity/IIdentityRepository for InactiveUsers, reset codes, roles, token revocation)
Repo impls:      Covenant.Infrastructure/Repositories/{Domain}/
DbContexts:      Covenant.Infrastructure/Contexts/                      (CovenantContext = API DB; IdentityContext = identity DB + OpenIddict stores; MyKeysContext)
EF configs:      Covenant.Infrastructure/Configurations/{Domain}/       (Configurations/Identity/ belong to IdentityContext only)
Migrations:      Covenant.Infrastructure/Migrations/                    (CovenantContext) and Migrations/Identity/ (IdentityContext)
DI registration: Covenant.Api/Configuration/ApiServicesConfiguration.cs (AddRepositories/AddServices/…/AddCovenantIdentity)
Functions:       Sigook.Functions/Functions/                            (ScheduleTasks timers, CraTables blob trigger) — references Covenant.Common only
Tests:           Covenant.Tests/, Covenant.Integration.Tests/ (Docker), Sigook.Functions.Tests/
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
- **Validators live in `Covenant.Api/Validators/{Domain}/`**, one per file, named `{Model}Validator`. Never inline them next to the model. Razor form models under `Models/Identity` keep DataAnnotations because MVC tag helpers render their validation summaries.
- All services/repos registered as `AddScoped<>` in `ApiServicesConfiguration.cs`
- Repository pattern with interfaces in `Covenant.Common`, implementations in `Covenant.Infrastructure`
- Services in `Covenant.Core.BL` depend only on repository interfaces (identity services also use ASP.NET Identity's `UserManager`/`RoleManager`)
- EF Core configurations in `Covenant.Infrastructure/Configurations/`; one `IEntityTypeConfiguration` per entity and file
- **Every API controller carries `[ApiController]`** — the built-in OpenAPI generator (`Microsoft.AspNetCore.OpenApi`) only describes controllers that have it; the identity controllers use `[ApiExplorerSettings(IgnoreApi = true)]` to stay out of the document.
- Sigook.Functions talks to the API over HTTP with a client-credentials token from `/connect/token`; shared DTOs/enums come from `Covenant.Common`. `PrefixKeyVaultSecretManager` is duplicated on purpose (Api + Functions) so the worker does not reference `Covenant.Infrastructure`.

## Domain Gotchas

- **`RequestStatus` enum has only 3 values:** `Open = 1`, `Filled = 3`, `Cancelled = 4` (value `2` intentionally skipped). `Status` is the single source of truth — there is no `IsOpen` flag. Transitions happen automatically inside `Request.AddWorker` / `RejectWorker` / `Cancel` / `Open`; never set `Status` directly.
- **Cancellation rule:** `Request.Cancel()` only succeeds when `Status == Open` AND `WorkersQuantityWorking == 0`. To cancel a request with assignees, reject every worker first.
- **Controller routes** use the pattern `public const string RouteName = "api/..."` + `[Route(RouteName)]`. To locate an endpoint, grep for `RouteName =`.
- **Deductions are DB table lookups, not formulas.** `TimesheetCalculatorService.CalculateDeductions` → `DeductionsRepository` range lookups by earnings/year over **two** tables: `CppDeductions` and `TaxDeductions`, both discriminated by a `PayPeriod` enum (`Weekly`/`BiWeekly`/`SemiMonthly`/`Monthly`), and `TaxDeductions` additionally by `TaxType` (`Federal`/`Provincial`). The old 12 per-period tables are gone. EI is the only computed one (`totalEarnings × rates.EmploymentInsurance`, no cap). There are no `CppCalculator`-style classes. Per-worker `WorkerProfileTaxCategory` overrides zero out deductions (subcontractors).
- **Night shift is deprecated.** Never computed: `PayStubService` hardcodes `nightShift: 0`; invoices set `NightShiftRate = 0`. Don't add night-shift logic.
- **Holiday asymmetry invoice vs pay stub:** invoices hardcode `holidayIsPaid: true` (worked holidays always billed at holiday rate); pay stubs honor the timesheet's `HolidayIsPaid` flag. Worked vs not-worked holidays are two separate flows in both.
- **Invoices do NOT bill vacations or bonus** — `VacationsRate`/`BonusRate` are stored on the entity but never enter the totals. Vacation 4% is a pay-stub concept. HST is a single global config rate (`rates.Hst`), not per-province.
- **Messaging is custom Azure Service Bus** (`SigookBusClient` + `SigookBackgroundService` + consumers in `Covenant.Core.BL/Consumers/`). There is no MassTransit. Locally the app connects to the staging Service Bus — inject `ISigookBusClient` (mockable).
- **Integration tests run on real Postgres** (Testcontainers, `Covenant.Integration.Tests/Configuration/PostgresTestDatabase.cs`): one container per test assembly, a `covenant_template` database built with `EnsureCreated()` + the SQL scripts in `Covenant.Infrastructure/Scripts/`, and one cloned database per test class. There is **no `InitialCreate` migration**, so `Database.Migrate()` cannot build the schema from scratch. Seed data comes from the builders in `Covenant.Integration.Tests/Utils/FakeData.cs` — extend those instead of hand-rolling entity graphs, since Postgres enforces every FK, unique index and NOT NULL that EF InMemory ignored. Compare dates with `DateAssert.Equal` (Postgres stores microseconds, .NET ticks are 100 ns). Tests that need `IIdentityServerService` register `UserAccountService` plus `Mock.Of<IUserAdministrationService>()`.
- **Roles:** exactly 7, lowercase, in `Covenant.Common/Constants/CovenantConstants.cs` — `superadmin, admin, recruiting, sales, company, company.user, worker`. Seeded at startup by `SigookBackgroundService`. Reference via `CovenantConstants.Role.*`, never string literals.

## Identity Gotchas

- **Two databases, two contexts.** `CovenantContext` (`ConnectionStrings:DefaultConnection`) and `IdentityContext` (`ConnectionStrings:IdentityConnection`, the former IdentityServer database). Both apply configurations from the same assembly filtered by namespace (`IdentityContext.ConfigurationsNamespace`); put identity configurations under `Configurations/Identity/` only.
- **`IdentityContext` must not call `base.OnModelCreating`**: the identity tables keep their historical shape (`User`, `Rol`, `UserLogin`/`UserToken` keyed by `UserId` only, no `AspNet*` indexes). It also `Ignore`s `IdentityUserPasskey` (.NET 10). Migrations: `dotnet ef migrations add X -p Covenant.Infrastructure -s Covenant.Api -c IdentityContext -o Migrations/Identity`.
- **`Npgsql.EnableLegacyTimestampBehavior` stays after `builder.Build()`** in `Program.cs`. `dotnet ef` and the build-time OpenAPI tool only run `Program` up to the host build, so moving it earlier flips the design-time column type of every `DateTime` (`timestamp with` ↔ `without time zone`) and generates bogus migrations. The flip side: the runtime model never matches the snapshot exactly, so every Npgsql context ignores `RelationalEventId.PendingModelChangesWarning` (EF Core 9+ turns it into an exception inside `Migrate()`). Keep that `ConfigureWarnings` when registering a context.
- **Error codes for the password grant are a contract**: `Covenant.Common/Constants/SignInErrors.cs` (`invalid_credentials`, `inactive_user`, `email_not_confirmed`, `locked_out`) — SigookApp and Sigook.Web switch on them. Same for the userinfo `role` shape (string for one role, array for several).
- **Access tokens are unencrypted JWTs** (`DisableAccessTokenEncryption`): SigookApp decodes them. Keep it that way.
- **Certificates outside Development** are loaded lazily from Key Vault when OpenIddict builds its options (`Identity:SigningCertificateName` / `EncryptionCertificateName`), not at host build, so `dotnet build` (OpenAPI generation) and `dotnet ef` never touch Key Vault certificates.
- **Default auth scheme is the OpenIddict validation (bearer) scheme.** The Identity application cookie backs only the Razor pages and `/connect/authorize`; never make it the default or API 401s turn into login redirects.
- **`sub` vs `NameIdentifier`**: read the caller id through `PrincipalExtensions.GetSubject()`/`TryGetUserId()`; OpenIddict identities carry `sub` and the test authentication handlers carry `ClaimTypes.NameIdentifier`.
- **Two Microsoft 365 app registrations.** `Identity:Microsoft365ClientId/ClientSecret` (Key Vault `{env}-api--Identity--Microsoft365Client*`) is the login + Graph `accountEnabled` registration (redirect URIs `{IssuerUri}/signin-oidc`, `User.Read.All`); `Microsoft365Configuration:ClientId/ClientSecret` only sends email through Graph. Don't merge them.
- **Sigook.Functions client**: seeded from `Identity:FunctionsClientId` / `FunctionsClientSecret` (Key Vault `{env}-api--Identity--FunctionsClient*`) on every startup; the Functions side reads the same values as `ScheduleTasks:ClientId/ClientSecret` under `{env}-func`.
- The identity database still contains the IdentityServer4 tables (`Clients`, `PersistedGrants`, …). `AddOpenIddict` copied the public clients and scopes from them; they are dropped in a later migration once the old `sigook-accounts` App Service is gone.

## Commands

```bash
# Build everything
dotnet build Covenant.Api.slnx

# Run the API (also serves /connect/*, /Account/*, /scalar)
dotnet run --project Covenant.Api/Covenant.Api.csproj

# Run unit tests
dotnet test Covenant.Tests/Covenant.Tests.csproj
dotnet test Sigook.Functions.Tests/Sigook.Functions.Tests.csproj

# Run integration tests (needs Docker running — spins up one postgres:16-alpine via Testcontainers)
dotnet test Covenant.Integration.Tests/Covenant.Integration.Tests.csproj

# Run the Azure Functions locally
cd Sigook.Functions && func start

# Add migrations (from this folder)
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api -c CovenantContext
dotnet ef migrations add MigrationName -p Covenant.Infrastructure -s Covenant.Api -c IdentityContext -o Migrations/Identity
```
