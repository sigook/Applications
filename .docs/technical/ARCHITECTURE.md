# Technical Architecture - Covenant/Sigook Platform

## 🎯 Overview

The Covenant/Sigook platform is built as a **monorepo** with several specialized applications:

- **Backend API** — .NET 8 (Covenant.Api)
- **Identity Server** — .NET 6 + IdentityServer4 (Covenant.IdentityServer)
- **Main Web App** — Vue.js 3 (Sigook.Web)
- **Marketing Website** — Vue.js 3 (Covenant.Web)
- **Mobile App** — Flutter (SigookApp)
- **Azure Functions** — .NET 8 (Sigook.Functions)
- **Cognitive Services** — .NET 8 (Sigook.CognitiveServices)

---

## 🏗️ Tech Stack

### Backend - Covenant.Api (.NET 8)

```
Framework:    ASP.NET Core 8.0 Web API
SDK:          .NET 8.0.415
Database:     PostgreSQL (cloud-hosted)
ORM:          Entity Framework Core 8.0.11
Patterns:     Repository, Service Layer, CQRS (MediatR)
Architecture: Domain-Driven Design, Clean Architecture
```

**Cloud Services:**
- Azure Storage — Documents, files, PDFs
- Azure Service Bus — Async messaging
- Azure Container Registry — Docker images
- Azure App Service — Hosting

**Main NuGet Packages:**
- Npgsql.EntityFrameworkCore.PostgreSQL 8.0.10
- MediatR 12.4.1
- AutoMapper 12.0.1
- FluentValidation 11.10.0
- Serilog
- Swashbuckle (Swagger) 7.2.0
- Azure.Messaging.ServiceBus 7.18.2
- Azure.Storage.Blobs 12.24.0
- ClosedXML 0.104.2

---

### Identity Server - Covenant.IdentityServer (.NET 6)

```
Framework: ASP.NET Core 6.0
SDK:       .NET 6.0.400
Auth:      IdentityServer4 4.1.2
ORM:       Entity Framework Core 6.0.13
Protocols: OpenID Connect, OAuth 2.0
```

**Staging:** `https://sigook-accounts-staging.azurewebsites.net`
**Production:** `https://sigook-accounts.azurewebsites.net`

---

### Web App - Sigook.Web (Vue.js 3)

```
Framework:    Vue.js 3.5.x
Build Tool:   Vite 8.x
Language:     TypeScript 6.x
Node Version: 20+ (required by Vite 8)
State:        Pinia 3.x (+ pinia-plugin-persistedstate)
Router:       Vue Router 4.x
HTTP:         Axios 1.10.0
Auth:         oidc-client-ts 3.x
UI Framework: @ntohq/buefy-next 0.2.x (Buefy port for Vue 3 / Bulma)
Validation:   VeeValidate 4.x + Yup 1.x
i18n:         vue-i18n
Deployment:   Docker (Node.js build → Nginx)
```

**Staging:** `https://sigook-web-staging.azurewebsites.net`
**Production:** `https://sigook.azurewebsites.net`

---

### Marketing Website - Covenant.Web (Vue.js 3)

```
Framework:    Vue.js 3.5.22
Build Tool:   Vite 7.1.11
Language:     TypeScript 5.9.3
State:        Pinia 3.0.3
Router:       Vue Router 4.6.3
UI Framework: Vuetify 3.7.0
Validation:   VeeValidate 4.15.1 + Yup 1.7.1
Node Version: ^20.19.0 OR >=22.12.0
Hosting:      Azure Static Web Apps (Free tier)
```

**Staging:** `https://lively-island-020c8260f.7.azurestaticapps.net`
**Production:** `https://www.covenantgroupl.com`

---

### Mobile App - SigookApp (Flutter)

```
Framework:    Flutter (Dart ^3.9.2)
Architecture: Clean Architecture (3 layers)
State:        Riverpod (flutter_riverpod ^3.0.3)
Routing:      GoRouter ^17.0.0
Immutability: Freezed ^3.2.3
Functional:   Dartz ^0.10.1 (Either, Option)
DI:           Riverpod + get_it ^9.0.5
HTTP:         Dio ^5.7.0
Auth:         flutter_appauth ^11.0.0
Storage:      SharedPreferences, FlutterSecureStorage
Code Gen:     build_runner, freezed, json_serializable
```

**Build Flavors:**
- `staging` — Staging environment
- `production` — Production environment

---

### Azure Functions - Sigook.Functions (.NET 8)

```
Framework:  .NET 8.0
SDK:        .NET 8.0.415
Runtime:    Azure Functions v4 (Isolated Worker)
Worker SDK: Microsoft.Azure.Functions.Worker 1.23.0
```

**Functions:**
- `SendEmail` — HTTP trigger, sends emails via SendGrid
- `SendInvitationToApply` — Queue trigger (Azure Storage Queue), sends job invitations
- `NotificationSinExpiration` — Timer trigger, notifies SIN document expiration
- `WarnLicensesExpiration` — Timer trigger, warns about license expirations

**Key Dependencies:**
- Covenant.Common NuGet package
- SendGrid 9.29.3
- IdentityModel 7.0.0
- Application Insights 2.22.0

---

### Cognitive Services - Sigook.CognitiveServices (.NET 8)

```
Framework:    .NET 8.0
SDK:          .NET 8.0.415
Architecture: Clean Architecture (3 layers)
```

**Projects:**
```
Sigook.CognitiveServices/
├── Sigook.CognitiveServices.Core/            # Domain logic
├── Sigook.CognitiveServices.Infraestructure/ # Infrastructure layer
└── Sigook.CognitiveServices.UI/              # ASP.NET Core Web UI
```

**Key Dependencies:**
- Microsoft.CognitiveServices.Speech 1.24.1

---

## 🏛️ Backend Architecture - Covenant.Api

### Project Structure

```
Covenant.Api/
├── Covenant.Api/             # Web API (Controllers, Startup)
├── Covenant.Common/          # Entities, Interfaces, Models (NuGet package)
├── Covenant.Core.BL/         # Business Logic (Services)
├── Covenant.Infrastructure/  # Data Access, EF Core, Repositories, Deductions, Integrations
├── Covenant.Documents/       # Excel/PDF generation
├── Covenant.Tests/           # Unit tests
├── Covenant.Test.Utils/      # Test utilities
└── Covenant.Integration.Tests/ # Integration tests
```

> Earlier revisions of this document referenced separate projects such as `Covenant.PayStubs`, `Covenant.Deductions`, `Covenant.TimeSheetTotal`, and `Covenant.Subcontractor`. Those are **not** part of the current solution — that logic now lives inside `Covenant.Core.BL/Services` and `Covenant.Infrastructure/Deductions`.

---

### Architecture Layers

#### 1️⃣ PRESENTATION LAYER - Controllers

**Location:** Each module groups its controllers under `Covenant.Api/{Module}Module/{Resource}/Controllers/`. A handful of cross-cutting controllers (Catalog, Location, File, Agency root) live under `Covenant.Api/Controllers/Sigook/`.

**Module structure:**

```
Covenant.Api/
├── Controllers/Sigook/                # Shared / cross-cutting
│   ├── CatalogController              → Countries, Provinces, Cities, Skills, etc.
│   ├── LocationController             → Address geocoding
│   ├── File*Controller                → File upload/download
│   └── Agency/                        → Agency, AgencyLocation
│
├── AgencyModule/                      # Agency perspective
│   ├── AgencyRequest/Controllers/             → Manage requests
│   ├── AgencyWorkerProfile/Controllers/       → Manage workers
│   ├── AgencyCompanyProfile/Controllers/      → Manage companies
│   ├── AgencyCandidate/Controllers/           → Manage candidates
│   ├── AgencyRequestWorker/Controllers/       → Assign workers to requests
│   ├── AgencyRequestWorkerTimeSheet/Controllers/ → Approve timesheets
│   └── ... (one folder per resource)
│
├── CompanyModule/                     # Company perspective
│   ├── CompanyRequest/Controllers/
│   ├── CompanyProfile/Controllers/
│   ├── CompanyLocation/Controllers/
│   ├── CompanyJobPosition/Controllers/
│   ├── CompanyRequestWorker/Controllers/
│   └── CompanyRequestWorkerTimeSheet/Controllers/
│
├── WorkerModule/                      # Worker perspective
│   ├── WorkerProfile/Controllers/             → Manage profile
│   ├── WorkerRequest/Controllers/             → Browse / apply to jobs
│   └── WorkerRequestTimeSheet/Controllers/    → Clock in/out
│
├── AccountingModule/                  # Accounting operations
│   ├── AgencyInvoice/Controllers/             → AccountingInvoiceV4Controller
│   └── Deduction/                             → CPP / Federal Tax / Provincial Tax
│   # Pay stub CRUD, PDF & email → Controllers/Sigook/Agency/Accounting/PayStubsController
│   # Invoice PDF & email        → Controllers/Sigook/Agency/Accounting/InvoicesController
│
├── ManagerModule/
└── Security/                          # Authentication
```

**Naming convention:** controllers follow the pattern `{Module}{Resource}V{N}Controller.cs` for versioned routes (e.g. `AccountingPayStubV4Controller`) and `{Module}{Resource}Controller.cs` otherwise.

**Responsibilities:**
- ✅ HTTP request/response handling
- ✅ Input validation (model binding)
- ✅ Authorization checks (`[Authorize]` attributes)
- ✅ Delegate to Services (Business Logic layer)
- ❌ NO business logic in controllers
- ❌ NO direct database access

---

#### 2️⃣ BUSINESS LOGIC LAYER - Services

**Location:** `Covenant.Core.BL/Services/`

**Main services:**

```csharp
// Request Management
RequestService
  - CreateRequest, UpdateRequest, CancelRequest
  - AddWorker, RejectWorker

// Worker Management
WorkerService
  - CreateWorker, UpdateWorkerProfile
  - ApproveToWork, UpdateDnu

// Company Management
CompanyService
  - CreateCompany, UpdateCompany
  - AddLocation, UpdateLocation
  - AddJobPositionRate

// Timesheet Management
TimeSheetService
  - CreateTimeSheet, ApproveTimeSheet
  - ClockIn, ClockOut
  - CalculateTimeSheetTotal

// Accounting Orchestration
AccountingService
  - GeneratePayStubs, GetPayStubs
  - CreateInvoice, PreviewInvoice
  - Delegates to CanadaInvoiceService or UsaInvoiceService

// Pay Stub Generation
PayStubService
  - GeneratePayStubForWorker
  - CalculateDeductions (CPP, EI, taxes)

// Location & Geocoding
LocationService
  - GeocodeAddress
  - ValidatePostalCode
  - GetDistanceBetweenLocations
```

**Responsibilities:**
- ✅ Business logic and validation
- ✅ Orchestration (coordinating multiple repositories)
- ✅ Business rule enforcement
- ✅ Calculations (payroll, billing)
- ✅ Call repositories for data access
- ❌ NO direct database queries

---

#### 3️⃣ DATA ACCESS LAYER - Infrastructure

**Location:** `Covenant.Infrastructure/`

**Structure:**

```
Covenant.Infrastructure/
├── Contexts/
│   └── CovenantContext.cs              # Main EF Core DbContext
│
├── Repositories/                       # Repository implementations, organized by domain
│   ├── Worker/
│   ├── Company/
│   ├── Agency/
│   ├── Request/
│   └── Accounting/
│
├── Configurations/                     # EF Core entity configurations
│
├── Deductions/                         # CPP / EI / Federal / Provincial calculators + tax tables
│   ├── Repositories/
│   └── Tables/
│
├── Migrations/                         # EF Core migrations
│
└── Integrations/                       # External services
    ├── EmailService                    → SendGrid
    ├── GeocodeService                  → Google Maps API
    ├── PushNotificationService         → Azure Notification Hub
    ├── TeamsWebhookService             → Microsoft Teams
    └── AzureStorageService             → Azure Blob Storage
```

**Responsibilities:**
- ✅ Database queries (EF Core)
- ✅ Entity mapping and configurations
- ✅ External service integrations
- ✅ Implement repository interfaces
- ❌ NO business logic

**CovenantContext (DbContext):**
```csharp
public class CovenantContext : DbContext
{
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<WorkerRequest> WorkerRequests { get; set; }
    public DbSet<TimeSheet> TimeSheets { get; set; }
    public DbSet<PayStub> PayStubs { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    // ... more entities
}
```

---

#### 4️⃣ DOMAIN LAYER - Common

**Location:** `Covenant.Common/` (shared NuGet package)

**Structure:**

```
Covenant.Common/
├── Entities/                           # Domain entities
│   ├── Agency/
│   ├── Company/
│   ├── Worker/
│   ├── Candidate/
│   ├── Request/
│   └── Accounting/
│
├── Enums/                              # Domain enums
│
├── Models/                             # DTOs, ViewModels (mirrors Entities)
│
├── Interfaces/                         # Service contracts
│
├── Repositories/                       # Repository interfaces
│
└── Functionals/                        # Functional types (Result<T>, etc.)
```

**Responsibilities:**
- ✅ Domain entities with business logic
- ✅ Value objects
- ✅ Interfaces (contracts)
- ✅ Domain events
- ❌ NO external dependencies
- ❌ NO infrastructure concerns

**Publication:**
- Built as a NuGet package: `Covenant.Common`
- Consumed by: Covenant.Api, Covenant.IdentityServer, Sigook.Functions

---

#### 5️⃣ SPECIALIZED MODULES

**Invoice / Billing Logic** (in `Covenant.Core.BL/Services/Invoices/`)
```
Responsibilities:
- Invoice listing, export, preview, creation, PDF and email orchestration
- Invoice calculations (rates, markup)
- Tax calculations (HST/GST per province)
- Discount calculations
- Additional item calculations

Key classes:
- IInvoiceService / InvoiceService (abstract base: orchestration + shared Canada/USA logic)
- CanadaInvoiceService / UsaInvoiceService (country-specific PreviewAsync/CreateAsync, IsUSA)
- IInvoiceServiceFactory / InvoiceServiceFactory (resolves the country service via the agency billing location)

Entities: Covenant.Common/Entities/Accounting/Invoice/
```

**Pay Stub Generation** (in `Covenant.Core.BL/Services/PayStubService.cs`)
```
Responsibilities:
- Generate pay stubs from approved timesheets
- Earnings calculations
- Deductions (delegates to Covenant.Infrastructure/Deductions)
- Vacation pay (4% Canada)
- Public holiday pay
```

**Deductions** (in `Covenant.Infrastructure/Deductions/`)
```
Responsibilities:
- CPP (Canada Pension Plan)
- EI (Employment Insurance)
- Federal tax
- Provincial tax (ON, BC, QC, AB, etc.)
- Tax tables (updated annually)

Calculators are exposed via REST endpoints under
api/Accounting/Deduction/Cpp, /FederalTax, /ProvincialTax.
```

**Timesheet Total Calculations** (in `Covenant.Core.BL/Services/TimeSheetService.cs`)
```
Responsibilities:
- Regular hours vs overtime
- Weekly accumulation
- Night shift detection (11 PM - 7 AM)
- Holiday detection
- Break deductions
```

**Document Generation** (in `Covenant.Documents/`)
```
Responsibilities:
- Excel reports (requests, payroll, invoices)
- PDF documents (invoices, pay stubs)
- Azure Storage upload/download
```

**Notifications** (in `Covenant.Infrastructure/Integrations/`)
```
- EmailService            → SendGrid
- TeamsWebhookService     → Microsoft Teams
- PushNotificationService → Azure Notification Hub / FCM
```

---

## 📱 Mobile Architecture - SigookApp (Flutter)

### Clean Architecture - 3 Layers

```
lib/
├── features/                  # Features organized by domain
│   ├── registration/
│   ├── auth/
│   ├── jobs/
│   └── timesheets/
│
├── core/                      # Core infrastructure
│   ├── config/                → Environment config
│   ├── network/               → API client, interceptors
│   ├── routing/               → GoRouter configuration
│   ├── theme/                 → App theme
│   ├── providers/             → Core providers
│   └── error/                 → Error handling
│
└── main_staging.dart / main_production.dart
```

### Feature Structure (Clean Architecture)

```
features/registration/
│
├── domain/                    # ← DOMAIN LAYER (pure)
│   ├── entities/              → Business objects
│   │   └── value_objects/     → Type-safe primitives
│   │
│   ├── repositories/          → Interfaces (abstractions)
│   │
│   └── usecases/              → Business logic
│
├── data/                      # ← DATA LAYER (implementation)
│   ├── models/                → DTOs with JSON (Freezed)
│   │
│   ├── datasources/           → Data sources
│   │   ├── *_local_datasource.dart   # SharedPreferences
│   │   └── *_remote_datasource.dart  # API calls (Dio)
│   │
│   └── repositories/          → Interface implementations
│
└── presentation/              # ← PRESENTATION LAYER (UI)
    ├── pages/                 → Full screens
    ├── widgets/               → Reusable components
    ├── viewmodels/            → UI logic (Riverpod StateNotifier)
    └── providers/             → Riverpod providers
```

### Layer Dependencies

```
Presentation → Domain ← Data
     ↓           ↓         ↓
   Widgets   Entities  Models
     ↓           ↓         ↓
ViewModels  UseCases  Datasources
     ↓           ↓         ↓
Providers  Repositories  API/Storage
```

**Golden rule:**
- Domain depends on nothing (pure Dart)
- Data implements Domain interfaces
- Presentation uses Domain (never Data directly)

### Core Infrastructure

**API Client:** `lib/core/network/api_client.dart`
```dart
class ApiClient {
  final Dio _dio;

  ApiClient(this._dio) {
    _dio.options.baseUrl = EnvironmentConfig.apiBaseUrl;
    _dio.options.connectTimeout = Duration(seconds: 30);
    _dio.interceptors.add(AuthInterceptor());
    _dio.interceptors.add(PrettyDioLogger());
  }
}
```

**Auth Interceptor:** `lib/core/network/auth_interceptor.dart`
- Injects the Bearer token automatically
- Handles 401 (token refresh + retry)

**Environment Config:** `lib/core/config/environment.dart`
- Loads `.env.staging` or `.env.production`
- Exposes: AUTH_AUTHORITY, API_BASE_URL, CLIENT_ID, etc.

**Routing:** `lib/core/routing/app_router.dart`
- GoRouter with custom transitions
- KeyboardDismissObserver

---

## 🌐 Frontend Architecture - Sigook.Web (Vue 3)

### Project Structure

```
Sigook.Web/
├── src/
│   ├── assets/         # Images, styles
│   ├── components/     # Reusable components (mostly <script setup>)
│   ├── pages/          # Page components
│   ├── router/         # Vue Router 4 config
│   ├── store/          # Pinia stores
│   ├── api/            # HTTP / API layer (TypeScript)
│   ├── types/          # Shared TypeScript types
│   ├── security/       # OIDC authentication (oidc-client-ts)
│   ├── utils/          # Utilities
│   ├── directives/     # Custom directives
│   ├── filters/        # Formatter helpers (Vue 3 has no filters — used as plain fns)
│   ├── lang/           # i18n translations
│   ├── composables/    # Composition API utilities
│   └── main.ts
│
├── public/             # Static assets
├── wwwroot/            # Build output (not dist/)
├── Dockerfile          # Multi-stage build
├── nginx.conf          # Nginx config
└── vite.config.ts
```

**Deployment:**
- Build: Node.js 22+ + pnpm → `pnpm run staging` or `pnpm run production` (Vite)
- Deploy: Docker image with Nginx serving static files

---

## 🔐 Authentication and Authorization

### Full Flow

```
┌─────────────────────────────────────────────────────────┐
│ 1. User accesses the application                        │
│    (Web: Sigook.Web, Mobile: SigookApp)                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. Redirect to Covenant.IdentityServer                  │
│    sigook-accounts.azurewebsites.net                    │
│    Protocols: OpenID Connect + OAuth 2.0                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. User enters credentials                              │
│    Email + Password                                     │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 4. IdentityServer validates and issues tokens           │
│    - access_token (JWT)                                 │
│    - id_token (user info)                               │
│    - refresh_token                                      │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 5. App stores tokens                                    │
│    Web: LocalStorage (via oidc-client)                  │
│    Mobile: FlutterSecureStorage (encrypted)             │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 6. All requests to Covenant.Api include the token       │
│    Authorization: Bearer {access_token}                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 7. API validates the token and extracts claims          │
│    - UserId, AgencyId, Roles, Permissions               │
│    - AuthorizationFilter applies data isolation         │
└─────────────────────────────────────────────────────────┘
```

### Data Isolation (Multi-tenancy)

**Agency Users:**
- Automatic filter: `query.Where(x => x.AgencyId == userAgencyId)`
- See only their agency's data

**Company Users:**
- Automatic filter: `query.Where(x => x.CompanyProfileId == userCompanyId)`
- See only their company's data

**Workers:**
- Automatic filter: `query.Where(x => x.WorkerProfileId == userWorkerId)`
- See only their own data

---

## 🚀 Deployment Architecture

### Environments

**Staging:**
- Branch: `dev`
- Auto-deploy on push to `dev`
- API: `sigook-api-staging.azurewebsites.net`
- Web: `sigook-web-staging.azurewebsites.net`
- Identity: `sigook-accounts-staging.azurewebsites.net`
- Marketing: `lively-island-020c8260f.7.azurestaticapps.net` (Static Web App)

**Production:**
- Branch: `main`
- Manual deploy from Azure DevOps
- API: `sigook-api.azurewebsites.net`
- Web: `sigook.azurewebsites.net`
- Identity: `sigook-accounts.azurewebsites.net`
- Marketing: `www.covenantgroupl.com` (Static Web App `covenantgroup-swa`)

### CI/CD Pipeline

**Azure DevOps Pipelines:**
- Path-based triggers (only runs if that app changed)
- Reusable templates (`.azure-pipelines/templates/`)
- Multi-stage: Build → Test → Deploy
- Docker build for backend
- Node.js build for frontend

See `.docs/technical/PIPELINES.md` for full details.

---

## 📊 Database Schema

**Provider:** PostgreSQL (cloud-hosted)
**ORM:** Entity Framework Core 8.0.11

**Migrations:**
```bash
cd Covenant.Api
dotnet ef migrations add MigrationName --project Covenant.Infrastructure --startup-project Covenant.Api
dotnet ef database update --project Covenant.Infrastructure --startup-project Covenant.Api
```

**Conventions:**
- Table names: PascalCase (singular)
- Foreign keys: `{Entity}Id`
- Timestamps: `CreatedAt`, `UpdatedAt`
- Soft delete: `DeletedAt` (nullable)

---

## 🔧 Development Commands

See `.docs/technical/DEVELOPMENT_COMMANDS.md` for the complete list.

---

## 📈 Scalability Considerations

**Current:**
- Monolithic API (all modules in one project)
- Single PostgreSQL database
- Azure App Service (horizontal scaling)
- Azure Functions for background jobs (Sigook.Functions)
- Azure Cognitive Services for AI (Sigook.CognitiveServices)

**Future Improvements:**
- Microservices (Accounting, Payroll as separate services)
- CQRS with Event Sourcing
- Read replicas for reports
- Redis for caching
