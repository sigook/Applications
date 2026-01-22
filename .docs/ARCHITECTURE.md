# Arquitectura Técnica - Covenant/Sigook Platform

## 🎯 Overview

La plataforma Covenant/Sigook está construida como un **monorepo** con múltiples aplicaciones especializadas:

- **Backend API** - .NET 6 (Covenant.Api)
- **Identity Server** - .NET 6 + IdentityServer4 (Covenant.IdentityServer)
- **Web App Principal** - Vue.js 2 (Sigook.Web)
- **Marketing Website** - Vue.js 3 (covenantWeb)
- **Mobile App** - Flutter (SigookApp)

---

## 🏗️ Stack Tecnológico

### Backend - Covenant.Api (.NET 6)

```
Framework:       ASP.NET Core 6.0 Web API
Database:        PostgreSQL (cloud-hosted)
ORM:             Entity Framework Core 6
Patterns:        Repository, Service Layer, CQRS (MediatR)
Architecture:    Domain-Driven Design, Clean Architecture
```

**Cloud Services:**
- Azure Storage - Documentos, archivos, PDFs
- Azure Service Bus - Mensajería asíncrona
- Azure Container Registry - Docker images
- Azure App Service - Hosting

**NuGet Packages principales:**
- Npgsql.EntityFrameworkCore.PostgreSQL
- MediatR
- AutoMapper
- FluentValidation
- Serilog
- Swashbuckle (Swagger)

---

### Identity Server - Covenant.IdentityServer (.NET 6)

```
Framework:       ASP.NET Core 6.0
Auth:            IdentityServer4
Protocols:       OpenID Connect, OAuth 2.0
```

**Staging:** `https://sigook-accounts-staging.azurewebsites.net`
**Production:** `https://sigook-accounts.azurewebsites.net`

---

### Web App - Sigook.Web (Vue.js 2)

```
Framework:       Vue.js 2.6.12
Build Tool:      Vue CLI 4.5.19
Node Version:    16.x
State:           Vuex 3.0.1
Router:          Vue Router 3.0.1
HTTP:            Axios 1.10.0
Auth:            OIDC Client 1.5.2
UI Framework:    Buefy 0.9.23 (Bulma)
i18n:            vue-i18n
Deployment:      Docker (Node.js build → Nginx)
```

**Staging:** `https://sigook-web-staging.azurewebsites.net`
**Production:** `https://sigook.azurewebsites.net`

---

### Marketing Website - covenantWeb (Vue.js 3)

```
Framework:       Vue.js 3.5.22
Build Tool:      Vite 7.1.11
Language:        TypeScript 5.9.3
State:           Pinia 3.0.3
Router:          Vue Router 4.6.3
Node Version:    ^20.19.0 OR >=22.12.0
```

**Staging:** `https://covenantgroup-staging.azurewebsites.net`
**Production:** `https://covenantgroup.azurewebsites.net`

---

### Mobile App - SigookApp (Flutter)

```
Framework:       Flutter (Dart ^3.9.2)
Architecture:    Clean Architecture (3 layers)
State:           Riverpod (flutter_riverpod ^3.0.3)
Routing:         GoRouter ^17.0.0
Immutability:    Freezed ^3.2.3
Functional:      Dartz ^0.10.1 (Either, Option)
DI:              Riverpod + get_it ^9.0.5
HTTP:            Dio ^5.7.0
Auth:            flutter_appauth ^11.0.0
Storage:         SharedPreferences, FlutterSecureStorage
Code Gen:        build_runner, freezed, json_serializable
```

**Build Flavors:**
- `staging` - Staging environment
- `production` - Production environment

---

## 🏛️ Arquitectura Backend - Covenant.Api

### Estructura de Proyectos

```
Covenant.Api/
├── Covenant.Api/                    # Web API (Controllers, Startup)
├── Covenant.Common/                 # Entities, Interfaces (NuGet package)
├── Covenant.Core.BL/                # Business Logic (Services)
├── Covenant.Infrastructure/         # Data Access, EF Core, Repositories
├── Covenant.Billing/                # Billing module
├── Covenant.PayStubs/               # Pay stubs generation
├── Covenant.Deductions/             # Tax calculations (CPP, EI, taxes)
├── Covenant.TimeSheetTotal/         # Timesheet calculations
├── Covenant.Documents/              # Excel/PDF generation
├── Covenant.Notifications/          # Email, Teams, Push notifications
├── Covenant.Tests/                  # Unit tests
└── Covenant.Integration.Tests/      # Integration tests
```

---

### Capas de la Arquitectura

#### 1️⃣ PRESENTATION LAYER - Controllers

**Ubicación:** `Covenant.Api/Controllers/`

**Estructura por Módulos:**

```
Controllers/
├── AgencyModule/                    # Agency perspective
│   ├── AgencyRequestController          → Manage requests
│   ├── AgencyWorkerProfileController    → Manage workers
│   ├── AgencyCompanyProfileController   → Manage companies
│   ├── AgencyCandidateController        → Manage candidates (pre-workers)
│   ├── AgencyRequestWorkerController    → Assign workers to requests
│   └── AgencyRequestTimeSheetController → Approve timesheets
│
├── CompanyModule/                   # Company perspective
│   ├── CompanyRequestController         → Manage my requests
│   ├── CompanyProfileController         → Manage my profile
│   ├── CompanyLocationController        → Manage locations
│   └── CompanyRequestWorkerController   → View assigned workers
│
├── WorkerModule/                    # Worker perspective
│   ├── WorkerProfileController          → Manage my profile
│   ├── WorkerRequestController          → Browse/apply to jobs
│   └── WorkerRequestTimeSheetController → Clock in/out, timesheets
│
├── AccountingModule/                # Accounting operations
│   ├── PayStubController                → Generate pay stubs
│   ├── InvoiceController                → Generate invoices
│   ├── InvoiceDocumentController        → Invoice PDFs
│   └── PayStubDocumentController        → Pay stub PDFs
│
├── CatalogController/               # Shared catalogs
│   └── Countries, Provinces, Cities, Skills, etc
│
└── Security/                        # Authentication
    ├── AccountController                → Register, login
    └── IdentityController               → User info
```

**Responsabilidades:**
- ✅ HTTP request/response handling
- ✅ Input validation (model binding)
- ✅ Authorization checks ([Authorize] attributes)
- ✅ Delegate to Services (Business Logic layer)
- ❌ NO business logic en controllers
- ❌ NO direct database access

---

#### 2️⃣ BUSINESS LOGIC LAYER - Services

**Ubicación:** `Covenant.Core.BL/Services/`

**Servicios Principales:**

```csharp
// Request Management
RequestService
  - CreateRequest, UpdateRequest, CancelRequest
  - PutInProcess, OpenRequest
  - AddWorker, RejectWorker
  - SendInvitation

// Worker Management
WorkerService
  - CreateWorker, UpdateWorkerProfile
  - ApproveToWork, UpdateDnu
  - CanApply, CanBeBook

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
  - CreatePayStubUsingTimeSheet
  - CalculateDeductions (CPP, EI, taxes)
  - GeneratePayStubDocument

// Location & Geocoding
LocationService
  - GeocodeAddress (Google Maps API)
  - ValidatePostalCode
  - GetDistanceBetweenLocations
```

**Responsabilidades:**
- ✅ Business logic y validaciones
- ✅ Orchestration (coordinar múltiples repositorios)
- ✅ Business rules enforcement
- ✅ Calculations (payroll, billing)
- ✅ Call repositories para data access
- ❌ NO direct database queries

---

#### 3️⃣ DATA ACCESS LAYER - Infrastructure

**Ubicación:** `Covenant.Infrastructure/`

**Estructura:**

```
Infrastructure/
├── Contexts/
│   └── CovenantContext.cs           # EF Core DbContext principal
│
├── Repositories/
│   ├── Worker/
│   │   ├── WorkerRepository
│   │   └── WorkerCommentsRepository
│   ├── Company/
│   │   └── CompanyRepository
│   ├── Agency/
│   │   └── AgencyRepository
│   ├── Request/
│   │   ├── RequestRepository
│   │   ├── TimeSheetRepository
│   │   └── WorkerRequestRepository
│   └── Accounting/
│       ├── InvoiceRepository
│       ├── PayStubRepository
│       └── SubcontractorRepository
│
├── Configurations/                  # EF Core entity configurations
│   ├── WorkerConfiguration.cs
│   ├── CompanyConfiguration.cs
│   └── ...
│
└── Integrations/                    # External services
    ├── EmailService                 → SendGrid
    ├── GeocodeService               → Google Maps API
    ├── PushNotificationService      → Azure Notification Hub
    ├── TeamsWebhookService          → Microsoft Teams
    └── AzureStorageService          → Azure Blob Storage
```

**Responsabilidades:**
- ✅ Database queries (EF Core)
- ✅ Entity mapping y configurations
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
    // ... más entities
}
```

---

#### 4️⃣ DOMAIN LAYER - Common

**Ubicación:** `Covenant.Common/` (NuGet package compartido)

**Estructura:**

```
Covenant.Common/
├── Entities/                        # Domain entities
│   ├── Agency/
│   │   ├── Agency.cs
│   │   ├── AgencyLocation.cs
│   │   └── AgencyPersonnel.cs
│   ├── Company/
│   │   ├── CompanyProfile.cs
│   │   ├── CompanyProfileLocation.cs
│   │   └── CompanyProfileJobPositionRate.cs
│   ├── Worker/
│   │   ├── WorkerProfile.cs
│   │   ├── WorkerSkill.cs
│   │   ├── WorkerLicense.cs
│   │   └── WorkerCertificate.cs
│   ├── Request/
│   │   ├── Request.cs
│   │   ├── WorkerRequest.cs
│   │   ├── TimeSheet.cs
│   │   └── TimeSheetTotal.cs
│   └── Accounting/
│       ├── PayStub.cs
│       ├── PayStubItem.cs
│       ├── Invoice.cs
│       └── InvoiceTotal.cs
│
├── Enums/                           # Domain enums
│   ├── CompanyStatus.cs
│   ├── RequestStatus.cs
│   ├── WorkerRequestStatus.cs
│   └── ...
│
├── Models/                          # DTOs, ViewModels
│   ├── Requests/
│   ├── Responses/
│   └── ViewModels/
│
├── Interfaces/                      # Service contracts
│   └── IRequestService, IWorkerService, etc
│
├── Repositories/                    # Repository interfaces
│   └── IRequestRepository, IWorkerRepository, etc
│
└── Functionals/                     # Functional types
    ├── Result<T>
    ├── Either<L, R>
    └── Option<T>
```

**Responsabilidades:**
- ✅ Domain entities con business logic
- ✅ Value objects
- ✅ Interfaces (contracts)
- ✅ Domain events
- ❌ NO dependencies externas
- ❌ NO infrastructure concerns

**Publicación:**
- Se publica como NuGet package: `Covenant.Common`
- Consumido por: Covenant.Api, Covenant.IdentityServer

---

#### 5️⃣ SPECIALIZED MODULES

**Covenant.Billing** - Facturación
```
Responsabilidades:
- Cálculo de invoices (rates, markup)
- Tax calculations (HST/GST por provincia)
- Discount calculations
- Additional item calculations

Principales:
- CanadaInvoiceService
- UsaInvoiceService
```

**Covenant.PayStubs** - Nóminas
```
Responsabilidades:
- Generación de pay stubs desde timesheets
- Earnings calculations
- Integración con Covenant.Deductions
- Vacation pay (4% Canadá)
- Public holiday pay

Principales:
- PayStubGenerator
- EarningsCalculator
```

**Covenant.Deductions** - Impuestos Canadienses
```
Responsabilidades:
- CPP (Canada Pension Plan) calculations
- EI (Employment Insurance) calculations
- Federal tax calculations
- Provincial tax calculations (ON, BC, QC, AB, etc)
- Tax tables (actualizadas anualmente)

Tax Tables:
- CppWeekly, CppBiWeekly, CppSemiMonthly, CppMonthly
- FederalTaxWeekly, FederalTaxBiWeekly, etc
- ProvincialTaxWeekly[ON], ProvincialTaxWeekly[BC], etc

Principales:
- CppCalculator
- EiCalculator
- FederalTaxCalculator
- ProvincialTaxCalculator
```

**Covenant.TimeSheetTotal** - Cálculos de Horas
```
Responsabilidades:
- Regular hours vs overtime
- Accumulate weekly hours
- Night shift detection (11 PM - 7 AM)
- Holiday detection
- Break deductions

Principales:
- TimeSheetTotalCalculator
- OvertimeCalculator
```

**Covenant.Documents** - Generación de Documentos
```
Responsabilidades:
- Excel reports (requests, payroll, invoices)
- PDF documents (invoices, pay stubs)
- Azure Storage upload/download
- Email delivery

Principales:
- InvoicePdfGenerator
- PayStubPdfGenerator
- ExcelReportGenerator
```

**Covenant.Notifications** - Notificaciones
```
Responsabilidades:
- Email (SendGrid)
- Push notifications (Azure Notification Hub)
- Teams webhooks
- SMS (futuro)

Principales:
- EmailNotificationService
- PushNotificationService
- TeamsNotificationService
```

---

## 📱 Arquitectura Mobile - SigookApp (Flutter)

### Clean Architecture - 3 Capas

```
lib/
├── features/                        # Features organizados por dominio
│   ├── registration/
│   ├── auth/
│   ├── jobs/
│   └── timesheets/
│
├── core/                            # Core infrastructure
│   ├── config/                      → Environment config
│   ├── network/                     → API client, interceptors
│   ├── routing/                     → GoRouter configuration
│   ├── theme/                       → App theme
│   ├── providers/                   → Core providers
│   └── error/                       → Error handling
│
└── main_staging.dart / main_production.dart
```

### Estructura de un Feature (Clean Architecture)

```
features/registration/
│
├── domain/                          # ← CAPA DE DOMINIO (puro)
│   ├── entities/                    → Business objects
│   │   ├── registration_form.dart
│   │   ├── personal_info.dart
│   │   └── value_objects/           → Type-safe primitives
│   │       ├── email.dart
│   │       ├── phone_number.dart
│   │       └── password.dart
│   │
│   ├── repositories/                → Interfaces (abstractions)
│   │   └── registration_repository.dart
│   │
│   └── usecases/                    → Business logic
│       ├── submit_registration.dart
│       └── validate_section.dart
│
├── data/                            # ← CAPA DE DATOS (implementación)
│   ├── models/                      → DTOs con JSON (Freezed)
│   │   ├── registration_form_model.dart
│   │   └── worker_registration_request.dart
│   │
│   ├── datasources/                 → Fuentes de datos
│   │   ├── registration_local_datasource.dart   # SharedPreferences
│   │   └── registration_remote_datasource.dart  # API calls (Dio)
│   │
│   └── repositories/                → Implementación de interfaces
│       └── registration_repository_impl.dart
│
└── presentation/                    # ← CAPA DE PRESENTACIÓN (UI)
    ├── pages/                       → Full screens
    │   ├── registration_screen.dart
    │   ├── basic_info_page.dart
    │   └── documents_page.dart
    │
    ├── widgets/                     → Reusable components
    │   ├── registration_form_field.dart
    │   └── document_upload_widget.dart
    │
    ├── viewmodels/                  → UI logic (Riverpod StateNotifier)
    │   └── registration_viewmodel.dart
    │
    └── providers/                   → Riverpod providers
        └── registration_providers.dart
```

### Dependencias entre Capas

```
Presentation → Domain ← Data
     ↓           ↓         ↓
   Widgets   Entities  Models
     ↓           ↓         ↓
ViewModels  UseCases  Datasources
     ↓           ↓         ↓
Providers  Repositories  API/Storage
```

**Regla de Oro:**
- Domain NO depende de nada (pure Dart)
- Data implementa interfaces del Domain
- Presentation usa Domain (nunca Data directamente)

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
- Inyecta Bearer token automáticamente
- Maneja 401 (token refresh + retry)

**Environment Config:** `lib/core/config/environment.dart`
- Carga `.env.staging` o `.env.production`
- Expone: AUTH_AUTHORITY, API_BASE_URL, CLIENT_ID, etc

**Routing:** `lib/core/routing/app_router.dart`
- GoRouter con custom transitions
- KeyboardDismissObserver

---

## 🌐 Arquitectura Frontend - Sigook.Web (Vue 2)

### Estructura de Proyecto

```
Sigook.Web/
├── src/
│   ├── assets/                      # Images, styles
│   ├── components/                  # Reusable components
│   ├── pages/                       # Page components
│   ├── router/                      # Vue Router config
│   ├── store/                       # Vuex store modules
│   │   ├── modules/
│   │   │   ├── auth.js
│   │   │   ├── requests.js
│   │   │   ├── workers.js
│   │   │   └── companies.js
│   │   └── index.js
│   ├── security/                    # OIDC authentication
│   ├── utils/                       # Utilities
│   ├── directives/                  # Custom directives
│   ├── filters/                     # Vue filters
│   ├── lang/                        # i18n translations
│   ├── mixins/                      # Vue mixins
│   └── main.js
│
├── public/                          # Static assets
├── wwwroot/                         # Build output (not dist/)
├── Dockerfile                       # Multi-stage build
├── nginx.conf                       # Nginx config
└── vue.config.js
```

**Deployment:**
- Build: Node.js 16 → `npm run staging` o `npm run production`
- Deploy: Docker image con Nginx serving static files

---

## 🔐 Autenticación y Autorización

### Flow Completo

```
┌─────────────────────────────────────────────────────────┐
│ 1. User accede a la aplicación                          │
│    (Web: Sigook.Web, Mobile: SigookApp)                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 2. Redirect a Covenant.IdentityServer                   │
│    sigook-accounts.azurewebsites.net                    │
│    Protocols: OpenID Connect + OAuth 2.0                │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 3. User ingresa credenciales                            │
│    Email + Password                                      │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 4. IdentityServer valida y genera tokens                │
│    - access_token (JWT)                                  │
│    - id_token (user info)                                │
│    - refresh_token                                       │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 5. App almacena tokens                                  │
│    Web: LocalStorage (via oidc-client)                  │
│    Mobile: FlutterSecureStorage (encrypted)             │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 6. Todas las requests a Covenant.Api incluyen token     │
│    Authorization: Bearer {access_token}                  │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│ 7. API valida token y extrae claims                     │
│    - UserId, AgencyId, Roles, Permissions                │
│    - AuthorizationFilter aplica data isolation           │
└─────────────────────────────────────────────────────────┘
```

### Data Isolation (Multi-tenancy)

**Agency Users:**
- Filtro automático: `query.Where(x => x.AgencyId == userAgencyId)`
- Ven solo datos de su agencia

**Company Users:**
- Filtro automático: `query.Where(x => x.CompanyProfileId == userCompanyId)`
- Ven solo datos de su empresa

**Workers:**
- Filtro automático: `query.Where(x => x.WorkerProfileId == userWorkerId)`
- Ven solo sus propios datos

---

## 🚀 Deployment Architecture

### Environments

**Staging:**
- Branch: `dev`
- Auto-deploy on push to dev
- API: `sigook-api-staging.azurewebsites.net`
- Web: `sigook-web-staging.azurewebsites.net`
- Identity: `sigook-accounts-staging.azurewebsites.net`
- Marketing: `covenantgroup-staging.azurewebsites.net`

**Production:**
- Branch: `main`
- Deploy with approval
- API: `sigook-api.azurewebsites.net`
- Web: `sigook.azurewebsites.net`
- Identity: `sigook-accounts.azurewebsites.net`
- Marketing: `covenantgroup.azurewebsites.net`

### CI/CD Pipeline

**Azure DevOps Pipelines:**
- Path-based triggers (solo ejecuta si cambió esa app)
- Templates reutilizables (`.azure-pipelines/templates/`)
- Multi-stage: Build → Test → Deploy
- Docker build para backend
- Node.js build para frontend

Ver: `.azure-pipelines/README.md` para detalles completos

---

## 📊 Database Schema

**Proveedor:** PostgreSQL (cloud-hosted)
**ORM:** Entity Framework Core 6

**Migrations:**
```bash
cd Covenant.Api
dotnet ef migrations add MigrationName --project Covenant.Infrastructure --startup-project Covenant.Api
dotnet ef database update --project Covenant.Infrastructure --startup-project Covenant.Api
```

**Convenciones:**
- Table names: PascalCase (singular)
- Foreign keys: `{Entity}Id`
- Timestamps: `CreatedAt`, `UpdatedAt`
- Soft delete: `DeletedAt` (nullable)

---

## 🔧 Development Commands

### Backend (.NET)
```bash
cd Covenant.Api
dotnet build                         # Build solution
dotnet run --project Covenant.Api    # Run API
dotnet watch run                     # Run with hot reload
dotnet test                          # Run all tests
```

### Web (Vue 2)
```bash
cd Sigook.Web
npm ci                               # Install dependencies
npm run serve                        # Dev server
npm run staging                      # Build staging
npm run production                   # Build production
```

### Mobile (Flutter)
```bash
cd SigookApp
flutter pub get                      # Install dependencies
flutter run --flavor staging -t lib/main_staging.dart
flutter pub run build_runner build --delete-conflicting-outputs
flutter test                         # Run tests
```

---

## 📈 Scalability Considerations

**Current:**
- Monolith API (todos los módulos en un proyecto)
- Single PostgreSQL database
- Azure App Service (horizontal scaling)

**Future Improvements:**
- Microservices (Accounting, Payroll como servicios separados)
- CQRS con Event Sourcing
- Read replicas para reportes
- Redis para caching
- Azure Functions para background jobs
