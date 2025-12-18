# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

This is a monorepo containing the Covenant/Sigook platform applications:

- **SigookApp** - Flutter mobile application for worker registration and job matching
- **covenantWeb** - Vue.js marketing/informational website

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

- **State Management**: Riverpod (flutter_riverpod ^3.0.3, riverpod_annotation ^3.0.3)
- **Routing**: GoRouter ^17.0.0
- **Immutability**: Freezed ^3.2.3 with freezed_annotation ^3.1.0
- **Functional Programming**: Dartz ^0.10.1 (Either, Option for error handling)
- **Dependency Injection**: Providers pattern via Riverpod
- **HTTP Client**: Dio ^5.7.0 with pretty_dio_logger for debugging
- **Authentication**: OpenID Client with flutter_appauth ^11.0.0

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

- **Routing**: `lib/core/routing/app_router.dart` - GoRouter configuration with custom transitions
- **API Client**: `lib/core/network/api_client.dart` - Dio-based HTTP client
- **Auth Interceptor**: `lib/core/network/auth_interceptor.dart` - Automatic token injection
- **Theme**: `lib/core/theme/app_theme.dart` - Material theme configuration
- **Providers**: `lib/core/providers/core_providers.dart` - Global providers (SharedPreferences, SecureStorage, ApiClient, NetworkInfo)

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

**Dependency Injection**: Providers are defined in `*_providers.dart` files and override placeholder providers from `core_providers.dart` in `main.dart`.

**State Management**: Riverpod providers manage state. Use `ref.watch()` in widgets to rebuild on changes, `ref.read()` for one-time access.

## covenantWeb (Vue.js Marketing Website)

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

- `/` - Home page
- `/licensed-certified` - Licensed & Certified information
- `/about` - About Us
- `/industries` - Industries served
- `/become-partner` - Partner signup
- `/employers` - Employers landing page

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

## Azure DevOps Pipelines

The repository uses **path-based triggers** to run pipelines only when relevant files change:

### Pipeline Files

- **`.azure-pipelines/sigookapp-pipeline.yml`** - Flutter app CI/CD (placeholder for future implementation)
- **`.azure-pipelines/covenantweb-pipeline.yml`** - Vue.js website CI/CD (fully functional)

### Intelligent Triggering

Each pipeline only executes when its application's files are modified:

```yaml
# Example: covenantWeb pipeline only triggers on:
paths:
  include:
    - covenantWeb/**
  exclude:
    - covenantWeb/**/*.md
```

**Benefits:**
- Saves build time (no unnecessary pipeline runs)
- Saves Azure DevOps minutes
- Faster CI/CD feedback

### Environment Detection

Pipelines automatically detect the environment based on the branch:

| Branch | Environment | Action |
|--------|-------------|--------|
| `main` | Production | Build production, deploy with approval |
| `dev` | Staging | Build staging, auto-deploy |

No duplicate stages - one pipeline handles both environments using conditional variables.

### Pipeline Structure

**CovenantWeb Pipeline (complete):**
1. **CI Stage** - Build & Validate
   - Install dependencies (with caching)
   - Type checking
   - Linting
   - Build for appropriate environment
   - Publish artifacts

2. **CD Stage** - Deploy
   - Download artifacts
   - Deploy to staging or production
   - Requires approval for production environment

**SigookApp Pipeline (placeholder):**
- Basic project structure validation
- TODO comments for future Flutter build implementation

### Setup Guide

See `.azure-pipelines/README.md` for detailed setup instructions including:
- Creating pipelines in Azure DevOps
- Configuring environments and approvals
- Setting up variables and secrets
- Deployment options (Azure Static Web Apps, App Service, etc.)
- Testing and troubleshooting

### Quick Commands for Testing Triggers

```bash
# Test CovenantWeb pipeline only
echo "test" >> covenantWeb/src/App.vue
git add . && git commit -m "test: trigger covenantweb pipeline"
git push origin dev

# Test SigookApp pipeline only
echo "test" >> SigookApp/lib/main.dart
git add . && git commit -m "test: trigger sigookapp pipeline"
git push origin dev

# No pipeline triggered (documentation only)
echo "test" >> README.md
git add . && git commit -m "docs: update readme"
git push origin dev
```
