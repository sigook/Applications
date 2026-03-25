# Sigook.App.Flutter

A Flutter application built with Clean Architecture, MVVM pattern, and Riverpod state management.

## Architecture Overview

This project follows **Clean Architecture** principles with clear separation of concerns across three layers:

### Layer Structure

```text
lib/
├── core/                    # Shared infrastructure
│   ├── config/              # App configuration
│   ├── constants/           # App-wide constants
│   ├── error/               # Failure & Exception classes
│   ├── l10n/                # Localization
│   ├── network/             # ApiClient (Dio), NetworkInfo
│   ├── providers/           # Shared Riverpod providers
│   ├── routing/             # GoRouter setup (app_router.dart)
│   ├── services/            # FilePickerService, FileNamingService
│   ├── theme/               # AppTheme (colors, typography)
│   ├── usecases/            # Base UseCase<T, Params> interface
│   ├── utils/               # Utility helpers
│   └── widgets/             # Reusable widgets (ProfileSectionCard, ProfileInfoRow, etc.)
│
├── features/                # Feature modules (one folder per feature)
│   ├── auth/
│   ├── profile/
│   ├── registration/
│   ├── jobs/
│   ├── history/
│   ├── catalog/
│   └── ...
│
├── main_staging.dart        # Staging entry point
└── main_production.dart     # Production entry point
```

Each **feature** follows the same 3-layer structure:

```text
features/<feature>/
├── domain/
│   ├── entities/            # Business objects (Equatable classes)
│   ├── repositories/        # Abstract repository interfaces
│   ├── usecases/            # One class per use case
│   └── value_objects/       # Type-safe domain primitives (if needed)
│
├── data/
│   ├── models/              # Freezed DTOs (fromJson/toJson + toEntity())
│   ├── datasources/         # Abstract + Impl for remote/local data access
│   └── repositories/        # Concrete repository implementations
│
└── presentation/
    ├── pages/               # Full-screen widgets (ConsumerStatefulWidget)
    ├── widgets/             # Feature-specific reusable widgets
    ├── viewmodels/          # Riverpod Notifiers (Freezed state classes)
    └── providers/           # Riverpod providers (DI wiring)
```

## Tech Stack

### Core Dependencies
- **flutter_riverpod** (^2.5.1) - State management
- **freezed** (^2.5.7) - Immutable data classes & code generation
- **dartz** (^0.10.1) - Functional programming (`Either<Failure, T>`)
- **equatable** (^2.0.5) - Value equality for entities
- **dio** - HTTP client
- **go_router** - Declarative routing
- **flutter_appauth** - OIDC authentication
- **shared_preferences** (^2.2.3) - Local storage

### Code Generation
- **build_runner** (^2.4.12)
- **freezed_annotation** (^2.4.4)
- **json_serializable** (^6.8.0)

```bash
flutter pub run build_runner build --delete-conflicting-outputs
```

## Adding a New Feature (Step-by-Step)

Use the **Profile** feature as the reference implementation. This walkthrough uses a hypothetical "Licenses" section as an example.

### 1. Domain Layer (define what, not how)

**Entity** — `lib/features/<feature>/domain/entities/<name>.dart`

```dart
import 'package:equatable/equatable.dart';

class WorkerLicenseItem extends Equatable {
  final String? id;
  final String? fileName;
  final String? fileUrl;
  final String? number;
  final String? expires;

  const WorkerLicenseItem({this.id, this.fileName, this.fileUrl, this.number, this.expires});

  @override
  List<Object?> get props => [id, fileName, fileUrl, number, expires];
}
```

> Entities are plain Dart + Equatable. No JSON, no Freezed, no framework imports.

**Repository interface** — `lib/features/<feature>/domain/repositories/<name>_repository.dart`

```dart
import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';

abstract class LicenseRepository {
  Future<Either<Failure, List<WorkerLicenseItem>>> getLicenses();
  Future<Either<Failure, void>> uploadLicense({required String filePath, ...});
  Future<Either<Failure, void>> deleteLicense(String licenseId);
}
```

> Always return `Either<Failure, T>`. Never throw from repositories.

**Use case** — `lib/features/<feature>/domain/usecases/<name>.dart`

```dart
import '../../../../core/usecases/usecase.dart';

class UploadLicense implements UseCase<void, UploadLicenseParams> {
  final LicenseRepository repository;
  UploadLicense(this.repository);

  @override
  Future<Either<Failure, void>> call(UploadLicenseParams params) async {
    return await repository.uploadLicense(filePath: params.filePath);
  }
}

class UploadLicenseParams {
  final String filePath;
  UploadLicenseParams({required this.filePath});
}
```

> One use case = one action. Extend `UseCase<ReturnType, Params>` from `core/usecases/usecase.dart`.

### 2. Data Layer (the how)

**Freezed Model** — `lib/features/<feature>/data/models/<name>_model.dart`

```dart
import 'package:freezed_annotation/freezed_annotation.dart';

part '<name>_model.freezed.dart';
part '<name>_model.g.dart';

@freezed
abstract class LicenseModel with _$LicenseModel {
  const LicenseModel._(); // needed for custom methods

  const factory LicenseModel({
    String? id,
    String? fileName,
    String? pathFile,
    String? number,
    String? expires,
  }) = _LicenseModel;

  factory LicenseModel.fromJson(Map<String, dynamic> json) =>
      _$LicenseModelFromJson(json);

  // Convert to domain entity
  WorkerLicenseItem toEntity() => WorkerLicenseItem(
    id: id,
    fileName: fileName,
    fileUrl: pathFile,
    number: number,
    expires: expires,
  );
}
```

> Models are Freezed. They handle JSON and convert to entities via `toEntity()`. Run `build_runner` after creating/modifying.

**Remote Datasource** — `lib/features/<feature>/data/datasources/<name>_remote_datasource.dart`

```dart
// 1. Define abstract interface
abstract class LicenseRemoteDataSource {
  Future<void> uploadLicense(String workerId, {required String filePath});
  Future<void> deleteLicense(String workerId, String licenseId);
}

// 2. Implement with Dio
class LicenseRemoteDataSourceImpl implements LicenseRemoteDataSource {
  final ApiClient apiClient;
  LicenseRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<void> uploadLicense(String workerId, {required String filePath}) async {
    try {
      final fileName = filePath.split(RegExp(r'[/\\]')).last;
      final formData = FormData();
      formData.fields.add(MapEntry('data', jsonEncode({...})));
      formData.files.add(MapEntry(
        fileName,
        await MultipartFile.fromFile(filePath, filename: fileName),
      ));

      final response = await apiClient.dio.post(
        '/WorkerProfile/$workerId/Licenses',
        data: formData,
      );

      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(message: 'Failed: ${response.statusCode}');
      }
    } on DioException catch (e) {
      // Convert to ServerException or NetworkException
    }
  }
}
```

> Datasources throw **exceptions** (`ServerException`, `NetworkException`). The repository catches them and returns `Failure`.

**Repository Implementation** — `lib/features/<feature>/data/repositories/<name>_repository_impl.dart`

```dart
class LicenseRepositoryImpl implements LicenseRepository {
  final LicenseRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  LicenseRepositoryImpl({required this.remoteDataSource, required this.networkInfo});

  @override
  Future<Either<Failure, void>> uploadLicense({required String filePath}) async {
    if (!await networkInfo.isConnected) return Left(NetworkFailure());
    try {
      final workerId = await ...;
      await remoteDataSource.uploadLicense(workerId, filePath: filePath);
      return Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }
}
```

> The repository is the **exception-to-failure boundary**. Everything above (use cases, UI) only sees `Either<Failure, T>`.

### 3. Presentation Layer (UI + state)

**Providers** — `lib/features/<feature>/presentation/providers/<name>_providers.dart`

```dart
import 'package:flutter_riverpod/flutter_riverpod.dart';

final licenseRemoteDataSourceProvider = Provider((ref) =>
  LicenseRemoteDataSourceImpl(apiClient: ref.read(apiClientProvider)));

final licenseRepositoryProvider = Provider((ref) =>
  LicenseRepositoryImpl(
    remoteDataSource: ref.read(licenseRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  ));

final uploadLicenseUseCaseProvider = Provider((ref) =>
  UploadLicense(ref.read(licenseRepositoryProvider)));
```

> Providers wire up the dependency graph. Each layer gets its own provider.

**Page** — `lib/features/<feature>/presentation/pages/<name>_page.dart`

```dart
class LicensesSection extends ConsumerStatefulWidget { ... }

class _LicensesSectionState extends ConsumerState<LicensesSection> {
  PickedFileData? _pendingFile;
  bool _isUploading = false;

  Future<void> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png']);
    if (!result.isSuccess || result.file == null) return;
    setState(() => _pendingFile = result.file);
  }

  Future<void> _upload() async {
    setState(() => _isUploading = true);
    final useCase = ref.read(uploadLicenseUseCaseProvider);
    final result = await useCase(UploadLicenseParams(filePath: _pendingFile!.path));

    if (!mounted) return;
    setState(() => _isUploading = false);

    result.fold(
      (failure) => ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error: ${failure.message}'), backgroundColor: AppTheme.errorRed),
      ),
      (_) {
        setState(() => _pendingFile = null);
        ref.invalidate(cachedWorkerProfileProvider); // refresh data
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Uploaded!'), backgroundColor: AppTheme.successGreen),
        );
      },
    );
  }
}
```

### 4. Routing (if adding a new page)

**File:** `lib/core/routing/app_router.dart`

```dart
// Add route constant
static const myNewPage = '/my-new-page';

// Add to GoRouter routes list
GoRoute(
  path: AppRoutes.myNewPage,
  builder: (context, state) => const MyNewPage(),
),
```

> If the feature is a section within an existing page (like profile sections), no route changes needed.

### 5. Run Code Generation

```bash
flutter pub run build_runner build --delete-conflicting-outputs
```

### 6. Verify

```bash
flutter analyze                # Check for static errors
flutter test                   # Run unit tests
flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart  # Manual test
```

## Reusable Core Widgets

Before building custom UI, check what already exists in `lib/core/widgets/`:

| Widget               | Purpose                                                                      |
| -------------------- | ---------------------------------------------------------------------------- |
| `ProfileSectionCard` | Card container with icon, title, gradient, trailing action, and children     |
| `ProfileInfoRow`     | Label + value row with icon, supports edit mode with `TextEditingController` |
| `LoadingIndicator`   | Centered loading spinner with message                                        |
| `ErrorStateWidget`   | Error display with retry button                                              |
| `NavbarLogo`         | App logo for navigation bars                                                 |

## Reusable Core Services

| Service              | Location         | Purpose                                        |
| -------------------- | ---------------- | ---------------------------------------------- |
| `ApiClient`          | `core/network/`  | Dio instance with auth interceptors            |
| `NetworkInfo`        | `core/network/`  | Connectivity checking                          |
| `FilePickerService`  | `core/services/` | File picking with validation (size, ext)       |
| `FileNamingService`  | `core/services/` | UUID-based file name generation                |
| `UseCase<T, Params>` | `core/usecases/` | Base class for all use cases                   |

## Error Handling Convention

```text
Datasource (throws)     →  Repository (catches)    →  Use Case / UI (receives)
ServerException          →  ServerFailure            →  Either<Failure, T>
NetworkException         →  NetworkFailure
CacheException           →  CacheFailure
```

- **Datasources** throw exceptions (`ServerException`, `NetworkException`, etc.)
- **Repositories** catch exceptions and return `Left(Failure)` or `Right(value)`
- **UI** folds the `Either` to show success/error

## Architecture Principles

### SOLID Principles

- **Single Responsibility** - Each class has one reason to change
- **Open/Closed** - Open for extension, closed for modification
- **Liskov Substitution** - Interfaces and abstractions
- **Interface Segregation** - Specific, focused interfaces
- **Dependency Inversion** - Depend on abstractions, not concrete implementations

### Key Patterns

- **Repository Pattern** - Abstract data sources behind interfaces
- **Use Case Pattern** - One class per business action
- **MVVM** - Separation of UI and business logic via Riverpod
- **Value Objects** - Type-safe domain primitives with validation

## Development Workflow

### Debugging with VS Code

Pre-configured launch configurations:

- **Development (Staging)** - Default development with staging environment
- **Staging Environment** - Explicit staging build
- **Production Environment** - Explicit production build
- **Platform-specific variants** for iOS Simulator and Android Emulator

1. Open VS Code, press `Ctrl+Shift+D` (`Cmd+Shift+D` on Mac)
2. Select configuration from the dropdown
3. Press `F5`

- **Staging**: Orange theme, points to staging servers
- **Production**: Clean theme, points to production servers
- Each environment loads its respective `.env` file

See `.vscode/README.md` for detailed configuration.

### Build Environments

```bash
# Local (localhost services)
flutter run --dart-define-from-file=.env.local -t lib/main_local.dart

# Staging
flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart

# Production
flutter build apk --dart-define-from-file=.env.production -t lib/main_production.dart --release
```
