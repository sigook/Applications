# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is a monorepo containing the Covenant/Sigook platform applications:

- **SigookApp** - Flutter mobile application for worker registration and job matching
- **Sigook.Web** - Vue.js 2 main web application for Sigook platform
- **covenantWeb** - Vue.js 3 marketing/informational website for Covenant
- **Covenant.Api** - .NET 6 API backend for staffing/recruitment management system

## SigookApp (Flutter Mobile Application)

### Architecture

The Flutter app follows **Clean Architecture** with three distinct layers:

1. **Domain Layer** (`lib/features/*/domain/`)
   - Entities: Pure business objects (e.g., `personal_info.dart`, `job.dart`)
   - Repositories: Abstract interfaces defining contracts
   - Use Cases: Single-responsibility business logic encapsulation
   - Value Objects: Type-safe primitives with validation (`email.dart`, `phone_number.dart`, `password.dart`)

2. **Data Layer** (`lib/features/*/data/`)
   - Models: JSON-serializable data transfer objects (DTOs) with Freezed
   - Datasources: Concrete implementations (local: SharedPreferences, remote: API calls)
   - Repositories: Implementations of domain repository interfaces

3. **Presentation Layer** (`lib/features/*/presentation/`)
   - Pages: Full-screen UI components
   - Widgets: Reusable UI components
   - Providers: Riverpod providers for dependency injection and state management
   - ViewModels: UI state management with Riverpod state notifiers

### Key Technologies

- **SDK**: Dart ^3.9.2
- **State Management**: Riverpod (flutter_riverpod ^3.0.3, riverpod_annotation ^3.0.3)
- **Routing**: GoRouter ^17.0.0
- **Immutability**: Freezed ^3.2.3 with freezed_annotation ^3.1.0
- **Functional Programming**: Dartz ^0.10.1 (Either, Option for error handling)
- **Dependency Injection**: Providers pattern via Riverpod (also uses get_it ^9.0.5)
- **HTTP Client**: Dio ^5.7.0 with pretty_dio_logger for debugging
- **Authentication**: OpenID Client with flutter_appauth ^11.0.0
- **Local Storage**: SharedPreferences ^2.2.3, FlutterSecureStorage ^9.2.2 (encrypted on Android)
- **Utilities**: phone_numbers_parser ^9.0.16, mask_text_input_formatter ^2.9.0, table_calendar ^3.1.2
- **File/Image Selection**: file_picker ^10.3.6, image_picker ^1.2.1 with permission_handler ^12.0.1

### Environment Configuration

The app uses **Build Flavors** for environment management:

**Staging:**
```bash
flutter run --flavor staging -t lib/main_staging.dart
flutter build apk --flavor staging -t lib/main_staging.dart
```

**Production:**
```bash
flutter run --flavor production -t lib/main_production.dart
flutter build apk --flavor production -t lib/main_production.dart --release
```

Environment variables are loaded from `.env.staging` and `.env.production` using flutter_dotenv. Configuration is accessed via `lib/core/config/environment.dart` (`EnvironmentConfig` class).

**Build Configuration:**
- Android: `android/app/build.gradle.kts` - Kotlin-based Gradle config with flavors (staging/production), Java 17, Compile SDK 36, MultiDex enabled, OAuth redirect scheme: `sigookcallback`
- iOS: `ios/Runner/Info.plist` - Photo library and camera permissions configured, URL schemes: `sigookcallback`, `com.sigook`

### Code Generation

The app heavily uses code generation. Run this after modifying Freezed models, Riverpod providers, or JSON serializable classes:

```bash
cd SigookApp
flutter pub run build_runner build --delete-conflicting-outputs
```

For continuous generation during development:
```bash
flutter pub run build_runner watch --delete-conflicting-outputs
```

### Feature Organization

Features are organized by domain (e.g., `features/registration/`, `features/auth/`, `features/jobs/`). Each feature follows Clean Architecture layers:

```
features/
  registration/
    data/
      datasources/         # Local and remote data access
      models/              # Freezed DTOs with JSON serialization
      repositories/        # Repository implementations
    domain/
      entities/            # Business objects
      repositories/        # Repository interfaces
      usecases/            # Business logic units
    presentation/
      pages/               # Full screens
      providers/           # Riverpod providers
      viewmodels/          # State management
      widgets/             # Reusable UI components
```

### Core Infrastructure

- **Routing**: `lib/core/routing/app_router.dart` - GoRouter configuration with custom transitions (Fade, Slide) and KeyboardDismissObserver
- **API Client**: `lib/core/network/api_client.dart` - Dio-based HTTP client with 30s timeout
- **Auth Interceptor**: `lib/core/network/auth_interceptor.dart` - Automatic Bearer token injection, handles 401 responses with token refresh and retry mechanism
- **Environment Config**: `lib/core/config/environment.dart` - Loads from `.env.staging` or `.env.production` (AUTH_AUTHORITY, API_BASE_URL, CLIENT_ID, REDIRECT_URI, SCOPES)
- **Theme**: `lib/core/theme/app_theme.dart` - Material theme configuration
- **Providers**: `lib/core/providers/core_providers.dart` - Global providers (SharedPreferences, SecureStorage with Android encryption, ApiClient, NetworkInfo)
- **Error Handling**: `lib/core/error/failures.dart` - Failure types (ServerFailure, NetworkFailure, CacheFailure, ParseFailure, ValidationFailure, PermissionFailure, UserCancelledFailure)

### Development Commands

```bash
# Get dependencies
flutter pub get

# Run app (staging)
flutter run --flavor staging -t lib/main_staging.dart

# Run app (production)
flutter run --flavor production -t lib/main_production.dart

# Generate code (Freezed, Riverpod, JSON)
flutter pub run build_runner build --delete-conflicting-outputs

# Run tests
flutter test

# Analyze code
flutter analyze

# Format code
flutter format lib/
```

### Important Patterns

**Error Handling**: Uses Dartz's `Either<Failure, Success>` pattern. Left side = failure, Right side = success.

**Value Objects**: Domain primitives (Email, Password, PhoneNumber) contain their own validation logic and return `Either<ValueFailure, ValueObject>`.

**Dependency Injection**: Providers are defined in `*_providers.dart` files and override placeholder providers from `core_providers.dart` in `main_common.dart`. Entry points (`main_staging.dart`, `main_production.dart`) load the appropriate `.env` file and call `mainCommon()`.

**State Management**: Riverpod providers manage state. Use `ref.watch()` in widgets to rebuild on changes, `ref.read()` for one-time access.

**Models vs Entities**: Data layer uses Freezed `models` (DTOs) with JSON serialization for API/storage. Domain layer uses pure `entities` (business objects) without serialization. Models are converted to entities when crossing the data→domain boundary.

## Sigook.Web (Vue.js 2 Main Application)

### Architecture

The main Sigook web application built with Vue.js 2 and vue-cli-service.

**Tech Stack:**
- Vue.js 2.6.12
- Vue CLI Service 4.5.19
- Node.js 16
- Vuex 3.0.1 (state management)
- Vue Router 3.0.1
- Axios 1.10.0 (HTTP client)
- OIDC Client 1.5.2 (authentication)
- Buefy 0.9.23 (UI components)
- Docker multi-stage build (Node.js → Nginx)

### Project Structure

```
Sigook.Web/
├── src/
│   ├── assets/         # Images, styles
│   ├── components/     # Reusable Vue components
│   ├── pages/          # Page-level components
│   ├── router/         # Vue Router configuration
│   ├── store/          # Vuex store modules
│   ├── security/       # Authentication logic (OIDC)
│   ├── utils/          # Utility functions
│   ├── directives/     # Custom Vue directives
│   ├── filters/        # Vue filters
│   ├── lang/           # i18n translations (vue-i18n)
│   ├── mixins/         # Vue mixins
│   └── main.js         # Application entry point
├── public/             # Static assets
├── wwwroot/            # Build output directory
├── .env.development.local
├── .env.staging
├── .env.production
├── Dockerfile          # Multi-stage Docker build
├── nginx.conf          # Nginx configuration
└── vue.config.js       # Vue CLI configuration
```

### Environment Configuration

Uses `.env` files for environment-specific configuration:
- `.env.development.local` - Local development
- `.env.staging` - Staging environment
- `.env.production` - Production environment

### Development Commands

```bash
cd Sigook.Web

# Install dependencies
npm ci

# Dev server
npm run serve

# Build for staging
npm run staging

# Build for production
npm run production

# Lint code
npm run lint
```

### Docker Deployment

Multi-stage Dockerfile:
1. **Build Stage** (node:16): Installs dependencies and runs `npm run staging` or `npm run production`
2. **Production Stage** (nginx:stable-alpine): Serves static files from `wwwroot/`

Build argument `ENV` determines which environment to build for:
```bash
docker build --build-arg ENV=staging -t sigook-web .
docker build --build-arg ENV=production -t sigook-web .
```

### Key Features

- **Authentication**: OIDC-based authentication with `oidc-client`
- **Internationalization**: Multi-language support with `vue-i18n`
- **State Management**: Vuex with `vuex-persistedstate` for persistence
- **Form Validation**: `vee-validate` for form validation
- **UI Components**: Buefy (Bulma-based Vue components)
- **Image Handling**: Cropping and compression (`vue-croppa`, `image-compressor.js`)
- **Maps Integration**: Google Maps via `vue2-google-maps`
- **reCAPTCHA**: Form protection with `vue-recaptcha`

### Important Notes

- Output directory is `wwwroot/` (not `dist/`)
- Uses Nginx for serving in production
- Token replacement in `public/**/*.html` and `public/**/*.json` during CI/CD for versioning
- ESLint configured with relaxed rules (warnings for most issues)

## covenantWeb (Vue.js 3 Marketing Website)

### Architecture

Standard Vue 3 + TypeScript + Vite application with component-based architecture.

**Tech Stack:**
- Vue 3.5.22
- TypeScript 5.9.3
- Vue Router 4.6.3
- Pinia 3.0.3 (state management)
- Vite 7.1.11

### Project Structure

```
src/
  assets/          # Images, CSS, static assets
  components/      # Vue components organized by page/feature
    about/
    employers/
    home/
    industries/
    layout/        # Shared layout components (Navbar, Footer, ContactForm)
    licensed/
    partners/
  router/          # Vue Router configuration
  stores/          # Pinia stores
  views/           # Page-level components
```

### Development Commands

```bash
cd covenantWeb

# Install dependencies
npm install

# Dev server
npm run dev

# Type checking
npm run type-check

# Build for staging
npm run build:staging

# Build for production
npm run build:production

# Preview production build
npm run preview

# Lint code
npm run lint
```

### Routes

Configuration: `src/router/index.ts`

- `/` - Home page
- `/licensed-certified` - Licensed & Certified information
- `/about` - About Us
- `/industries` - Industries served
- `/become-partner` - Partner signup
- `/employers` - Employers landing page
- `/talents` - Talents/Workers landing page
- `/open-positions` - Open positions listing

### Important Notes

- Path alias `@` maps to `src/` directory (configured in `vite.config.ts`)
- Components are organized by feature/page for maintainability
- Layout components (MainNavbar, MainFooter, ContactForm) are shared across views

## Git Workflow

**Main branch**: `main`
**Development branch**: `dev`

Recent development focuses on:
- Worker registration form completion
- Splash screen finalization
- Job display and search functionality
- Responsive design improvements

## Node.js Version Requirements

The covenantWeb project requires:
- Node.js ^20.19.0 OR >=22.12.0

Use nvm or similar to manage Node versions if needed.

## Covenant.Api (.NET 6 Backend API)

### Architecture

The Covenant API is a comprehensive staffing/recruitment management system with modular architecture:

**Tech Stack:**
- .NET 6.0 with ASP.NET Core Web API
- PostgreSQL with Entity Framework Core
- Azure Service Bus for messaging
- Azure Storage for file management
- Docker containerization

### Project Structure

```
Covenant.Api/
├── Covenant.Api/              # Main API project
├── Covenant.Common/           # Shared entities, interfaces (NuGet package)
├── Covenant.Infrastructure/   # EF Core, repositories, integrations
├── Covenant.Core.BL/          # Business logic services
├── Covenant.Billing/          # Billing module
├── Covenant.PayStubs/         # Pay stubs generation
├── Covenant.Tests/            # Unit tests
└── Covenant.Integration.Tests/ # Integration tests
```

### Development Commands

```bash
cd Covenant.Api

# Build solution
dotnet build Covenant.Api.sln

# Run API
dotnet run --project Covenant.Api

# Run with watch mode (auto-restart)
dotnet watch run --project Covenant.Api

# Run tests
dotnet test **/Covenant.Tests.csproj        # Unit tests
dotnet test **/Covenant.Integration.Tests.csproj  # Integration tests
dotnet test                                  # All tests

# Database migrations (when changing entity models)
dotnet ef migrations add MigrationName --project Covenant.Infrastructure --startup-project Covenant.Api
```

### Key Features

- **Multi-module Architecture**: Agency, Company, Worker, Accounting modules
- **Canadian Payroll**: Complex tax calculations (CPP, EI, federal/provincial)
- **Document Generation**: Excel reports, PDF invoices/pay stubs
- **Background Processing**: Azure Service Bus for async operations
- **Multi-language**: English/Spanish localization

### Important Notes

- Uses shared cloud PostgreSQL database (no local setup needed)
- All code must be in **English** (American market)
- Publishes NuGet package: `Covenant.Common`
- Deployed as Docker container to Azure App Service

## Azure DevOps Pipelines

The repository uses **path-based triggers** to run pipelines only when relevant files change:

### Pipeline Files

- **`.azure-pipelines/sigookapp-pipeline.yml`** - Flutter app CI/CD (placeholder for future implementation)
- **`.azure-pipelines/sigook-web-pipeline.yml`** - Sigook.Web Vue.js 2 app CI/CD (fully functional)
- **`.azure-pipelines/covenantweb-pipeline.yml`** - CovenantWeb Vue.js 3 marketing CI/CD (fully functional)
- **`.azure-pipelines/covenant-api-pipeline.yml`** - .NET API CI/CD (fully functional)
- **`.azure-pipelines/covenant-common-nuget-pipeline.yml`** - NuGet package CI/CD (fully functional)

### Intelligent Triggering

Each pipeline only executes when its application's files are modified:

```yaml
# Example: Sigook.Web pipeline only triggers on:
paths:
  include:
    - Sigook.Web/**
  exclude:
    - Sigook.Web/**/*.md

# Example: covenantWeb pipeline only triggers on:
paths:
  include:
    - covenantWeb/**
  exclude:
    - covenantWeb/**/*.md

# Example: Covenant.Api pipeline only triggers on:
paths:
  include:
    - Covenant.Api/**
  exclude:
    - Covenant.Api/**/*.md
```

**Benefits:**
- Saves build time (no unnecessary pipeline runs)
- Saves Azure DevOps minutes
- Faster CI/CD feedback
- Independent deployments per application

### Environment Detection

Pipelines automatically detect the environment based on the branch:

| Branch | Environment | Action |
|--------|-------------|--------|
| `main` | Production | Build production, deploy with approval |
| `dev` | Staging | Build staging, auto-deploy |

No duplicate stages - one pipeline handles both environments using conditional variables.

### Pipeline Structure

**CovenantWeb Pipeline (complete):**
1. **Build and Test Job**
   - Install Node.js 20.x with node_modules caching
   - Type checking (vue-tsc)
   - Linting (ESLint)
   - Build for appropriate environment (staging or production)
   - Verify dist/index.html exists
   - Archive and publish artifacts (only on direct push, not PR)

2. **Deploy to Azure Job** (runs after build on non-PR pushes)
   - Downloads build artifacts
   - Deploys to Azure App Service (Linux)
   - Staging: `covenantgroup-staging.azurewebsites.net`
   - Production: `covenantgroup.azurewebsites.net`
   - Runtime: Node.js 20 LTS with `npm start` (serves static files via `serve` package)

**Sigook.Web Pipeline (complete):**
1. **Build and Validate Stage**
   - Install Node.js 16.x with node_modules caching
   - Linting (ESLint)
   - Build validation

2. **Build Docker and Deploy Stage**
   - Replace tokens (version in index.html and version.json)
   - Build Docker image multi-stage (Node.js → Nginx)
   - Push to Azure Container Registry
   - Deploy to Azure App Service Container
   - Staging: `sigook-web-staging.azurewebsites.net`
   - Production: `sigook.azurewebsites.net`

**SigookApp Pipeline (placeholder):**
- Basic project structure validation
- TODO comments for future Flutter build implementation

**Covenant.Api Pipeline (complete):**
1. **Build and Test Stage** (uses templates)
   - Install .NET SDK 6.0 (template: dotnet-setup.yml)
   - Build solution (template: dotnet-build-test.yml)
   - Run unit tests
   - Run integration tests
   - Publish test results

2. **Build Docker and Deploy Stage**
   - Build Docker image (staging or production tag)
   - Push to Azure Container Registry
   - Deploy to Azure App Service
   - Staging: `sigook-api-staging`
   - Production: `sigook-api`

**Covenant.Common NuGet Pipeline (complete):**
1. **Build, Test, and Publish Stage** (dev only, uses templates)
   - Quality Gate: Build + Unit Tests
   - Pack and Publish to Azure Artifacts (sigook/Covenant.Common)
   - Only triggers on changes to Covenant.Api/Covenant.Common/**

### Setup Guide

See `.azure-pipelines/README.md` for detailed setup instructions including:
- Creating pipelines in Azure DevOps
- Configuring environments and approvals
- Setting up variables and secrets
- Deployment options (Azure Static Web Apps, App Service, etc.)
- Testing and troubleshooting

### Quick Commands for Testing Triggers

```bash
# Test Sigook.Web pipeline only
echo "test" >> Sigook.Web/src/App.vue
git add . && git commit -m "test: trigger sigook-web pipeline"
git push origin dev

# Test CovenantWeb pipeline only
echo "test" >> covenantWeb/src/App.vue
git add . && git commit -m "test: trigger covenantweb pipeline"
git push origin dev

# Test SigookApp pipeline only
echo "test" >> SigookApp/lib/main.dart
git add . && git commit -m "test: trigger sigookapp pipeline"
git push origin dev

# Test Covenant.Api pipeline only
echo "// test" >> Covenant.Api/Covenant.Api/Program.cs
git add . && git commit -m "test: trigger covenant-api pipeline"
git push origin dev

# Test Covenant.Common NuGet pipeline only
echo "// test" >> Covenant.Api/Covenant.Common/README.md
git add . && git commit -m "test: trigger covenant-common-nuget pipeline"
git push origin dev

# No pipeline triggered (documentation only)
echo "test" >> README.md
git add . && git commit -m "docs: update readme"
git push origin dev
```
