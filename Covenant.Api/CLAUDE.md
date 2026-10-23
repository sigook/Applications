# Covenant.Api — .NET 10 Backend + OpenIddict identity server + Azure Functions

One solution (`Covenant.Api.slnx`, SDK `10.0.401` pinned in `global.json`, `net10.0` for every project
through `Directory.Build.props`, package versions in `Directory.Packages.props`, packages from
nuget.org only through `nuget.config`).

## Code Navigation

```
Controllers:     Covenant.Api/Controllers/{Portal}/{Menu}/{Feature}/     folder = route: Controllers/Agencies/Recruiting/Requests/ serves api/agency/recruiting/requests (lowercase class prefix, action templates keep their casing)
                 Covenant.Api/Controllers/Identity/                     (AuthorizationController = OpenIddict /connect/* passthrough; AccountController = POST /Account/ConfirmEmail|CreatePassword|ResendConfirmationLink + legacy email-link redirects; ExternalController = Microsoft 365 sign-in; PasswordController = /Password/forgot|reset; HomeController = / and /Home/InvalidUser, redirects to Sigook.Web)
                 Covenant.Api/Controllers/Shared/                       (any role / anonymous: Catalog, Location, EmailPreferences, Website, MobileApp → api/catalog, api/location, api/emailpreferences, api/website, api/mobileapp; files are never served by the API, every environment reads blobs directly through `FilesConfiguration:FilesPath`; `MobileApp` serves the forced-update gate of Sigook.App from `MobileAppConfiguration` — bump `MobileAppConfiguration__MinimumVersion` in App Settings to lock out older builds)
                 Covenant.Api/Controllers/Shared/Account/               (the caller's own account, bearer: UserAccount = api/account/* + PATCH /identity; UserNotification = api/usernotification)
                 Covenant.Api/Controllers/Shared/Jobs/                  (machine-to-machine, called by Sigook.Functions: ScheduleTasks = api/jobs/scheduletasks, Deductions = api/jobs/deductions)
                 Covenant.Api/Controllers/Agencies/                       (Notifications = api/agency/notifications, the sidebar bell)
                 Covenant.Api/Controllers/Agencies/Profile/               (own agency: AgencyProfile = api/agency/profile, Locations, Personnel, PersonnelAgencies, Attendance)
                 Covenant.Api/Controllers/Agencies/Recruiting/Requests/   (RequestsList = lists/Excel, Requests = detail/CRUD, Applicants, Runners, Workers, TimeSheets, WorkerTimeSheets, WorkerNotes, WorkersReport, Notes, Shift, Skills, ReportTo, RequestedBy → api/agency/recruiting/requests[/{requestId}/…])
                 Covenant.Api/Controllers/Agencies/Recruiting/Applicants/ (cross-request applicants board)
                 Covenant.Api/Controllers/Agencies/Recruiting/WeeklyBoard/
                 Covenant.Api/Controllers/Agencies/Recruiting/Candidates/ (Candidates, Notes, PhoneNumbers, Skills, Documents)
                 Covenant.Api/Controllers/Agencies/Recruiting/Workers/    (Workers, Notes, Comments, Holidays, RequestHistory, WageHistory, TimeSheetHistory)
                 Covenant.Api/Controllers/Agencies/Recruiting/Clients/    (ClientsList = lists/Excel, Clients = CRUD, ContactInformation, ContactPeople, Documents, InvoiceNotes, InvoiceRecipients, JobPositions, Locations, Logo, Notes, Users → api/agency/recruiting/clients[/{profileId}/…])
                 Covenant.Api/Controllers/Agencies/Sales/                 (sales-rep scoped: Dashboard, Clients, Requests; Agencies = sub-agencies → api/agency/sales/*)
                 Covenant.Api/Controllers/Agencies/Sales/Clients/         (owner-scoped Deals + Interactions under {profileId}, Policy=Sales)
                 Covenant.Api/Controllers/Agencies/Accounting/            (Invoices, PayStubs, Reports, LocationTax → api/agency/accounting/*)
                 Covenant.Api/Controllers/Companies/Profile/              (Company = api/company/profile, Users, ContactPeople, JobPositions, Locations)
                 Covenant.Api/Controllers/Companies/Requests/             (Requests, Shift, Workers, WorkerTimeSheets → api/company/requests[/{requestId}/…])
                 Covenant.Api/Controllers/Companies/Invoices/             (api/company/invoices)
                 Covenant.Api/Controllers/Companies/Workers/              (Comments)
                 Covenant.Api/Controllers/Workers/                       (the worker's own endpoints, Policy=Worker: Profile/, Requests/, History/, Comments/, TimeSheets/ → api/worker/*)
Identity config: Covenant.Api/Configuration/OpenIddictConfiguration.cs  (server + local validation, certificates)
                 Covenant.Api/Configuration/Microsoft365OpenIdConnect.cs (scheme "oidc")
                 Covenant.Api/Configuration/OpenIddictSeeder.cs         (scopes, Functions client, dev clients)
Razor views:     Covenant.Api/Views/Notifications/Identity/            (account emails) + Billing/, Notifications/, Website/ — email templates only, no pages (invoice/pay stub PDFs are QuestPDF handlers in Covenant.Documents)
Static assets:   Covenant.Api/wwwroot/assets/images/                    (images referenced by the email templates; `defaults/` = source of the default logo blobs `company.png`/`agency.png`/`worker.png` uploaded to the `files` container — the API never generates images)
OpenAPI:         Covenant.Api/Configuration/OpenApi/                    (document/operation transformers; UI = Scalar at /scalar)
Services:        Covenant.Core.BL/Services/{Domain}/                    by domain, PLURAL folders (Agencies/, Candidates/, Companies/, Locations/, Notifications/, Requests/, Sales/, Workers/, Shared/) so the namespace never shadows the entity; Covenant.Core.BL/Interfaces/ mirrors the tree
                 Covenant.Core.BL/Services/Requests/                    (RequestService, RequestApplicantService, RequestApplicantNotificationService, RequestInvitationService, RequestDetailsService, RunnerService, WeeklyBoardService, TimesheetService)
                 Covenant.Core.BL/Services/Identity/                    (UserAdministrationService, AccountNotificationService, UserSessionValidator, PasswordResetService — interfaces stay in Covenant.Common/Interfaces/Identity/ because Infrastructure implements/consumes them)
                 Covenant.Core.BL/Services/Accounting/                  (AccountingService, TimesheetCalculatorService — hours breakdown + deductions)
                 Covenant.Core.BL/Services/Accounting/{Deductions,Invoices,PayStubs}/ (DeductionImportService; InvoiceService + CanadaInvoiceStrategy/UsaInvoiceStrategy + InvoiceCountryStrategyResolver; PayStubService)
Bus consumers:   Covenant.Api/Consumers/                                (Invitation, NewCandidate, RequestApplicant, Teams, BulkPayStubEmail — thin: deserialize, call one Core.BL service, complete)
Entities:        Covenant.Common/Entities/{Domain}/                     singular (Accounting/{Deductions,Invoice,PayStub,Subcontractor}/, Agency/, Candidate/, Company/, Identity/, Request/{,Runners}/, Sales/ = Deal + CompanyInteraction, Worker/); catalogs and Location* in the root
Models/DTOs:     Covenant.Common/Models/{Domain}/                       same singular domains as Entities + Location/, Notification/, Website/, Sales/{,Dashboard}/. Inside a domain, the root holds what is shared or belongs to the domain's own portal; DTOs consumed by ONE other portal go in Agency/, Company/ or Worker/ (Models/Workers/Agency/ = agency view of workers, Models/Requests/{Agency,Company,Worker}/). Never Domain/Domain/
Repo interfaces: Covenant.Common/Repositories/{Domain}/                 PLURAL (Agencies/, Candidates/, Companies/, Identity/, Notifications/, Requests/, Workers/, Accounting/{Deductions,Invoices,PayStubs,Subcontractors}/; Catalog/Location/Shift/User in the root)
Repo impls:      Covenant.Infrastructure/Repositories/{Domain}/         same tree
DbContexts:      Covenant.Infrastructure/Contexts/                      (CovenantContext = API DB; IdentityContext = identity DB + OpenIddict stores; MyKeysContext)
EF configs:      Covenant.Infrastructure/Configurations/{Domain}/       same plural tree + Sales/ + Requests/Runners/ (Configurations/Identity/ belongs to IdentityContext only — exact namespace match, never nest folders under it)
Validators:      Covenant.Api/Validators/{Domain}/                      PLURAL (Agencies/, Candidates/, Companies/, Identity/, Locations/, Requests/, Sales/, Website/, Workers/, Accounting/Deductions/)
Migrations:      Covenant.Infrastructure/Migrations/                    (CovenantContext) and Migrations/Identity/ (IdentityContext)
DI registration: Covenant.Api/Configuration/ApiServicesConfiguration.cs (AddRepositories/AddServices/…/AddCovenantIdentity)
Functions:       Sigook.Functions/Functions/                            (ScheduleTasks timers, CraTables blob trigger) — references Covenant.Common only
Tests:           Covenant.Tests/{Domain}/ (plural, like Services), Covenant.Integration.Tests/{Portal}/{Menu}/{Feature}/ (mirrors Controllers/, Docker), Sigook.Functions.Tests/
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
| EF configuration | `{Entity}Configuration.cs` | `PayStubConfiguration.cs` |
| Create model | `Create{Name}Model.cs` | `CreatePayStubModel.cs` |
| Detail model | `{Name}DetailModel.cs` | `PayStubDetailModel.cs` |
| List model | `{Name}ListModel.cs` | `InvoiceListModel.cs` |
| Filter model | `Get{Name}Filter.cs` | `GetPayStubsFilter.cs` |

## Patterns

- **Every model/DTO lives in `Covenant.Common/Models/{Domain}/`** — request bodies, responses, filters, view models. No `Models/` folders inside `Covenant.Api`, even for a DTO used by a single endpoint. Keep ASP.NET types (`IFormFile`) out of them: bind files as a separate controller parameter (see `InvoicesController.SendInvoiceEmail`).
- **Validators live in `Covenant.Api/Validators/{Domain}/`**, one per file, named `{Model}Validator`. Never inline them next to the model.
- All services/repos registered as `AddScoped<>` in `ApiServicesConfiguration.cs`
- **Clock = .NET `TimeProvider`** (singleton `TimeProvider.System`); never `DateTime.Now` in services. Server "now" is `GetLocalNow().DateTime`; zone-aware "now" from coordinates or an IANA id comes from `TimeProviderExtensions.GetLocalNow(...)` in `Covenant.Common/Utils/Extensions`. Tests use `FakeTimeProvider` (`Microsoft.Extensions.TimeProvider.Testing`); a test class whose tests each set their own "now" uses `Covenant.Integration.Tests/Configuration/TestClock.cs` instead, because `FakeTimeProvider` throws when the clock moves backwards. Integration tests never depend on execution order (there is no `TestOrder` attribute): each test seeds or creates what it needs and asserts by id, never by global counts.
- **`CovenantContext` runs with `QuerySplittingBehavior.SplitQuery` globally** (`Program.cs` + `TestStartupExtensions`), so never add `AsSplitQuery()` per query. Because split queries re-run the base query per collection, **every query that reaches `ToPaginatedList`/`Skip`/`Take` must be fully ordered on every path**: `ApplySort*` switches need a `default`, and list queries with collection projections add `.ThenBy(x => x.Id)`. Subquery `Take(1)` also needs an `OrderBy`.
- Repository pattern with interfaces in `Covenant.Common`, implementations in `Covenant.Infrastructure`. **Every interface extends `IBaseRepository` and every implementation inherits `BaseRepository<TContext>`** (`CovenantContext`; `IdentityContext` for `IdentityRepository`), which provides generic `Create`/`CreateRange` (async), `Update`/`UpdateRange`, `Delete`/`DeleteRange` (void, EF change tracker) and `SaveChangesAsync`; the DbContext is the protected `Context` property. Never re-declare those in a repository: interfaces only add queries and methods with logic (upserts, bulk imports, cascading deletes). Pass a single entity to `Create`/`Update`/`Delete` and a collection to the `*Range` variant — a collection passed to the generic single-entity method compiles (`T` becomes the list type) and throws at runtime.
- Services in `Covenant.Core.BL` depend only on repository interfaces (identity services also use ASP.NET Identity's `UserManager`/`RoleManager`)
- **Controllers never write.** Every `Create`/`Update`/`Delete`/`SaveChangesAsync` (and any repository method that persists) runs inside a Core.BL service; controllers only validate input (`ModelState`, request validators), call one service and map the `Result`. Reads may still call repository query methods that return DTOs. For a 404, the service returns `Result.Fail(ResultError.NotFound(msg))` and the controller does `result.IsNotFound ? NotFound() : BadRequest(ModelState.AddErrors(result.Errors))`.
- **Entities change state through their own methods.** No entity under `Covenant.Common.Entities` has a public setter (enforced by `Covenant.Tests/Architecture/EntityEncapsulationTest.cs`). When a service needs a new mutation, add an intention-revealing method on the entity (`CompanyUser.UpdateDetails`, `CompanyProfileContactPerson.Update`, `Agency.UpdateProfile/UpdateLogo/ReplaceWsibGroups`, `CompanyProfile.UpdatePhones/UpdatePaymentTermDays`, `TimeSheet.UpdateComment`, `Request.MarkInvitationSent`). EF Core maps private setters without configuration changes; confirm with `dotnet ef migrations has-pending-model-changes`. A property an entity deserializes from Service Bus JSON keeps `[JsonInclude]` on its private setter (`RequestApplicant.Status`). Use `private set` by default and `internal set` when another entity of the aggregate writes it (e.g. `WorkerProfile` creating its `WorkerProfileLicense`) or a test seeds it: `Covenant.Common` declares `InternalsVisibleTo` only for `Covenant.Tests` and `Covenant.Integration.Tests`, so Core.BL, Infrastructure and the Api still cannot write those properties. Aggregates that are built from a DTO expose a factory on the entity (`WorkerProfile.Register(model, agency, user)`); the adapter delegates to it. Tests prefer `FakeData` builders (`FakeData.FakeContactPerson`) or the `.With(e => e.Method(...))` helper in each test project's `Utils/EntityTestExtensions.cs`. Computed records are built through entity factories that receive the already calculated values (`PayStub.Create`, `Invoice.Create` computes only `TotalNet = SubTotal + Tax`, `InvoiceTotal.Create` + `Charge*`, `ReportSubcontractor.Create`); payroll and invoice math stays in the services. `ICatalog<T>` is read-only (`Id`/`Value` get-only); DTOs that implement it keep their setters. An entity sent through Service Bus (`RequestApplicant`) needs `[JsonInclude]` on every private setter it carries, guarded by `Covenant.Tests/Requests/RequestApplicantSerializationTest.cs`.
- EF Core configurations in `Covenant.Infrastructure/Configurations/`; one `IEntityTypeConfiguration` per entity and file
- **File endpoints declare `[Produces("application/octet-stream", "application/json")]`**, never octet-stream alone: with a single non-JSON content type no formatter can write `BadRequest(...)`/`ProblemDetails`, so every error path answers 406 with an empty body instead of 400 (guarded by `Covenant.Integration.Tests/Agencies/Accounting/ReportsControllerTest.cs`).
- **Every API controller carries `[ApiController]`** — the built-in OpenAPI generator (`Microsoft.AspNetCore.OpenApi`) only describes controllers that have it; the identity controllers use `[ApiExplorerSettings(IgnoreApi = true)]` to stay out of the document.
- Sigook.Functions talks to the API over HTTP with a client-credentials token from `/connect/token`; shared DTOs/enums come from `Covenant.Common`. `PrefixKeyVaultSecretManager` is duplicated on purpose (Api + Functions) so the worker does not reference `Covenant.Infrastructure`.

## Domain Gotchas

- **`RequestStatus` enum has only 3 values:** `Open = 1`, `Filled = 3`, `Cancelled = 4` (value `2` intentionally skipped). `Status` is the single source of truth — there is no `IsOpen` flag. Transitions happen automatically inside `Request.AddWorker` / `RejectWorker` / `Cancel` / `Open`; never set `Status` directly.
- **Cancellation rule:** `Request.Cancel()` only succeeds when `Status == Open` AND `WorkersQuantityWorking == 0`. To cancel a request with assignees, reject every worker first.
- **Controller routes** use the pattern `public const string RouteName = "api/..."` + `[Route(RouteName)]` (no `[controller]` tokens). URL scheme: `api/agency/{recruiting|sales|accounting|profile}/{feature}`, `api/company/{feature}`, `api/worker/{feature}`, shared `api/{feature}`, jobs `api/jobs/{job}` — lowercase class prefix, action templates unchanged. To locate an endpoint, grep for `RouteName =`; integration tests reference the constant, never a literal URL.
- **Moving entities or EF configurations must not create a migration**: run `dotnet ef migrations has-pending-model-changes` for both contexts (see DEVELOPMENT_COMMANDS.md) and expect "No changes".
- **Deductions are DB table lookups, not formulas.** `TimesheetCalculatorService.CalculateDeductions` → `DeductionsRepository` range lookups by earnings/year over **two** tables: `CppDeductions` and `TaxDeductions`, both discriminated by a `PayPeriod` enum (`Weekly`/`BiWeekly`/`SemiMonthly`/`Monthly`), and `TaxDeductions` additionally by `TaxType` (`Federal`/`Provincial`). The old 12 per-period tables are gone. EI is the only computed one (`totalEarnings × rates.EmploymentInsurance`, no cap). There are no `CppCalculator`-style classes. Per-worker `WorkerProfileTaxCategory` overrides zero out deductions (subcontractors).
- **Night shift is deprecated.** Never computed: `PayStubService` hardcodes `nightShift: 0`; invoices never compute it and the `Invoice` entity has no night-shift rate column. Don't add night-shift logic.
- **Holiday asymmetry invoice vs pay stub:** invoices hardcode `holidayIsPaid: true` (worked holidays always billed at holiday rate); pay stubs honor the timesheet's `HolidayIsPaid` flag. Worked vs not-worked holidays are two separate flows in both.
- **Invoices do NOT bill vacations or bonus** — the `Invoice` entity has no vacation/bonus columns and the totals never include them. Vacation 4% is a pay-stub concept. HST is a single global config rate (`rates.Hst`), not per-province. One `Invoice` entity serves Canada and the USA; country differences (prefix, tax, unworked holidays) live only in `IInvoiceCountryStrategy`, resolved from the agency billing location.
- **Messaging is custom Azure Service Bus** (`SigookBusClient` + `SigookBackgroundService` + consumers in `Covenant.Api/Consumers/`). There is no MassTransit. Locally the app connects to the staging Service Bus — to send, inject `IMessagePublisher` (`Covenant.Common/Interfaces/`, mockable); `ISigookBusClient` only creates processors for the consumers.
- **Layer rule:** `Covenant.Core.BL` never references `Covenant.Infrastructure` (`Api → Core.BL → Documents → Common`, `Api → Infrastructure → Common`). An interface Core.BL needs and Infrastructure implements goes in `Covenant.Common/Interfaces/`. Core.BL has no `Microsoft.AspNetCore.App` reference either: no `HttpContext`/`IFormFile` in services — read the request through `ICurrentUserService` / `IUploadedFilesService` (both implemented in Infrastructure), pass uploads as `Stream`. Enforced by `Covenant.Tests/Architecture/LayerDependencyTest.cs` (NetArchTest + Mono.Cecil): Core.BL and Common dependency rules, and no `IBaseRepository` write call (`Create`/`Update`/`Delete`/`*Range`/`SaveChangesAsync`) from `Covenant.Api.Controllers` or `Covenant.Api.Consumers`. `ToPaginatedList` lives in `Covenant.Infrastructure/Extensions` because it needs EF Core; Common never references EF Core.
- **Integration tests run on real Postgres** (Testcontainers, `Covenant.Integration.Tests/Configuration/PostgresTestDatabase.cs`): one container per test assembly, a `covenant_template` database built with `EnsureCreated()` + the SQL scripts in `Covenant.Infrastructure/Scripts/`, and one cloned database per test class. There is **no `InitialCreate` migration**, so `Database.Migrate()` cannot build the schema from scratch. Seed data comes from the builders in `Covenant.Integration.Tests/Utils/FakeData.cs` — extend those instead of hand-rolling entity graphs, since Postgres enforces every FK, unique index and NOT NULL that EF InMemory ignored. Compare dates with `DateAssert.Equal` (Postgres stores microseconds, .NET ticks are 100 ns). `AddDefaultTestConfiguration` registers a happy-path `IUserAdministrationService` (`Configuration/UserAdministrationMock.cs`); tests that need the real `IUserAccountService` register `UserAccountService` on top of it, and override the mock per test with `builder.ConfigureTestServices` (plain `ConfigureServices` runs before the test `Startup`, so the default wins).
- **Roles:** exactly 7, lowercase, in `Covenant.Common/Constants/CovenantConstants.cs` — `superadmin, admin, recruiting, sales, company, company.user, worker`. Seeded at startup by `SigookBackgroundService`. Reference via `CovenantConstants.Role.*`, never string literals.

## Identity Gotchas

- **Two databases, two contexts.** `CovenantContext` (`ConnectionStrings:DefaultConnection`) and `IdentityContext` (`ConnectionStrings:IdentityConnection`, the former IdentityServer database). Both apply configurations from the same assembly filtered by namespace (`IdentityContext.ConfigurationsNamespace`); put identity configurations under `Configurations/Identity/` only.
- **`IdentityContext` must not call `base.OnModelCreating`**: the identity tables keep their historical shape (`User`, `Rol`, `UserLogin`/`UserToken` keyed by `UserId` only, no `AspNet*` indexes). It also `Ignore`s `IdentityUserPasskey` (.NET 10). Migrations: `dotnet ef migrations add X -p Covenant.Infrastructure -s Covenant.Api -c IdentityContext -o Migrations/Identity`.
- **`Npgsql.EnableLegacyTimestampBehavior` stays after `builder.Build()`** in `Program.cs`. `dotnet ef` and the build-time OpenAPI tool only run `Program` up to the host build, so moving it earlier flips the design-time column type of every `DateTime` (`timestamp with` ↔ `without time zone`) and generates bogus migrations. The flip side: the runtime model never matches the snapshot exactly, so every Npgsql context ignores `RelationalEventId.PendingModelChangesWarning` (EF Core 9+ turns it into an exception inside `Migrate()`). Keep that `ConfigureWarnings` when registering a context.
- **Error codes for the password grant are a contract**: private constants at the top of `Controllers/Identity/AuthorizationController.cs` (`invalid_credentials`, `inactive_user`, `email_not_confirmed`, `locked_out`) — Sigook.App and Sigook.Web switch on them. Same for the userinfo `role` shape (string for one role, array for several).
- **Account-activation links point to Sigook.Web** (`{WebClientUrl}/confirm-email|create-password?token=&id=`, built in `AccountNotificationService`). The API serves no HTML: don't add Razor pages back; the old `GET /Account/*` links only redirect.
- **Access tokens are unencrypted JWTs** (`DisableAccessTokenEncryption`): Sigook.App decodes them. Keep it that way.
- **Certificates outside Development** are loaded lazily from Key Vault when OpenIddict builds its options (`Identity:SigningCertificateName` / `EncryptionCertificateName`), not at host build, so `dotnet build` (OpenAPI generation) and `dotnet ef` never touch Key Vault certificates.
- **Default auth scheme is the OpenIddict validation (bearer) scheme.** The Identity application cookie backs only the Microsoft 365 sign-in and `/connect/authorize` (`LoginPath = /External/Challenge`); never make it the default or API 401s turn into login redirects.
- **`sub` vs `NameIdentifier`**: read the caller id through `PrincipalExtensions.GetSubject()`/`TryGetUserId()`; OpenIddict identities carry `sub` and the test authentication handlers carry `ClaimTypes.NameIdentifier`.
- **Two Microsoft 365 app registrations.** `Identity:Microsoft365ClientId/ClientSecret` (Key Vault `{env}-api--Identity--Microsoft365Client*`) is the login + Graph `accountEnabled` registration (redirect URIs `{IssuerUri}/signin-oidc`, `User.Read.All`); `Microsoft365Configuration:ClientId/ClientSecret` only sends email through Graph. Don't merge them.
- **Sigook.Functions client**: seeded from `Identity:FunctionsClientId` / `FunctionsClientSecret` (Key Vault `{env}-api--Identity--FunctionsClient*`) on every startup; the Functions side reads the same values as `ScheduleTasks:ClientId/ClientSecret` under `{env}-func`.
- The IdentityServer4 tables (`Clients`, `PersistedGrants`, the old `DataProtectionKeys`, …) are gone: `AddOpenIddict` copied the public clients and scopes from them and `DropIdentityServer4Tables` removed them together with their `__EFMigrationsHistory` rows. Data protection keys live in the API database (`MyKeysContext` on `DefaultConnection`).

## Commands

```bash
# Build everything
dotnet build Covenant.Api.slnx

# Run the API (also serves /connect/*, /Account/*, /Password/*, /scalar)
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
