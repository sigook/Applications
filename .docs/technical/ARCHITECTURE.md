# Technical Architecture - Covenant/Sigook Platform

Monorepo with seven applications. Each has its own `CLAUDE.md` with app-specific conventions.

| Application | Stack | Purpose |
|---|---|---|
| `Covenant.Api/` | .NET 8 Web API | Backend for the whole platform |
| `Covenant.IdentityServer/` | .NET 6 + IdentityServer4 4.1.2 | OIDC/OAuth2 authentication server |
| `Sigook.Web/` | Vue 3 + Pinia + buefy-next | Agency web portal (main platform) |
| `Covenant.Web/` | Vue 3 + Vuetify | Public marketing website |
| `SigookApp/` | Flutter | Worker mobile app |
| `Sigook.Functions/` | Azure Functions v4 (.NET 8 isolated) | Scheduled background triggers |
| `Sigook.CognitiveServices/` | .NET 8 | AI/speech services (Azure Cognitive) |

---

## Tech Stacks

### Covenant.Api (.NET 8)

```
Framework:  ASP.NET Core 8.0 Web API
Database:   PostgreSQL (cloud-hosted), EF Core 8.0.11 + Npgsql 8.0.10
Patterns:   Repository, Service Layer, MediatR (document generation)
Packages:   MediatR 12.4.1, FluentValidation 11.10.0,
            Swashbuckle 7.2.0, Azure.Messaging.ServiceBus 7.18.2,
            Azure.Storage.Blobs 12.24.0, ClosedXML 0.104.2, PdfPig 0.1.15
Logging:    Microsoft.Extensions.Logging (Console/Debug) + Application Insights (no Serilog)
Config:     Azure Key Vault via PrefixKeyVaultSecretManager (also used by Sigook.Functions)
Cloud:      Azure App Service, Azure Service Bus, Azure Blob Storage, Azure Container Registry
```

### Covenant.IdentityServer (.NET 6)

```
Framework: ASP.NET Core 6.0 + IdentityServer4 4.1.2 (+ AspNetIdentity, EntityFramework stores)
ORM:       EF Core 6.0.13 + Npgsql 6.0.8
Protocols: OpenID Connect, OAuth 2.0
Staging:   https://sigook-accounts-staging.azurewebsites.net
Prod:      https://sigook-accounts.azurewebsites.net
```

Pinned to .NET 6 — upgrading to .NET 8 deadlocks IdentityServer4 + AutoMapper. It does **not**
reference `Covenant.Common` (see [Covenant.Common sharing](#covenantcommon-sharing)).

### Sigook.Web (Vue 3, agency portal)

```
Framework:  Vue 3.5.x + Vite 8 (Node 20+), TypeScript 6.x
State:      Pinia 3.x (+ pinia-plugin-persistedstate)
UI:         @ntohq/buefy-next 0.2.x (Buefy port for Vue 3 / Bulma)
Auth:       oidc-client-ts 3.x
HTTP:       Axios 1.10.0 (API calls live in src/api/*.ts)
Validation: VeeValidate 4.x + Yup 1.x; i18n: vue-i18n
Deploy:     Docker (Node build → Nginx), output in wwwroot/
Staging:    https://sigook-web-staging.azurewebsites.net
Prod:       https://sigook.azurewebsites.net
```

### Covenant.Web (Vue 3, marketing site)

```
Framework:  Vue 3.5.22 + Vite 7.1.11, TypeScript 6.x (^6.0.3)
State/UI:   Pinia 3.0.3, Vuetify 3.7.0, VeeValidate 4.15.1 + Yup 1.7.1
Hosting:    Azure Static Web Apps (staging: lively-island-020c8260f.7.azurestaticapps.net,
            prod: https://www.covenantgroupl.com)
```

Both web apps use **pnpm** as their package manager (`packageManager: pnpm@11.7.0`, via
corepack). Covenant.Web builds to `dist/` (Sigook.Web builds to `wwwroot/`).

### SigookApp (Flutter)

```
Framework:  Flutter (Dart ^3.9.2), Clean Architecture per feature (domain/data/presentation)
State:      Riverpod (flutter_riverpod ^3.0.3); DI: Riverpod + get_it ^9.0.5
Routing:    GoRouter ^17.0.0 (lib/core/routing/app_router.dart)
HTTP:       Dio ^5.7.0 (lib/core/network/api_client.dart + auth_interceptor.dart: bearer
            injection, 401 refresh + retry)
Auth:       flutter_appauth ^11.0.0; tokens in FlutterSecureStorage
Codegen:    build_runner, freezed ^3.2.3, json_serializable; Dartz for Either/Option
Flavors:    staging / production (main_staging.dart / main_production.dart, .env.* configs)
```

Each feature under `lib/features/{name}/` has `domain/` (entities, repositories interfaces,
usecases — pure Dart), `data/` (Freezed models, local/remote datasources, repository impls),
`presentation/` (pages, widgets, Riverpod viewmodels/providers).

### Sigook.Functions (.NET 8, isolated worker)

`Sigook.Functions/Sigook.Functions/Functions/ScheduleTasks.cs` holds two timer triggers
(`0 0 0 * * 1-5`): `NotificationSinExpiration` and `WarnLicensesExpiration`.
`Functions/CraTables.cs` holds a blob trigger, `CraTableUploaded`, on the `cra-tables` container
(`CraTablesStorage` connection): it reads the table, the pay period and the year from the blob name
(`Utils/CraBlobName.cs`) and asks the API to import the CRA CPP or income tax table.

Every function gets a client-credentials token and POSTs to Covenant.Api (`ScheduleTasks:ApiUrl`,
`CraTables:CppApiUrl`, `CraTables:TaxApiUrl`), reporting the outcome to Teams. Email/invitation sending does **not** live
here — that moved to Service Bus consumers inside Covenant.Api
(see [Async messaging](#async-messaging-azure-service-bus)).

Unit tests live in `Sigook.Functions/Sigook.Functions.Tests` and run in the pipeline.

The blob-trigger connection is resolved by the Functions **host**, not by the isolated worker's
`IConfiguration`, so it can never come from Key Vault through `ConfigureAppConfiguration`. It is
declared as an **identity-based connection** instead of a connection string, which keeps secrets out
of the repo: `CraTablesStorage__blobServiceUri` + `CraTablesStorage__queueServiceUri` (the polling
trigger needs the queue endpoint too, for its internal scan queues).

- Locally it authenticates with `az login`; in Azure, with the Function App's managed identity.
- Whoever runs it needs **Storage Blob Data Contributor** and **Storage Queue Data Contributor** on
  the storage account.
- Local settings point at `sigookfilesstaging`; production points at `sigookfiles` through the same
  two App Settings on `sigook-functions`.

### Sigook.CognitiveServices (.NET 8)

Three projects (`.Core`, `.Infraestructure`, `.UI`); key package Microsoft.CognitiveServices.Speech.
Runs on an F1 App Service plan (intentional — do not suggest upgrading).

---

## Covenant.Api Solution

### Projects

```
Covenant.Api/
├── Covenant.Api/               # Web API: controllers, auth filters, background service, DI
├── Covenant.Common/            # Entities, enums, models/DTOs, repository+service interfaces
├── Covenant.Core.BL/           # Services (business logic) + Service Bus consumers
├── Covenant.Infrastructure/    # EF Core (CovenantContext), repositories, integrations, deductions
├── Covenant.Documents/         # Excel/PDF report generators (MediatR handlers)
├── Covenant.Tests/             # Unit tests
└── Covenant.Integration.Tests/ # Integration tests
```

### Covenant.Common sharing

- `Covenant.Common.csproj` has `IsPackable=False`; it is consumed **only by `ProjectReference`
  inside the Covenant.Api solution**.
- `Covenant.IdentityServer` does NOT reference it (no package, no project reference). It vendors
  the few types it needs under `Covenant.IdentityServer/Covenant.IdentityServer/Entities/`
  (`CovenantUser`, `CovenantRole`, `InactiveUser`) and `Enums/` (`UserType`, `EmailSettingName`).
  Keep these in sync with `Covenant.Common` by hand — the API exchanges DTOs with IdentityServer
  over HTTP.
- `Sigook.Functions` doesn't reference it either; it only calls the API over HTTP.
- **Declare what you use**: every project lists a `PackageReference` for each package its own code
  compiles against, even when a referenced project already brings it in transitively. Duplicated
  declarations cost nothing (NuGet resolves one package, one version) and keep a lower layer from
  breaking the ones above it when it drops a dependency. Conversely, `Covenant.Common` only carries
  what it actually compiles against (CsvHelper, FluentValidation, libphonenumber, ASP.NET Identity
  EF) — don't add packages there just because a downstream project needs them.
- **Versions are centralized** in `Covenant.Api/Directory.Packages.props`
  (`ManagePackageVersionsCentrally`). `PackageReference` entries carry no `Version` — add or bump
  the `PackageVersion` in that file so every project resolves the same version.
- Keep infrastructure concerns out of `Covenant.Common`: Excel helpers live in `Covenant.Documents`,
  ASP.NET-bound helpers (`IFormFile`, `IConfiguration`, Razor helpers) in `Covenant.Api`/
  `Covenant.Core.BL`, and test-only helpers inside each test project.

### Controllers (presentation layer)

Two coexisting layouts:

**1. Route-first controllers** under `Covenant.Api/Controllers/`:

| Path | Controllers |
|---|---|
| `Controllers/Sigook/` | `CatalogController`, `LocationController`, `FileController` |
| `Controllers/Sigook/Agency/` | `AgencyController`, `AgencyLocationController`, `NotificationsController` |
| `Controllers/Sigook/Agency/Accounting/` | `InvoicesController`, `PayStubsController`, `ReportsController`, `LocationTaxController`, `DeductionsController` |
| `Controllers/Sigook/Agency/CompanyProfiles/` | company detail: profile, contacts, documents, invoice notes/recipients, job positions, locations, logo, notes, users |
| `Controllers/Sigook/Agency/Requests/` | request detail: `RequestsController`, `ApplicantsController`, `RunnersController`, `WorkersController`, `TimeSheetsController`, `WorkerTimeSheetsController`, `WorkerNotesController` (per-worker notes on a request), notes, shift, skills, report-to, requested-by |
| `Controllers/Sigook/Agency/Recruiting/` | recruiting-scoped lists: `RequestsController`, `CompanyProfilesController`, `WeeklyBoardController` |
| `Controllers/Sigook/Agency/Sales/` | sales-scoped lists: `RequestsController`, `CompanyProfilesController` |
| `Controllers/Sigook/Agency/Candidates/` | candidate domain: `CandidatesController`, `NotesController`, `PhoneNumbersController`, `SkillsController`, `DocumentsController` |
| `Controllers/Sigook/Agency/Workers/` | worker-profile management: `WorkersController`, `NotesController`, `CommentsController`, `HolidaysController`, `RequestHistoryController` |
| `Controllers/Sigook/Agency/Personnel/` | `PersonnelController` (agency back-office users), `AgenciesController` (agencies the caller belongs to) |
| `Controllers/WebSite/` | `WebSiteController` (public marketing endpoints) |
| `Controllers/Jobs/` | `ScheduleTasksController` (called by Sigook.Functions timers) |

**2. Module folders** under `Covenant.Api/{Module}Module/{Resource}/Controllers/`:

| Module | Contents |
|---|---|
| `CompanyModule/` | company perspective: requests, profile, locations, job positions, request workers, timesheets, invoices, users |
| `WorkerModule/` | worker perspective: profile, requests, request history, timesheets (clock in/out) |

Routing: older controllers declare `public const string RouteName = "api/..."` +
`[Route(RouteName)]` (grep for `RouteName =` to find an endpoint); newer ones use attribute
literals like `[Route("api/agency/accounting/[controller]")]` or
`[Route("api/agency/sales/[controller]")]`. There is no `{Module}{Resource}V{N}Controller`
convention in production code (the single legacy `V2` name,
`V2CompanyRequestWorkerTimeSheetController.cs`, is historical, not a system).

### Services (business logic) — `Covenant.Core.BL/Services/`

Root: `AgencyService`, `CandidateService`, `CompanyService`, `LocationService`,
`NotificationService`, `RequestService`, `RunnerService`, `SalesService`, `TimeSheetService`,
`WeeklyBoardService`, `WorkerService`. Watch the file/type mismatch: the file
`TimeSheetService.cs` holds the class `TimesheetService : ITimesheetService`, while the
interface file is `ITimeSheetService.cs` — a real grep trap.

Everything billing/payroll lives under `Services/Accounting/`:

- `AccountingService`, `PayStubService`, `DeductionImportService` (CRA PDF import).
- `Services/Accounting/Invoices/` — `InvoiceService` (abstract base), `CanadaInvoiceService`,
  `UsaInvoiceService`, `InvoiceServiceFactory` (resolves the country service from the agency
  billing location).
- `Services/Accounting/Shared/` — `TimesheetCalculatorService` (hours breakdown + payroll
  deductions), shared by payroll and invoicing.

Also in `Covenant.Core.BL/`: `Adapters/` (entity→model adapters for candidate, company, worker,
interfaces in `Covenant.Common/Interfaces/Adapters/`), `Extensions/Accounting/` (Razor view-model
extensions for the invoice and payroll templates) and `Consumers/` (Service Bus).

Services depend only on repository interfaces from `Covenant.Common/Repositories/`; controllers
only delegate to services. DI registration (services/repositories/adapters/containers
`AddScoped`; Service Bus clients + consumers and config option objects (`Rates`, `TimeLimits`,
`AzureStorageConfiguration`) `AddSingleton`) lives in
`Covenant.Api/Configuration/ApiServicesConfiguration.cs`.

### Infrastructure — `Covenant.Infrastructure/`

```
Contexts/         CovenantContext.cs (main DbContext), MyKeysContext (DataProtection keys),
                  PostgresFunctions.cs (EF DbFunction mappings backing
                  Scripts/Functions/get_week_start_sunday.sql)
Repositories/     by domain: Accounting/, Agency/, Candidate/, Company/, Notification/,
                  Request/, Worker/ + root repositories (Catalog, Location, Shift, User);
                  shared plumbing in BaseRepository.cs
Mappers/          entity→model projection extensions used inside repositories:
                  AgencyExtensionsMapping, CandidateExtensionsMapping, CompanyExtensionsMapping,
                  RequestExtensionsMapping, WorkerRequestExtensionsMapping
Configurations/   EF Core IEntityTypeConfiguration classes, mirrored by domain
Migrations/       EF Core migrations
Scripts/          raw SQL (views, functions, stored procedures) run at startup
Services/         integrations: EmailService + SendGridService (SendGrid), GeocodeService
                  (Google Maps), PushNotifications (Azure Notification Hub), TeamsService
                  (webhooks), DocumentService, PdfGeneratorService, RazorViewToStringRenderer,
                  IdentityServerService, TimeService, CraPdfParser (PdfPig reader for the CRA
                  deduction tables), SigookBusClient / SigookBusAdministrationClient (Service Bus)
Services/Storage/ Azure Blob containers, one class per container (see below)
Services/Handlers/ Microsoft365TokenHandler (DelegatingHandler: Entra client-credentials bearer
                  token, attached to the IdentityServer HttpClient in AddClients)
```

**Blob storage** — one typed container class per Azure Blob container, all deriving from
`BaseAzureStorage`, with interfaces in `Covenant.Common/Interfaces/Storage/`:
`InvoicesContainer` (invoice PDFs), `PayStubsContainer` (pay stub PDFs), `FilesContainer`
(worker/company documents), `CraTablesContainer` (CRA deduction PDFs). Registered in
`ApiServicesConfiguration.AddContainers` against two connection strings —
`AccountingStorageConnection` (invoices, pay stubs) and `FileStorageConnection` (files, CRA
tables). Invoice and pay stub PDFs are **cached** in their container: delete the blob to force a
regeneration after fixing a Razor template.

Payroll deductions are **DB table lookups, not formulas**: `TimesheetCalculatorService` →
`DeductionsRepository` range lookups by earnings/year. EI is the only computed deduction.
Import flow: a CRA PDF is uploaded to the `cra-tables` blob container → the Azure Function
`CraTableUploaded` (blob trigger, `Sigook.Functions/Functions/CraTables.cs`) calls the API's
`DeductionsController` (`POST api/Accounting/Deduction/Cpp/Blob` / `Tax/Blob`, which takes a
blob reference) → `DeductionImportService` (uses `CraPdfParser` + `CraTablesContainer`) →
`DeductionsRepository.ImportCpp/ImportTax(year, payPeriod, rows, yearsKept)`, keeping the last
2 years. Reads happen only through `TimesheetCalculatorService`; there is no endpoint to read
the tables back.

**Health checks** — `AddCovenantHealthChecks` + `Covenant.Api/HealthChecks/` (tagged
`config`/`ready`/`live`) probe the four connection strings: `DefaultConnection`,
`AccountingStorageConnection`, `FileStorageConnection`, `ServiceBusConnection`.

For entities, enums, and the data model, see
[ENTITIES_RELATIONSHIPS.md](ENTITIES_RELATIONSHIPS.md). For the Request lifecycle rule
(`Status` is the single source of truth, no `IsOpen` flag), see
[.docs/business/REQUEST_STATE_MANAGEMENT.md](../business/REQUEST_STATE_MANAGEMENT.md).

---

## Async messaging (Azure Service Bus)

Custom implementation on `Azure.Messaging.ServiceBus` — **there is no MassTransit**.

| Piece | File |
|---|---|
| Client (send + create processors) | `Covenant.Infrastructure/Services/SigookBusClient.cs` (+ `ISigookBusClient.cs`) |
| Admin client (create queues/topics/subscriptions/rules) | `Covenant.Infrastructure/Services/SigookBusAdministrationClient.cs` |
| Consumer contract | `Covenant.Common/Interfaces/IAzureServiceBusConsumer.cs` (`Task OnInit()`) |
| Bootstrap | `Covenant.Api/BackgroundServices/SigookBackgroundService.cs` |
| Queue/topic names | `Covenant.Common/Configuration/ServiceBusConfiguration.cs` |

`SigookBackgroundService` (a `BackgroundService` registered in `Program.cs`) runs at startup:
applies pending EF migrations for both contexts, executes the raw SQL in
`Covenant.Infrastructure/Scripts/`, creates queues/topics/subscriptions if missing
(`ValidateCandidateQueue`, `BulkPayStubEmailQueue`, `InvitationQueue`, `CreateApplicantTopic`
with Teams/Email/RequestApplicant subscriptions), then calls `OnInit()` on every registered
consumer.

Consumers live in `Covenant.Core.BL/Consumers/`; exactly **5** are registered as
`IAzureServiceBusConsumer` singletons in `AddAzureServiceBusConsumer`
(`ApiServicesConfiguration.cs`): `NewCandidateConsumer`, `TeamsConsumer`,
`RequestApplicantConsumer`, `BulkPayStubEmailConsumer`, `InvitationConsumer`.
(A stillborn `EmailConsumer` and its `EmailNotification` topic subscription were removed in
2026-08; the orphaned subscription must also be deleted by hand in the Azure portal for staging
and production, since removing the `CreateSubscriptionIfNotExistsAsync` call does not delete an
existing subscription.)

Local development connects to the **staging** Service Bus — inject `ISigookBusClient` (mockable)
rather than constructing clients directly.

---

## Authentication & Authorization

- All apps authenticate against Covenant.IdentityServer via OIDC: Sigook.Web with
  `oidc-client-ts` (`src/security/`), SigookApp with `flutter_appauth` (tokens in secure
  storage). Covenant.Api validates the JWT Bearer token on every request.
- **Roles** (exactly 7, lowercase, defined in `Covenant.Common/Constants/CovenantConstants.cs`):
  `superadmin`, `admin`, `recruiting`, `sales`, `company`, `company.user`, `worker`. Role groups:
  `RecruitingAccess` (superadmin/admin/recruiting), `SalesAccess` (superadmin/admin/sales),
  `AgencyStaff` (recruiting access + sales), `AdminAccess` (superadmin/admin),
  `AgencyAssignable` (admin/recruiting/sales), `SuperAdminAssignable` (`CovenantConstants.cs`).
  Always reference via `CovenantConstants.Role.*`.
- **Policies** in `Covenant.Api/Authorization/PolicyConfiguration.cs`: `Agency`, `Recruiting`,
  `Company`, `Worker`, `Request` (authenticated only), `Covenant` (claim `all2job`),
  `AgencyOrCompany`, `AgencyOrWorker`, `Admin`, `SuperAdmin`, `Sales`. There is no `Accounting`
  policy. Note `AgencyOrCompany`/`AgencyOrWorker` are built from `RecruitingAccess`, so they
  exclude sales.

### Data isolation (multi-tenancy)

Claims are injected per-request by action filters in `Covenant.Api/Authorization/`:

- `AgencyIdFilter` — resolves the caller's `agencyId` (and all `agencyIds`) claim for agency
  staff; repositories filter with it.
- `AgencyPersonnelIdFilter` — resolves the caller's `agencyPersonnelId` claim (read via
  `GetAgencyPersonnelId()` in `Covenant.Common/Utils/Extensions/PrincipalExtensions.cs`).
- `CompanyIdFilter` — same idea for company users.

Scoping rules:

- **Agency staff** see only their agency's data (`AgencyId` filter in every agency repository).
- **Sales users** additionally see only their own portfolio: the sales controllers
  (`Controllers/Sigook/Agency/Sales/`) pass the personnel id down, and
  `Covenant.Infrastructure/Repositories/Company/CompanyRepository.cs` filters
  `CompanyProfile.SalesRepresentativeId == salesPersonnelId` (requests are filtered by the
  companies assigned to that rep in `RequestRepository`).
- **Company users** see only their company; **workers** see only their own profile/requests.

---

## Deployment

| | Staging | Production |
|---|---|---|
| Branch | `dev` (auto-deploy on push) | `main` (manual deploy from Azure DevOps) |
| API | sigook-api-staging.azurewebsites.net | sigook-api.azurewebsites.net |
| Web | sigook-web-staging.azurewebsites.net | sigook.azurewebsites.net |
| Identity | sigook-accounts-staging.azurewebsites.net | sigook-accounts.azurewebsites.net |
| Marketing | lively-island-020c8260f.7.azurestaticapps.net | www.covenantgroupl.com |

Azure DevOps pipelines with path-based triggers and reusable templates
(`.azure-pipelines/templates/`); Docker builds for backend/Sigook.Web, static build for
Covenant.Web. Full details: [PIPELINES.md](PIPELINES.md).

---

## Database

PostgreSQL + EF Core 8. Migrations are applied **automatically at API startup** by
`SigookBackgroundService`; to add one:

```bash
cd Covenant.Api
dotnet ef migrations add MigrationName --project Covenant.Infrastructure --startup-project Covenant.Api
```

Conventions: PK `Id` (Guid) + human-facing sequential `NumberId` on major entities; FKs `{Entity}Id`;
`CreatedAt`/`UpdatedAt` timestamps; `CreatedBy`/`UpdatedBy` audit strings on newer entities.

Development commands: [DEVELOPMENT_COMMANDS.md](DEVELOPMENT_COMMANDS.md).
API endpoint reference: `openapi.json` (generated on build).
