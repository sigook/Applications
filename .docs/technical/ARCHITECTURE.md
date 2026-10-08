# Technical Architecture - Covenant/Sigook Platform

Monorepo with six applications. Each has its own `CLAUDE.md` with app-specific conventions.

| Application | Stack | Purpose |
|---|---|---|
| `Covenant.Api/` | .NET 10 Web API + OpenIddict 7 | Backend for the whole platform and OIDC/OAuth2 authorization server (one solution, `Covenant.Api.slnx`) |
| `Covenant.Api/Sigook.Functions/` | Azure Functions v4 (.NET 10 isolated) | Scheduled background triggers (same solution as the API) |
| `Sigook.Web/` | Vue 3 + Pinia + buefy 3 | Agency web portal (main platform) |
| `Covenant.Web/` | Vue 3 + Vuetify | Public marketing website |
| `SigookApp/` | Flutter | Worker mobile app |
| `Sigook.CognitiveServices/` | .NET 8 | AI/speech services (Azure Cognitive) |

---

## Tech Stacks

### Covenant.Api (.NET 10)

```
Framework:  ASP.NET Core 10.0 Web API + Razor views (email and PDF templates only)
Database:   PostgreSQL (cloud-hosted), EF Core 10.0.12 + Npgsql 10.0.3 — two databases:
            CovenantContext (API data) and IdentityContext (users, roles, OpenIddict stores)
Identity:   ASP.NET Core Identity (custom table names) + OpenIddict 7 server/validation
Patterns:   Repository, Service Layer, MediatR (document generation)
Packages:   MediatR 12.4.1, FluentValidation 11.10.0, OpenIddict 7.7.0,
            Microsoft.AspNetCore.OpenApi (document) + Scalar.AspNetCore (UI),
            Azure.Messaging.ServiceBus 7.18.2, Azure.Storage.Blobs 12.24.0,
            ClosedXML 0.104.2, PdfPig 0.1.15, SixLabors.ImageSharp 3.1.12 (default avatar)
Logging:    Microsoft.Extensions.Logging (Console/Debug) + Application Insights (no Serilog)
Config:     Azure Key Vault via PrefixKeyVaultSecretManager (prefix `{env}-api`)
Cloud:      Azure App Service, Azure Service Bus, Azure Blob Storage, Azure Container Registry
SDK:        10.0.401 (Covenant.Api/global.json); packages only from nuget.org (Covenant.Api/nuget.config)
```

The identity server is **inside the API process**: `accounts.sigook.ca` / `staging.accounts.sigook.ca`
are custom domains bound to the same App Service as `api.sigook.com`, and the OpenID Connect issuer is
`Identity:IssuerUri` (`appsettings.{Environment}.json`). See
[Authentication & Authorization](#authentication--authorization).

### Sigook.Web (Vue 3, agency portal)

```
Framework:  Vue 3.5.x + Vite 8 (Node 20+), TypeScript 6.x
State:      Pinia 3.x (+ pinia-plugin-persistedstate)
UI:         buefy 3.x (official Buefy for Vue 3, bundles Bulma 1.x)
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
Auth:       native email/password screen → OAuth2 password grant (POST {authority}/connect/token,
            form-urlencoded, no id_token — role via /connect/userinfo; scopes
            openid profile api1 offline_access — the android/ios clients do NOT grant
            `roles`, asking for it fails with invalid_scope); in-app 2-step forgot-password
            (/forgot-password → POST /Password/forgot + /Password/reset, 6-digit code,
            60-s resend cooldown); refresh handled by auth_interceptor against
            /connect/token; tokens in FlutterSecureStorage. The authority is the API host.
Codegen:    build_runner, freezed ^3.2.3, json_serializable; Dartz for Either/Option
Envs:       local / staging / production — one entry point each (main_local.dart,
            main_staging.dart, main_production.dart) over shared main_common.dart, with
            config from --dart-define-from-file=.env.<env>. Xcode flavors (staging /
            production schemes) exist on iOS only; Android declares no productFlavors
```

Each feature under `lib/features/{name}/` has `domain/` (entities, repositories interfaces,
usecases — pure Dart), `data/` (Freezed models, local/remote datasources, repository impls),
`presentation/` (pages, widgets, Riverpod viewmodels/providers).

### Sigook.Functions (.NET 10, isolated worker, `Microsoft.Azure.Functions.Worker` 2.x)

`Covenant.Api/Sigook.Functions/Functions/ScheduleTasks.cs` holds two timer triggers
(`0 0 0 * * 1-5`): `NotificationSinExpiration` and `WarnLicensesExpiration`.
`Functions/CraTables.cs` holds a blob trigger, `CraTableUploaded`, on the `cra-tables` container
(`CraTablesStorage` connection): it reads the table, the pay period and the year from the blob name
(`Utils/CraBlobName.cs`) and asks the API to import the CRA CPP or income tax table.

Every function gets a client-credentials token from the API's own token endpoint
(`ScheduleTasks:AccountsUrl`, client `Identity:FunctionsClientId` seeded by the API) and POSTs to
Covenant.Api (`ScheduleTasks:ApiUrl`, `CraTables:CppApiUrl`, `CraTables:TaxApiUrl`), reporting the
outcome to Teams. The project references only `Covenant.Common` (shared enums and models such as
`PayPeriod`, `ImportCraTableFromBlobModel`, `TeamsNotificationModel`); it has no database access.
Email/invitation sending does **not** live here — that moved to Service Bus consumers inside
Covenant.Api (see [Async messaging](#async-messaging-azure-service-bus)).

Unit tests live in `Covenant.Api/Sigook.Functions.Tests` and run in both the API and the Functions
pipelines.

The blob-trigger connection is resolved by the Functions **host**, not by the isolated worker's
`IConfiguration`, so it can never come from Key Vault. It is declared as an **identity-based
connection** instead of a connection string, which keeps secrets out of the repo:
`CraTablesStorage__blobServiceUri` + `CraTablesStorage__queueServiceUri` (the polling trigger needs
the queue endpoint too, for its internal scan queues).

- Locally it authenticates with `az login`; in Azure, with the Function App's managed identity.
- Whoever runs it needs **Storage Blob Data Contributor** and **Storage Queue Data Contributor** on
  the storage account.
- Local settings point at `sigookfilesstaging`; production points at `sigookfiles` through the same
  two App Settings on `sigook-functions`.
- Key Vault prefix for the worker's own settings: `{env}-func`.

### Sigook.CognitiveServices (.NET 8)

Three projects (`.Core`, `.Infraestructure`, `.UI`); key package Microsoft.CognitiveServices.Speech.
Runs on an F1 App Service plan (intentional — do not suggest upgrading).

---

## Covenant.Api Solution

### Projects

```
Covenant.Api/                       # Covenant.Api.slnx, Directory.Build.props (net10.0), Directory.Packages.props, global.json, nuget.config
├── Covenant.Api/                   # Web API + identity host: controllers, auth filters, Razor views, background service, DI
├── Covenant.Common/                # Entities (incl. Entities/Identity), enums, models/DTOs, repository+service interfaces, constants
├── Covenant.Core.BL/               # Services (business logic, incl. Services/Identity) + Service Bus consumers
├── Covenant.Infrastructure/        # EF Core (CovenantContext, IdentityContext), repositories, integrations, deductions
├── Covenant.Documents/             # Excel/PDF report generators (MediatR handlers)
├── Sigook.Functions/               # Azure Functions (timers + blob trigger), references Covenant.Common only
├── Covenant.Tests/                 # API unit tests
├── Covenant.Integration.Tests/     # API integration tests (Testcontainers Postgres)
└── Sigook.Functions.Tests/         # Functions unit tests
```

`Directory.Build.props` sets `TargetFramework=net10.0`, `ImplicitUsings`, `LangVersion=latest` and
`IsPackable=false` for every project; individual csproj files only carry what differs.

### Covenant.Common sharing

- `Covenant.Common` is consumed only by `ProjectReference` inside the solution — by the API layers
  and by `Sigook.Functions`.
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
  `Covenant.Core.BL`, and test-only helpers inside each test project. For that reason
  `PrefixKeyVaultSecretManager` exists twice on purpose (Api and Sigook.Functions), so the worker
  does not have to reference `Covenant.Infrastructure`.

### Controllers (presentation layer)

One layout under `Covenant.Api/Controllers/`, mirroring the Sigook.Web navigation:
**portal → menu → feature**. The folder is the route: `Controllers/Agency/Recruiting/Requests/`
serves `api/agency/recruiting/requests`, `Controllers/Company/Invoices/` serves
`api/company/invoices`, `Controllers/Worker/History/` serves `api/worker/history`. Class-level
prefixes are lowercase and without hyphens; action templates keep their own casing.

| Path | Route prefix | Controllers |
|---|---|---|
| `Controllers/Identity/` | `/connect/*`, `/Account`, `/Password`, `/External`, `/` | `AuthorizationController` (OpenIddict passthrough: `/connect/authorize`, `/connect/token`, `/connect/userinfo`, `/connect/endsession`), `AccountController` (`POST /Account/ConfirmEmail`, `/Account/CreatePassword`, `/Account/ResendConfirmationLink`; legacy email links redirect to Sigook.Web), `ExternalController` (Microsoft 365 sign-in), `PasswordController` (`POST /Password/forgot` + `/Password/reset`), `HomeController` (`/` and `/Home/InvalidUser`, both redirect to Sigook.Web). All excluded from the OpenAPI document |
| `Controllers/Shared/` | `api/catalog`, `api/location`, `api/file`, `api/emailpreferences`, `api/website` | any role or anonymous: `CatalogController`, `LocationController`, `FileController` (only the `defaultImage` placeholder — uploads are multipart on each domain endpoint), `EmailPreferencesController`, `WebsiteController` (public marketing endpoints) |
| `Controllers/Shared/Account/` | `api/account`, `api/usernotification` | the caller's own account, any role, bearer auth: `UserAccountController` (`POST api/account/ChangeEmail`, `GET api/account/GetEmail`, `PATCH /identity` to deactivate — kept because installed SigookApp builds call them), `UserNotificationController` |
| `Controllers/Shared/Jobs/` | `api/jobs/scheduletasks`, `api/jobs/deductions` | machine-to-machine, called by Sigook.Functions: `ScheduleTasksController`, `DeductionsController` (CRA table import) |
| `Controllers/Agency/` | `api/agency/notifications` | `NotificationsController` (sidebar bell) |
| `Controllers/Agency/Profile/` | `api/agency/profile[/locations\|personnel\|personnel/agencies\|attendance]` | the caller's own agency: `AgencyProfileController`, `LocationsController`, `PersonnelController`, `PersonnelAgenciesController` (agencies the caller belongs to), `AttendanceController` (staff clock in/out) |
| `Controllers/Agency/Recruiting/Requests/` | `api/agency/recruiting/requests[/{requestId}/…]` | `RequestsListController` (lists + Excel, Policy `Recruiting`), `RequestsController` (detail/CRUD), `ApplicantsController`, `RunnersController` (Policy `Recruiting`), `WorkersController`, `TimeSheetsController`, `WorkerTimeSheetsController`, `WorkerNotesController`, `WorkersReportController`, `NotesController`, `ShiftController`, `SkillsController`, `ReportToController`, `RequestedByController` |
| `Controllers/Agency/Recruiting/Applicants/` | `api/agency/recruiting/applicants` | `ApplicantsController` (cross-request applicants board) |
| `Controllers/Agency/Recruiting/WeeklyBoard/` | `api/agency/recruiting/weeklyboard` | `WeeklyBoardController` |
| `Controllers/Agency/Recruiting/Candidates/` | `api/agency/recruiting/candidates[/{candidateId}/…]` | `CandidatesController`, `NotesController`, `PhoneNumbersController`, `SkillsController`, `DocumentsController` |
| `Controllers/Agency/Recruiting/Workers/` | `api/agency/recruiting/workers[/{workerProfileId}/…]` | `WorkersController`, `NotesController`, `CommentsController`, `HolidaysController`, `RequestHistoryController`, `TimeSheetHistoryController`, `WageHistoryController` |
| `Controllers/Agency/Recruiting/Clients/` | `api/agency/recruiting/clients[/{profileId}/…]` | `ClientsListController` (lists + Excel, Policy `Recruiting`), `ClientsController` (CRUD, Policy `Agency`), `ContactInformationController`, `ContactPeopleController`, `DocumentsController`, `InvoiceNotesController`, `InvoiceRecipientsController`, `JobPositionsController`, `LocationsController`, `LogoController`, `NotesController`, `UsersController` |
| `Controllers/Agency/Sales/` | `api/agency/sales/{dashboard\|clients\|requests\|agencies}` | sales-rep scoped (Policy `Sales`): `DashboardController`, `ClientsController`, `RequestsController`; `AgenciesController` (sub-agencies, Policy `Agency`) |
| `Controllers/Agency/Sales/Clients/` | `api/agency/sales/clients/{profileId}/{deals\|interactions}` | owner-scoped `DealsController`, `InteractionsController` (Policy `Sales`) |
| `Controllers/Agency/Accounting/` | `api/agency/accounting/{invoices\|paystubs\|reports\|locations}` | `InvoicesController`, `PayStubsController`, `ReportsController` (Policy `Admin`), `LocationTaxController` |
| `Controllers/Company/Profile/` | `api/company/profile[/users\|contactpeople\|jobpositions\|locations]` | `CompanyController` (own profile), `UsersController`, `ContactPeopleController`, `JobPositionsController`, `LocationsController` |
| `Controllers/Company/Requests/` | `api/company/requests[/{requestId}/…]` | `RequestsController`, `ShiftController`, `WorkersController`, `WorkerTimeSheetsController` |
| `Controllers/Company/Invoices/` | `api/company/invoices` | `InvoicesController` |
| `Controllers/Company/Workers/` | `api/company/workers/{workerProfileId}/comments` | `CommentsController` |
| `Controllers/Worker/` | `api/worker/{profile\|requests\|history\|comments}`, `api/worker/requests/{requestId}/timesheet` | the worker's own endpoints (Policy `Worker`): `Profile/ProfileController` (+ `ProfileUpdateController`, `JobExperienceController`), `Requests/RequestsController`, `History/HistoryController`, `Comments/CommentsController`, `TimeSheets/TimeSheetsController` (clock in/out) |

**`Covenant.Api/WorkerModule/` is obsolete.** It holds the previous worker controllers
(`api/WorkerProfile`, `api/WorkerRequest`, `api/WorkerRequestHistory`) untouched and marked
`[Obsolete]`, because the SigookApp build in the stores still calls them. Delete the folder and
`Covenant.Integration.Tests/WorkerModule/` once the app that uses `api/worker/*` is published.
Never add code there.

Routing: every controller declares `public const string RouteName = "api/..."` +
`[Route(RouteName)]` (grep for `RouteName =` to find an endpoint; integration tests reference the
constant instead of literal URLs). Every API controller carries `[ApiController]`; the built-in
OpenAPI generator only describes controllers that have it.

### Services (business logic) — `Covenant.Core.BL/Services/`

Services are organized by **domain**, not by portal (one `RequestService` serves the agency,
company and worker controllers). Folder names are **plural** so the namespace never shadows the
entity (`Services.Requests` vs the `Request` entity); `Covenant.Core.BL/Interfaces/` mirrors the
same tree.

| Folder | Services |
|---|---|
| `Services/Agencies/` | `AgencyService` |
| `Services/Candidates/` | `CandidateService` |
| `Services/Companies/` | `CompanyService` |
| `Services/Locations/` | `LocationService` |
| `Services/Notifications/` | `NotificationService` |
| `Services/Requests/` | `RequestService`, `RequestApplicantService`, `RequestApplicantNotificationService`, `RunnerService`, `WeeklyBoardService`, `TimesheetService` |
| `Services/Sales/` | `SalesService` (deals, interactions, dashboard) |
| `Services/Workers/` | `WorkerService` |
| `Services/Shared/` | `UploadedFilesService` |
| `Services/Accounting/` | `AccountingService`, `TimesheetCalculatorService` (hours breakdown + payroll deductions, shared by payroll and invoicing) |
| `Services/Accounting/Deductions/` | `DeductionImportService` (CRA PDF import) |
| `Services/Accounting/Invoices/` | `InvoiceService` (abstract base), `CanadaInvoiceService`, `UsaInvoiceService`, `InvoiceServiceFactory` (resolves the country service from the agency billing location) |
| `Services/Accounting/PayStubs/` | `PayStubService` |
| `Services/Identity/` | see below |

Identity lives under `Services/Identity/` (interfaces in `Covenant.Common/Interfaces/Identity/`):

- `UserAdministrationService` — creates users (with confirmation emails), agency/company claims,
  role changes, email changes, deactivation (adds `InactiveUsers` and revokes OpenIddict tokens).
  `Covenant.Infrastructure/Services/UserAccountService.cs` implements `IUserAccountService` on
  top of it and keeps the `Users` mirror table of the API database in sync. The split follows the
  two databases: integration tests replace `IUserAdministrationService` with a mock and run the real
  `UserAccountService` without an identity database.
- `ICurrentUserService` (`Covenant.Infrastructure/Services/CurrentUserService.cs`) — who is calling:
  user, company, agency and personnel ids, nickname and role checks, read from the claims of the
  current `HttpContext`. Services use it instead of reading `HttpContext` themselves.
- `AccountNotificationService` — confirmation / set-password / reset-password links (rendered from
  `Covenant.Api/Views/Notifications/Identity/`) and the 6-digit reset code email.
- `UserSessionValidator` — the session kill switch (`InactiveUsers` + Microsoft Graph
  `accountEnabled` through `Microsoft365AccountService`).
- `PasswordResetService` — code-based forgot password.

Also in `Covenant.Core.BL/`: `Adapters/` (entity→model adapters for candidate, company, worker,
interfaces in `Covenant.Common/Interfaces/Adapters/`), `Extensions/` (`FormCollectionExtensions`)
and `Consumers/` (Service Bus).

Services depend only on repository interfaces from `Covenant.Common/Repositories/` (plus ASP.NET
Identity's `UserManager`/`RoleManager` for the identity services); controllers only delegate to
services. DI registration (services/repositories/adapters/containers `AddScoped`; Service Bus
clients + consumers and config option objects (`Rates`, `TimeLimits`, `AzureStorageConfiguration`)
`AddSingleton`; `AddCovenantIdentity` for `IdentityContext`, ASP.NET Identity core and the
OpenIddict core stores) lives in `Covenant.Api/Configuration/ApiServicesConfiguration.cs`.
`Covenant.Api/Configuration/OpenIddictConfiguration.cs` configures the OpenIddict server and
validation.

### Infrastructure — `Covenant.Infrastructure/`

```
Contexts/         CovenantContext.cs (main DbContext), IdentityContext.cs (ASP.NET Identity tables
                  User/Rol/UserClaim/UserRole/UserLogin/RoleClaim/UserToken + InactiveUsers +
                  PasswordResetCode + OpenIddict stores), MyKeysContext (DataProtection keys),
                  PostgresFunctions.cs (EF DbFunction mappings backing
                  Scripts/Functions/get_week_start_sunday.sql)
Repositories/     by domain, plural folders mirroring Covenant.Common/Repositories/:
                  Accounting/{Deductions,Invoices,PayStubs,Subcontractors}/, Agencies/, Candidates/,
                  Companies/, Identity/, Notifications/, Requests/, Workers/ + root repositories
                  (Catalog, Location, Shift, User); shared plumbing in BaseRepository.cs
Mappers/          entity→model projection extensions used inside repositories:
                  AgencyExtensionsMapping, CandidateExtensionsMapping, CompanyExtensionsMapping,
                  RequestExtensionsMapping, WorkerRequestExtensionsMapping
Configurations/   EF Core IEntityTypeConfiguration classes, same plural tree as Repositories/ plus
                  Sales/ (Deal, CompanyInteraction) and Requests/Runners/; catalogs and Location in
                  the root. Configurations/Identity/ belongs to IdentityContext only: both contexts
                  pick configurations by the exact namespace
                  Covenant.Infrastructure.Configurations.Identity, so never nest folders under it
Migrations/       EF Core migrations for CovenantContext; Migrations/Identity/ for IdentityContext
Scripts/          raw SQL (views, functions, stored procedures) run at startup
Services/         integrations: EmailService + SendGridService (SendGrid), GeocodeService
                  (Google Maps), PushNotifications (Azure Notification Hub), TeamsService
                  (webhooks), DocumentService, PdfGeneratorService, RazorViewToStringRenderer,
                  UserAccountService, CurrentUserService, Microsoft365AccountService (Graph
                  accountEnabled check), CraPdfParser (PdfPig reader for the CRA
                  deduction tables), SigookBusClient / SigookBusAdministrationClient (Service Bus)
Services/Storage/ Azure Blob containers, one class per container (see below)
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
`CraTableUploaded` (blob trigger, `Covenant.Api/Sigook.Functions/Functions/CraTables.cs`) calls the
API's `DeductionsController` (`POST api/jobs/deductions/Cpp/Blob` / `Tax/Blob`, which takes a
blob reference) → `DeductionImportService` (uses `CraPdfParser` + `CraTablesContainer`) →
`DeductionsRepository.ImportCpp/ImportTax(year, payPeriod, rows, yearsKept)`, keeping the last
2 years. Reads happen only through `TimesheetCalculatorService`; there is no endpoint to read
the tables back.

**Health checks** — `AddCovenantHealthChecks` + `Covenant.Api/HealthChecks/` (tagged
`config`/`ready`/`live`) probe the connection strings `DefaultConnection`, `IdentityConnection`,
`AccountingStorageConnection`, `FileStorageConnection`, `ServiceBusConnection`.

**API reference** — the OpenAPI document is generated by `Microsoft.AspNetCore.OpenApi`
(`AddOpenApi("v1")` + transformers in `Covenant.Api/Configuration/OpenApi/`) and written to
`.docs/technical/openapi.json` on every build by `Microsoft.Extensions.ApiDescription.Server`.
In Development and Staging the API serves the document at `/openapi/v1.json` and the Scalar UI at
`/scalar`.

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
applies pending EF migrations for `MyKeysContext`, `CovenantContext` and (when
`ConnectionStrings:IdentityConnection` is set) `IdentityContext`, executes the raw SQL in
`Covenant.Infrastructure/Scripts/`, seeds the 7 roles, runs `OpenIddictSeeder` (the
`api1`/`roles` scopes, the Sigook.Functions confidential client, and the `sigook.com`/`android`
public clients in Development), creates queues/topics/subscriptions if missing
(`ValidateCandidateQueue`, `BulkPayStubEmailQueue`, `InvitationQueue`, `CreateApplicantTopic`
with Teams/Email/RequestApplicant subscriptions), then calls `OnInit()` on every registered
consumer.

Consumers live in `Covenant.Core.BL/Consumers/`; exactly **5** are registered as
`IAzureServiceBusConsumer` singletons in `AddAzureServiceBusConsumer`
(`ApiServicesConfiguration.cs`): `NewCandidateConsumer`, `TeamsConsumer`,
`RequestApplicantConsumer`, `BulkPayStubEmailConsumer`, `InvitationConsumer`.

Local development connects to the **staging** Service Bus — inject `ISigookBusClient` (mockable)
rather than constructing clients directly.

---

## Authentication & Authorization

- **Covenant.Api is the OpenID Connect provider.** OpenIddict 7 serves `/connect/authorize`,
  `/connect/token`, `/connect/userinfo`, `/connect/endsession` and `/connect/revocation`
  (`Covenant.Api/Configuration/OpenIddictConfiguration.cs`, controller
  `Controllers/Identity/AuthorizationController.cs`). Enabled flows: authorization code + PKCE
  (Sigook.Web browser login), password (Sigook.Web and SigookApp native login screens), refresh
  token (non-rolling, 30 days; access tokens last 1 hour) and client credentials (Sigook.Functions).
  Access tokens are plain signed JWTs (encryption disabled) with `aud = api1`; the API validates
  them in-process (`AddValidation().UseLocalServer()`), so there is no discovery round-trip.
  The issuer is `Identity:IssuerUri`; signing/encryption certificates come from Key Vault
  (`Identity:SigningCertificateName`, `Identity:EncryptionCertificateName`) outside Development,
  where OpenIddict's development certificates are used.
- **Clients and scopes** are rows in the identity database (`OpenIddictApplications`,
  `OpenIddictScopes`), managed through migrations or `OpenIddictSeeder`; there is no admin UI.
  The `android`/`ios` public clients use `password` + `refresh_token`; the web clients (`sigook.com`,
  `all2job*`) add `authorization_code` with PKCE. Public clients send `client_id` only.
- **Claims**: `sub`, `name`/`preferred_username`/`email`, `email_verified`, `nickname` (the
  Microsoft 365 email for staff), `role` (one claim per role; userinfo returns a string for one role
  and an array for several), `agencyId`/`companyId` user claims, `idp` (`local` or `oidc`). Roles and
  user claims are re-read from the database on every token issuance, so a role change reaches the
  token at the next refresh. The API reads the subject through `PrincipalExtensions.GetSubject()`
  (`sub`, falling back to `ClaimTypes.NameIdentifier` for the test handlers) and roles through
  `IsInRole` (OpenIddict identities use `role` as the role claim type).
- The **password grant** (`Controllers/Identity/AuthorizationController.cs`, `ExchangePassword`)
  rejects inactive users and unconfirmed emails, applies lockout
  (5 attempts / 5 min) and returns `error = invalid_grant` with machine-readable
  `error_description` codes from `Covenant.Common/Constants/SignInErrors.cs`
  (`invalid_credentials`, `inactive_user`, `email_not_confirmed`, `locked_out`). Password grant
  issues no `id_token`; clients build the profile from `/connect/userinfo`.
  Sigook.Web keeps `oidc-client-ts` (`src/app/security/`) for token storage/refresh and for the
  "Sign in with Microsoft 365" button (`signinRedirect` with `acr_values=idp:oidc`, which goes
  straight to the external provider via `Controllers/Identity/ExternalController.cs`).
- **Microsoft 365 accounts are re-checked after login.** The external callback stores the Entra
  `oid` as the `microsoft_oid` user claim and rejects the login when Graph reports
  `accountEnabled = false`. `UserSessionValidator` repeats that Graph check (and the
  `InactiveUsers` check) every time a token is issued or userinfo is served, so blocking sign-in in
  the Microsoft 365 admin center or deactivating a user in Sigook ends their session at the next
  token refresh. Results are cached 5 minutes per user (`Microsoft365AccountService`); a Graph
  outage or missing credentials fails open (session allowed, error logged).
  The OIDC sign-in and the Graph check use the identity app registration
  (`Identity:Microsoft365ClientId` / `Identity:Microsoft365ClientSecret`, tenant from
  `Microsoft365Configuration:TenantId`), which holds the `User.Read.All` application permission and
  the `{IssuerUri}/signin-oidc` redirect URIs. `Microsoft365Configuration:ClientId/ClientSecret` is a
  different registration, used only to send email through Graph.
- **Forgot password** is API-driven (`POST /Password/forgot` → 6-digit emailed code, 15-min TTL,
  5 attempts, 60-s resend cooldown; `POST /Password/reset` with `{email, code, newPassword}`).
  Codes live in the `PasswordResetCode` table (hashed). Email flooding limits, all silent (the
  response never changes, so they don't reveal whether an email exists): per user at most 3 codes
  per hour and 6 per 24 h, counted from `PasswordResetCode`. On top of that, `/Password/*` and the
  anonymous `POST /Account/*` endpoints share the
  `password-reset` rate-limit policy (`RateLimitingConfiguration`): 10 requests per client IP in a
  sliding 10-minute window, answering `429` beyond it. The client IP comes from `X-Forwarded-For`
  (`ForwardedHeadersOptions` trusts any proxy because the App Service front end is the only way
  in). Both Sigook.Web (`/forgot-password`)
  and SigookApp (`/forgot-password` route, 2-step screen) consume it; Web and App also offer a
  resend-confirmation action when login fails with `email_not_confirmed`
  (`POST /Account/ResendConfirmationLink`, always `200` so it does not reveal whether an email exists).
- **Account-activation emails** link to Sigook.Web (`{WebClientUrl}/confirm-email` or
  `/create-password`, `?token=&id=`), built by `AccountNotificationService`. The pages post back to
  `POST /Account/ConfirmEmail` (`IUserAdministrationService.ConfirmEmail`) and
  `POST /Account/CreatePassword` (`IPasswordResetService.CreatePassword`, which also confirms the
  email); a bad or expired token answers `400 { error: "invalid_token" }`. The old
  `GET /Account/ConfirmEmailAddress|CreatePassword|ResetPassword` links still in inboxes redirect to
  those pages. The API serves no HTML pages: `Views/` only holds email and PDF templates.
- **Browser sign-in is Microsoft 365 only.** `/connect/authorize` without a session always goes to
  `External/Challenge` (also the cookie `LoginPath`); a rejected staff login lands on
  `{WebClientUrl}/login?error=invalid_user`.
- **User administration** (create user, agency/company claims, role and email changes,
  deactivation) happens in-process through `IUserAdministrationService`; the API writes the
  identity database directly (`ConnectionStrings:IdentityConnection`) and mirrors the user in its
  own `Users` table.
- **Roles** (exactly 7, lowercase, defined in `Covenant.Common/Constants/CovenantConstants.cs`):
  `superadmin`, `admin`, `recruiting`, `sales`, `company`, `company.user`, `worker`. Role groups:
  `RecruitingAccess` (superadmin/admin/recruiting), `SalesAccess` (superadmin/admin/sales),
  `AgencyStaff` (recruiting access + sales), `AdminAccess` (superadmin/admin),
  `AgencyAssignable` (admin/recruiting/sales), `SuperAdminAssignable` (`CovenantConstants.cs`).
  Always reference via `CovenantConstants.Role.*`. They are seeded at startup.
- **Policies** in `Covenant.Api/Authorization/PolicyConfiguration.cs`: `Agency`, `Recruiting`,
  `Sales`, `Company`, `Worker`, `Admin`, `SuperAdmin`. Every policy requires a role — there is no
  authenticated-only policy, no `Accounting` policy, and no cross-actor policies. An endpoint
  reachable by two actors is exposed once per actor (`Controllers/Agency/`, `Controllers/Company/`,
  `Controllers/Worker/`), each under its own policy, with the shared behaviour in a `Covenant.Core.BL`
  service. The default authentication scheme is the OpenIddict validation (bearer) scheme; the
  Identity application cookie only backs the Microsoft 365 sign-in and the authorize endpoint.

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
  (`Controllers/Agency/Sales/`) pass the personnel id down, and
  `Covenant.Infrastructure/Repositories/Companies/CompanyRepository.cs` filters
  `CompanyProfile.SalesRepresentativeId == salesPersonnelId` (requests are filtered by the
  companies assigned to that rep in `RequestRepository`).
- **Company users** see only their company; **workers** see only their own profile/requests.

---

## Deployment

| | Staging | Production |
|---|---|---|
| Branch | `dev` (auto-deploy on push) | `main` (manual deploy from Azure DevOps) |
| API + identity | sigook-api-staging.azurewebsites.net (`staging.accounts.sigook.ca`) | sigook-api.azurewebsites.net (`accounts.sigook.ca`) |
| Web | sigook-web-staging.azurewebsites.net | sigook.azurewebsites.net |
| Marketing | lively-island-020c8260f.7.azurestaticapps.net | www.covenantgroupl.com |
| Functions | — | sigook-functions.azurewebsites.net |

Azure DevOps pipelines with path-based triggers and reusable templates
(`.azure-pipelines/templates/`); Docker builds for the API (context `Covenant.Api/`, image
`sigook.azurecr.io/api`) and Sigook.Web, zip deploy for the Functions, static build for
Covenant.Web. Full details: [PIPELINES.md](PIPELINES.md).

---

## Database

PostgreSQL + EF Core 10, two databases with one context each. Migrations are applied
**automatically at API startup** by `SigookBackgroundService`; to add one:

```bash
dotnet ef migrations add MigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api --context CovenantContext
dotnet ef migrations add MigrationName --project Covenant.Api/Covenant.Infrastructure --startup-project Covenant.Api/Covenant.Api --context IdentityContext --output-dir Migrations/Identity
```

`IdentityContext` does not call `IdentityDbContext.OnModelCreating` (the identity tables keep their
historical shape: custom table names, `UserLogin`/`UserToken` keyed by `UserId` only, no
`AspNet*` indexes) and ignores the .NET 10 passkey entity. `Npgsql.EnableLegacyTimestampBehavior`
is switched on after `builder.Build()` in `Program.cs`; keep it there — `dotnet ef` runs `Program`
only up to the host build, so moving it earlier changes the design-time column types of every
`DateTime` property.

Conventions: PK `Id` (Guid) + human-facing sequential `NumberId` on major entities; FKs `{Entity}Id`;
`CreatedAt`/`UpdatedAt` timestamps; `CreatedBy`/`UpdatedBy` audit strings on newer entities.

Development commands: [DEVELOPMENT_COMMANDS.md](DEVELOPMENT_COMMANDS.md).
API endpoint reference: `openapi.json` (generated on build).
