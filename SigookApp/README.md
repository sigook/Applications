# Sigook.App.Flutter

A Flutter application built with Clean Architecture, MVVM pattern, and Riverpod state management.

## Architecture Overview

This project follows **Clean Architecture** principles with clear separation of concerns across three layers:

### Layer Structure

1. **Data Layer**
   - Data sources (local, remote)
   - Repositories
   - Use cases

2. **Domain Layer**
   - Use cases
   - Entities
   - Value objects

3. **Presentation Layer**
   - ViewModels
   - Providers
   - Widgets


## Tech Stack

### Core Dependencies
- **flutter_riverpod** (^2.5.1) - State management
- **freezed** (^2.5.7) - Immutable data classes
- **dartz** (^0.10.1) - Functional programming (Either, Option)
- **equatable** (^2.0.5) - Value equality
- **get_it** (^7.7.0) - Dependency injection
- **shared_preferences** (^2.2.3) - Local storage

### Code Generation
- **build_runner** (^2.4.12)
- **freezed_annotation** (^2.4.4)
- **json_serializable** (^6.8.0)

## Architecture Principles

### SOLID Principles
- ✅ **Single Responsibility** - Each class has one reason to change
- ✅ **Open/Closed** - Open for extension, closed for modification
- ✅ **Liskov Substitution** - Interfaces and abstractions
- ✅ **Interface Segregation** - Specific, focused interfaces
- ✅ **Dependency Inversion** - Depend on abstractions, not concrete implementations

### Key Patterns
- **Repository Pattern** - Abstract data sources
- **Use Case Pattern** - Encapsulate business logic
- **MVVM** - Separation of UI and business logic
- **Value Objects** - Type-safe domain primitives with validation

## Development Workflow

### 🐛 Debugging with VS Code

The project includes pre-configured VS Code launch configurations for easy debugging across different environments:

#### Available Debug Configurations
- **Development (Staging)** - Default development with staging environment
- **Staging Environment** - Explicit staging build
- **Production Environment** - Explicit production build
- **Platform-specific variants** for iOS Simulator and Android Emulator

#### How to Debug
1. Open VS Code in the project directory
2. Press `Ctrl+Shift+D` (or `Cmd+Shift+D` on Mac) to open Run & Debug
3. Select your desired configuration from the dropdown
4. Press `F5` or click the green play button

#### Environment Details
- **Staging**: Orange theme, points to staging servers
- **Production**: Clean theme, points to production servers
- Each environment loads its respective `.env` file

See `.vscode/README.md` for detailed configuration information.

### 🚀 Build Flavors

This app uses Flutter **Build Flavors** for environment management:

```bash
# Staging build
flutter run --flavor staging -t lib/main_staging.dart

# Production build  
flutter build apk --flavor production -t lib/main_production.dart --release
```

See `BUILD_FLAVORS_GUIDE.md` for complete setup instructions.
