# Sigook.App Codebase Structure

Flutter app for workers (Android + iOS). Stack: Dart ^3.9.2, Flutter 3.47.4, Riverpod (`@riverpod` notifiers), Freezed + json_serializable, dartz (`Either<Failure, T>`), Equatable, Dio, go_router, flutter_appauth / native password grant against the Covenant.Api OpenIddict server, flutter_secure_storage.

The source tree follows **Clean Architecture per feature**: every feature owns a `domain/`, `data/` and `presentation/` layer, and `core/` holds the infrastructure shared by all of them. `Sigook.App/CLAUDE.md` is the normative rulebook (layer rules, viewmodel/providers patterns, import depths, naming, checklist); this document is the map.

How to install, run and build the app: `Sigook.App/README.md`.

---

## Project Root

```
Sigook.App/
├── lib/                       # Dart source (see below)
├── test/                      # Unit tests mirroring lib/ (core/, features/, helpers/)
├── integration_test/          # store_screenshots_test.dart: signs in against staging and captures the store screenshots
├── test_driver/               # Host side of `flutter drive`; writes each capture to $SCREENSHOT_DIR
├── scripts/                   # store_screenshots.sh <android|ios> <device> <out-dir>: device prep + flutter drive
├── android/                   # Gradle 9.1 / AGP 9 / Kotlin 2.3; debug manifest allows cleartext to 10.0.2.2
├── ios/                       # Single scheme `Runner`, CocoaPods (SPM disabled in pubspec.yaml)
├── fastlane/                  # Store lanes (see PIPELINES.md); screenshots/ (App Store) and metadata/android/ (Play) hold the committed store screenshots
├── .env.local / .env.staging / .env.production   # Public compile-time config per environment
├── pubspec.yaml               # Date-based version YYYY.M.D+BUILD (CI overrides both numbers)
├── analysis_options.yaml
├── CLAUDE.md                  # Architecture rules
└── README.md                  # Setup, run, build
```

## lib/

```
lib/
├── main_common.dart           # mainCommon(): validates env, wires crash reporting, runApp(MyApp)
├── main_local.dart            # Entry points: one per environment, all call mainCommon()
├── main_staging.dart
├── main_production.dart
│
├── core/
│   ├── config/                # EnvironmentConfig (String.fromEnvironment), FeatureFlags
│   ├── constants/             # ErrorMessages (loaded at startup), app constants
│   ├── error/                 # Exceptions (thrown by datasources) and Failures (returned by repositories)
│   ├── l10n/                  # Localization
│   ├── network/               # ApiClient (Dio) + AuthInterceptor, DioErrorInterceptor, CrashReportingInterceptor, NetworkInfo
│   ├── providers/             # core_providers (sharedPreferences, secureStorage, networkInfo, apiClient), analytics_providers
│   ├── routing/               # AppRouter (go_router), AppRoutes constants, session redirect. Jobs · Activity · Profile are the three branches of a StatefulShellRoute (AppShell bottom bar, each branch keeps its state); job details, About, Privacy and Terms are pushed over it without the bar
│   ├── services/              # FilePicker, FileNaming, Location, Analytics, CrashReporting (Azure App Insights)
│   ├── theme/                 # AppTheme (colors, typography)
│   ├── usecases/              # UseCase<T, Params> base class, NoParams
│   ├── utils/                 # Extensions (date, number, string)
│   └── widgets/               # Shared UI, grouped by kind (see table below)
│
└── features/
    ├── splash/                # Boot screen while the session is restored
    ├── welcome/               # Onboarding page for signed-out users
    ├── auth/                  # Sign-in, logout, token refresh, password reset, resend confirmation
    ├── registration/          # Multi-step worker registration form (value objects: ZipCode, IdentificationNumber)
    ├── jobs/                  # Job list, details (tabs Detail + Punch Card), apply, clock in/out, punch status. A worker with an active assignment (GET /worker/history?status=Active) sees only those assignment cards instead of the open jobs list (a booked worker cannot apply to other jobs); their OPEN PUNCH CARD button opens the job on the Punch Card tab
    ├── activity/              # "Activity" screen: tabs Pay Stubs · Timesheets · History, split into sub-features (see below)
    ├── profile/               # Worker profile, split into self-contained sub-features (see below)
    ├── comments/              # Comments the agency left on the worker (read-only section)
    ├── catalog/               # Read-only catalogs (availabilities, languages, ...)
    ├── app_update/            # Forced-update gate: GET api/mobileapp/version vs. build number, re-checked on resume
    ├── settings/              # Language settings
    ├── legal/                 # Terms and privacy policy pages
    └── about/                 # Static info page
```

## Anatomy of a Feature

```
features/<feature>/
├── domain/
│   ├── entities/              # Plain Dart + Equatable. No JSON, no Flutter imports.
│   ├── repositories/          # Abstract interfaces returning Either<Failure, T>
│   ├── usecases/              # One class per user action
│   └── value_objects/         # Validated primitives (only where needed)
├── data/
│   ├── models/                # Freezed DTOs: fromJson/toJson + toEntity()
│   ├── datasources/           # Abstract + Impl; remote ones use ApiClient
│   └── repositories/          # Implement the domain interface; map exceptions to Failures
└── presentation/
    ├── providers/             # DI wiring: datasource → repository → use cases
    ├── viewmodels/            # @riverpod notifier + @freezed state (.freezed.dart + .g.dart)
    ├── pages/                 # Screens (ConsumerWidget / ConsumerStatefulWidget)
    └── widgets/               # Feature-only widgets
```

`profile/` is the one feature large enough to be split: each section (certificates, contact_info, documents, emergency, job_experience, licenses, personal_details, preferences, resume, sin, ...) is a sub-feature with its own three layers, and `profile/data`, `profile/domain`, `profile/presentation` hold only what they share (`ProfileBaseDatasource`, `guardedProfileCall()`, `WorkerProfile` entity, `cachedWorkerProfileProvider`). Sub-features never import each other. The isolation rules and import depths are in `Sigook.App/CLAUDE.md`.

`activity/` follows the same split: `history/` (`GET /worker/history`, past assignments only: "All" sends `status=Past`, chips Completed/Ended, read-only cards), `pay_stubs/` (`/worker/paystubs`, year selector + YTD net, PDF downloaded to the temp folder and opened through `FileOpenerService` / `open_filex`) and `hours/` (Timesheets tab: `GET /worker/activity/timesheets?weekStart=`, week navigation capped at the current week, tap a day → detail sheet). `activity/data` holds `ActivityBaseDatasource` and `guardedActivityCall()`; `activity/presentation` holds `ActivityPage` (route `/activity`, `/history` redirects; hides Pay Stubs when `WorkerProfile.isUsa`, read from `cachedWorkerProfileProvider`), `ActivityStatusBadge` and `ActivityDetailRow`. Cards and headers mirror the worker portal's mobile cards.

## Data Flow and Error Handling

```
Page / Section  ──ref.watch──▶  ViewModel  ──ref.read──▶  Use Case  ──▶  Repository  ──▶  Datasource  ──▶  ApiClient (Dio)
     ▲                              │
     └── ref.listen (snackbars, navigation) ◀── state (isLoading, error, justSaved)
```

| Layer | Throws / returns | Types |
|-------|------------------|-------|
| Datasource | throws | `ServerException`, `NetworkException`, `CacheException` |
| Repository | catches, returns `Either` | `Left(ServerFailure)`, `Left(NetworkFailure)`, `Left(CacheFailure)`, `Right(value)` |
| Use case / ViewModel | receives `Either` | `fold` into state fields |
| Page | reacts to state | snackbar on `error`, success snackbar on `justSaved` |

The repository is the exception-to-failure boundary. Nothing above it sees an exception; nothing below it sees a `Failure`. Repositories also check `NetworkInfo.isConnected` first and short-circuit to `NetworkFailure`.

## Core Widgets (`lib/core/widgets/`)

Check here before writing a new widget. Everything is re-exported from `core/widgets/widgets.dart`.

| Folder | Widget | Purpose |
|--------|--------|---------|
| `buttons/` | `ActionButton` | Primary/secondary button with loading state |
| | `FilePickerButton` | Opens `FilePickerService` with the allowed extensions |
| `cards/` | `CustomCard` | Base card container |
| | `InfoCard` | Card with icon, title and body |
| | `ProfileSectionCard` | Profile section container: icon, title, gradient, trailing action, children |
| `display/` | `InfoRow` | Label + value row |
| | `ProfileInfoRow` | Label + value row with icon, switches to a text field in edit mode |
| | `ChipDisplayRow` | Row of chips for list values (languages, availabilities) |
| | `StatusBadge` | Colored status pill |
| | `GradientIconContainer` | Rounded gradient box around an icon |
| `feedback/` | `LoadingIndicator`, `LoadingOverlay` | Spinner with message; full-screen overlay variant |
| | `ErrorStateWidget` | Error message with retry button |
| | `EmptyStateWidget` | Empty list placeholder |
| | `EnvironmentBanner` | Orange "Local"/"Staging" banner above every screen; hidden in production |
| | `NetworkStatusBanner` | Offline banner driven by `NetworkInfo` |
| | `ProfileSnackBar` | Success/error snackbar helpers |
| `inputs/` | `CustomTextField`, `CustomDropdown`, `DatePickerField` | Themed form controls |
| `layout/` | `PageScaffold` | Standard page scaffold (app bar, drawer, padding) |
| | `SectionDivider` | Titled divider between sections |
| `navigation/` | `NavbarLogo`, `AppTabBar`, `AppShell` | App logo for app bars; white icon + label tab bar used as `AppBar.bottom` (Job, Activity, Profile); bottom `NavigationBar` (Jobs · Activity · Profile) wrapping the shell route. There is no side menu: About and Logout live in Profile › Account. The Profile destination shows a red badge with the number of missing approval requirements (`ProfileNavIcon`; "not approved to work" does not count and stays only on Jobs) |
| | `DocumentPreviewPage` | In-app viewer for uploaded documents (PDF via `pdfx`, images via `Image.memory`) with a Share action; files are fetched into memory from the public blob URL, written to `tmp/preview/` only when shared, and that folder is cleared on app start |

## Core Services (`lib/core/`)

| Service | Location | Purpose |
|---------|----------|---------|
| `ApiClient` | `network/` | Dio instance; the authenticated variant (`authenticatedApiClientProvider`) attaches the bearer token and refreshes it |
| `NetworkInfo` | `network/` | Connectivity check used by every repository |
| `FilePickerService` | `services/` | Picks a file and validates size and extension |
| `FileNamingService` | `services/` | UUID-based file names for uploads |
| `LocationService` | `services/` | GPS position for clock in/out (punch radius check happens on the API). The live watch and the punch fix (`watchPosition` / `getFreshPosition`) share one stream configuration (`bestForNavigation`, 1 s interval, no distance filter): the geolocator plugin keeps a single position stream per app and hands it to every later caller ignoring their settings, and a distance filter on the watch starves the punch fix on a stationary phone, making the app wrongly report "enable location services" |
| `AnalyticsService` / `CrashReportingService` | `services/` | Azure Application Insights page views, events and uncaught errors |
| `UseCase<T, Params>` | `usecases/` | Base class for single-parameter use cases |

## Environments and Entry Points

The environment is selected by **entry point + `.env` file**, not by flavors (Android declares no `productFlavors`; iOS has one scheme). `EnvironmentConfig` reads every value with `String.fromEnvironment`, so they are compiled into the binary. `validateRequiredConfig()` runs first thing in `mainCommon()` and shows a red "Initialization Failed" screen listing the missing keys when the app is started without `--dart-define-from-file`.

| Environment | Entry point | Config | Banner |
|-------------|-------------|--------|--------|
| Local | `lib/main_local.dart` | `.env.local` (Api on `http://10.0.2.2:5000`, the Android emulator alias for the host) | orange "Local" |
| Staging | `lib/main_staging.dart` | `.env.staging` (`staging.api.sigook.ca`, `staging.accounts.sigook.ca`) | orange "Staging" |
| Production | `lib/main_production.dart` | `.env.production` | none |

Keys: `ENVIRONMENT`, `AUTH_AUTHORITY`, `API_BASE_URL`, `CLIENT_ID`, `REDIRECT_URI`, `POST_LOGOUT_REDIRECT_URI`, `SCOPES`, `APP_NAME`, `APP_INSIGHTS_CONNECTION_STRING`, `GOOGLE_MAPS_API_KEY`. The `.env` files are committed and hold public values only; CI passes the same keys as individual `--dart-define` flags from the `SigookApp-Staging` / `SigookApp-Production` variable groups and is the only place `APP_INSIGHTS_CONNECTION_STRING` and `GOOGLE_MAPS_API_KEY` are set (see `PIPELINES.md`). `GOOGLE_MAPS_API_KEY` feeds the Static Maps image of the job location card (`features/jobs/presentation/widgets/job_location_card.dart`); without it the card keeps the address and the "Open in Maps" button but shows no map.

`SCREENSHOT_MODE=true` (a boolean `--dart-define`, never set in the `.env` files) hides the environment banner so a staging build can be used for store screenshots; `scripts/store_screenshots.sh` is the only caller.

## Adding a Feature — Walkthrough

Reference implementation: any `profile/<sub-feature>/`. Example below: a "Licenses" section.

### 1. Domain

```dart
// domain/entities/worker_license.dart
class WorkerLicense extends Equatable {
  final String? id;
  final String? fileName;
  final String? fileUrl;
  final String? number;
  final String? expires;

  const WorkerLicense({this.id, this.fileName, this.fileUrl, this.number, this.expires});

  @override
  List<Object?> get props => [id, fileName, fileUrl, number, expires];
}

// domain/repositories/license_repository.dart
abstract class LicenseRepository {
  Future<Either<Failure, List<WorkerLicense>>> getLicenses();
  Future<Either<Failure, void>> uploadLicense({required String filePath});
  Future<Either<Failure, void>> deleteLicense(String licenseId);
}

// domain/usecases/upload_license.dart
class UploadLicense implements UseCase<void, UploadLicenseParams> {
  final LicenseRepository repository;
  UploadLicense(this.repository);

  @override
  Future<Either<Failure, void>> call(UploadLicenseParams params) =>
      repository.uploadLicense(filePath: params.filePath);
}

class UploadLicenseParams {
  final String filePath;
  UploadLicenseParams({required this.filePath});
}
```

### 2. Data

```dart
// data/models/license_model.dart
@freezed
abstract class LicenseModel with _$LicenseModel {
  const LicenseModel._();

  const factory LicenseModel({
    String? id,
    String? fileName,
    String? pathFile,
    String? number,
    String? expires,
  }) = _LicenseModel;

  factory LicenseModel.fromJson(Map<String, dynamic> json) => _$LicenseModelFromJson(json);

  WorkerLicense toEntity() =>
      WorkerLicense(id: id, fileName: fileName, fileUrl: pathFile, number: number, expires: expires);
}

// data/datasources/license_remote_datasource.dart
abstract class LicenseRemoteDataSource {
  Future<void> uploadLicense({required String filePath});
}

class LicenseRemoteDataSourceImpl implements LicenseRemoteDataSource {
  final ApiClient apiClient;
  LicenseRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<void> uploadLicense({required String filePath}) async {
    try {
      final fileName = filePath.split(RegExp(r'[/\\]')).last;
      final formData = FormData()
        ..files.add(MapEntry(fileName, await MultipartFile.fromFile(filePath, filename: fileName)));
      final response = await apiClient.dio.post('/worker/profile/licenses', data: formData);
      if (response.statusCode != 200 && response.statusCode != 204) {
        throw ServerException(message: 'Failed: ${response.statusCode}');
      }
    } on DioException catch (e) {
      throw ServerException(message: e.message ?? 'Upload failed');
    }
  }
}

// data/repositories/license_repository_impl.dart
class LicenseRepositoryImpl implements LicenseRepository {
  final LicenseRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;
  LicenseRepositoryImpl({required this.remoteDataSource, required this.networkInfo});

  @override
  Future<Either<Failure, void>> uploadLicense({required String filePath}) async {
    if (!await networkInfo.isConnected) return Left(NetworkFailure());
    try {
      await remoteDataSource.uploadLicense(filePath: filePath);
      return const Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    }
  }
}
```

### 3. Presentation

```dart
// presentation/providers/license_providers.dart
final licenseRemoteDataSourceProvider = Provider<LicenseRemoteDataSource>((ref) =>
    LicenseRemoteDataSourceImpl(apiClient: ref.read(authenticatedApiClientProvider)));

final licenseRepositoryProvider = Provider<LicenseRepository>((ref) => LicenseRepositoryImpl(
      remoteDataSource: ref.read(licenseRemoteDataSourceProvider),
      networkInfo: ref.read(networkInfoProvider),
    ));

final uploadLicenseUseCaseProvider = Provider((ref) => UploadLicense(ref.read(licenseRepositoryProvider)));

// presentation/viewmodels/licenses_viewmodel.dart
@freezed
abstract class LicensesState with _$LicensesState {
  const factory LicensesState({
    @Default(false) bool isUploading,
    String? uploadError,
    @Default(false) bool justUploaded,
  }) = _LicensesState;
}

@riverpod
class LicensesViewModel extends _$LicensesViewModel {
  @override
  LicensesState build() => const LicensesState();

  Future<void> upload(String filePath) async {
    state = state.copyWith(isUploading: true, uploadError: null, justUploaded: false);
    final result = await ref.read(uploadLicenseUseCaseProvider)(UploadLicenseParams(filePath: filePath));
    result.fold(
      (failure) => state = state.copyWith(isUploading: false, uploadError: failure.message),
      (_) {
        state = state.copyWith(isUploading: false, justUploaded: true);
        ref.invalidate(cachedWorkerProfileProvider);
      },
    );
  }
}
```

The page watches the viewmodel and reacts to `uploadError` / `justUploaded` through `ref.listen`; it never calls the use case or the API directly.

### 4. Routing (new screens only)

Add a constant to `AppRoutes` and a `GoRoute` in `lib/core/routing/app_router.dart`. Sections rendered inside an existing page (profile tabs) need no route.

### 5. Generate and verify

```bash
dart run build_runner build --delete-conflicting-outputs
flutter analyze
flutter test
```

Then walk through the feature in the app against staging (`Sigook.App/README.md` → Quick Start) and tick the checklist in `Sigook.App/CLAUDE.md`.
