# SigookApp — Flutter Mobile App

## Code Navigation

```
lib/
├── core/                          Shared infrastructure (no business logic)
│   ├── config/                    Environment, feature flags
│   ├── error/                     Failures, exceptions
│   ├── network/                   ApiClient (Dio), interceptors, NetworkInfo
│   ├── providers/                 core_providers (networkInfo, secureStorage, apiClient)
│   ├── routing/                   app_router (go_router)
│   ├── services/                  FilePicker, FileNaming, Analytics, CrashReporting
│   ├── theme/                     AppTheme (colors, text styles)
│   ├── usecases/                  UseCase<T, Params> base class, NoParams
│   └── widgets/                   Reusable UI: cards, inputs, feedback, display, navigation
│
└── features/
    ├── auth/                      Authentication (sign-in, logout, token refresh)
    ├── catalog/                   Read-only catalog data (availabilities, languages, etc.)
    ├── history/                   Worker assignment history
    ├── jobs/                      Job listing, details, apply, timesheets
    ├── profile/                   Worker profile — split into sub-features (see below)
    ├── registration/              Multi-step worker registration form
    ├── about/                     Static info page
    ├── settings/                  Language settings
    ├── splash/                    Splash screen
    └── welcome/                   Welcome / onboarding page
```

---

## Feature Folder Structure (Standard)

Every feature follows this exact layout. Do not skip layers.

```
features/{feature}/
├── domain/
│   ├── entities/          Plain Dart classes (no Freezed). Extend Equatable.
│   ├── repositories/      Abstract interfaces only. Return Either<Failure, T>.
│   └── usecases/          One class per action. Call UseCase<T,Params> or named-param callable.
├── data/
│   ├── models/            Freezed + json_serializable. Include toEntity() method.
│   ├── datasources/       Abstract + Impl. One class per source (remote / local).
│   └── repositories/      Implements domain interface. Calls datasource, maps exceptions.
└── presentation/
    ├── providers/         All DI wiring in one file: datasource → repo → usecases.
    ├── viewmodels/        @riverpod class + @freezed state. Both .freezed.dart and .g.dart required.
    ├── pages/             Screens. Can contain sub-folders (e.g., tabs, sections).
    └── widgets/           Feature-specific widgets only (see Widget Co-location rules).
```

---

## Profile — Sub-Feature Pattern

`profile/` is large and is split into sub-features. Each sub-feature is **fully self-contained**. This is the reference pattern for any future large feature.

```
profile/
├── {sub-feature}/         (certificates, contact_info, documents, emergency,
│   ├── domain/             job_experience, licenses, personal_details,
│   ├── data/               preferences, resume, sin, ...)
│   └── presentation/
│
├── data/                  Shared base infrastructure for all sub-features
│   ├── datasources/
│   │   ├── profile_base_datasource.dart   Abstract base — extend this in sub-features
│   │   └── profile_remote_datasource.dart  Top-level GET /WorkerProfile/me
│   ├── models/            WorkerProfileModel (Freezed)
│   └── repositories/
│       ├── profile_repository_impl.dart
│       └── profile_repository_helpers.dart  guardedProfileCall() helper
│
├── domain/
│   ├── entities/          WorkerProfile, WorkerLicense, WorkerCertificate, ...
│   └── repositories/
│
└── presentation/
    ├── pages/             Tabs (PersonalDetailsTab, PreferencesTab) + section files
    ├── providers/         cachedWorkerProfileProvider (shared across all sub-features)
    └── widgets/           ONLY widgets shared by 2+ sub-features
```

### Sub-Feature Isolation Rules

A sub-feature (`profile/{sub}/`) may ONLY import from:
- Its own layers: `../../data/`, `../../domain/`
- Parent base infrastructure: `../../../data/datasources/profile_base_datasource.dart`
- Parent helpers: `../../../data/repositories/profile_repository_helpers.dart`
- Parent shared providers: `../../../presentation/providers/cached_worker_profile_provider.dart`
- Core (`core/`) and other top-level features (`auth/`, `catalog/`)

**A sub-feature MUST NOT import from a sibling sub-feature.**
Good: `licenses/` imports `../../../data/datasources/profile_base_datasource.dart`
Bad:  `licenses/` imports `../../certificates/presentation/...`

---

## Clean Architecture Rules

### 1 — Domain Layer (no Flutter, no Dio, no Riverpod)
- **Entities**: plain Dart, extend `Equatable`, add computed getters (formatting, derived values). No `fromJson`.
- **Repositories**: abstract only, return `Either<Failure, T>` from dartz.
- **Use Cases**: one per user action. Either extend `UseCase<T, Params>` (for single-param calls) or use named-param callable class (for multi-field forms). Always return `Either<Failure, T>`.

```dart
// Simple use case (extends base)
class GetJobs extends UseCase<List<Job>, GetJobsParams> { ... }

// Named-param use case (profile style)
class AddJobExperience {
  Future<Either<Failure, void>> call({ required String company, ... }) =>
      repository.add(...);
}
```

### 2 — Data Layer
- **Models**: Freezed + `json_serializable`. Always include `toEntity()`. Place in `data/models/`.
- **Datasources**: one class per data source. Remote datasources use `ApiClient` (always the `authenticatedApiClientProvider` for protected endpoints). Profile sub-feature datasources extend `ProfileBaseDatasource` and use its `execute<T>()` wrapper.
- **Repository impls**: catch `ServerException` and `NetworkException`, map to `Failure`. Use `guardedProfileCall()` helper for void profile calls.

### 3 — Presentation Layer
- **Providers file**: wires the full dependency chain. Order: datasource → repository → use cases. The viewmodel provider is auto-generated by `@riverpod`.
- **ViewModel**: `@riverpod class XViewModel extends _$XViewModel`. State is a `@freezed` class. Logic lives here, never in widgets. Calls use cases via `ref.read(xUseCaseProvider)`. Invalidates cached data after mutations.
- **Pages / Sections**: `ConsumerStatefulWidget` or `ConsumerWidget`. Reads viewmodel via `ref.watch`. Uses `ref.listen` for side-effects (snackbars, navigation). Never calls API directly.
- **Widgets**: purely presentational where possible. Accept callbacks rather than reading providers.

---

## Viewmodel Pattern (Mandatory)

Every feature with mutable state must have a `@freezed` state class and a `@riverpod` notifier. Both generated files (`.freezed.dart`, `.g.dart`) **must exist** — write them manually following the pattern below when `build_runner` has not been run.

```dart
// {name}_viewmodel.dart
@freezed
abstract class XState with _$XState {
  const factory XState({
    @Default(false) bool isLoading,
    String? error,
    @Default(false) bool justSaved,  // triggers snackbar in ref.listen
  }) = _XState;
}

@riverpod
class XViewModel extends _$XViewModel {
  @override
  XState build() => const XState();

  Future<void> save(...) async {
    state = state.copyWith(isLoading: true, error: null, justSaved: false);
    final result = await ref.read(xUseCaseProvider)(...);
    result.fold(
      (failure) => state = state.copyWith(isLoading: false, error: failure.message),
      (_) {
        state = state.copyWith(isLoading: false, justSaved: true);
        ref.invalidate(cachedDataProvider);  // refresh cache after mutation
      },
    );
  }
}
```

State field conventions:
| Field | Purpose |
|-------|---------|
| `isLoading` / `isSaving` / `isUploading` | Disables buttons, shows spinner |
| `error` / `saveError` / `uploadError` | Shown in `ref.listen` as snackbar |
| `justSaved` / `justUploaded` / `justAdded` | One-shot flag for success snackbar |
| `isEditing` / `showForm` | Controls inline edit/form visibility |

---

## Providers File Pattern (Mandatory)

```dart
// {name}_providers.dart
final xDatasourceProvider = Provider<XRemoteDataSource>((ref) {
  return XRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),  // always authenticated
  );
});

final xRepositoryProvider = Provider<XRepository>((ref) {
  return XRepositoryImpl(
    datasource: ref.read(xDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final xUseCaseProvider = Provider<DoX>((ref) {
  return DoX(ref.read(xRepositoryProvider));
});

// FutureProvider for read-only data lists (auto-refetched on invalidate)
final xListProvider = FutureProvider<List<X>>((ref) async {
  final result = await ref.read(xRepositoryProvider).getAll();
  return result.fold((f) => throw Exception(f.message), (list) => list);
});

// XViewModelProvider is auto-generated by @riverpod — do not declare manually.
```

---

## Widget Co-location Rules

| Widget is used by | Location |
|-------------------|----------|
| Only one section/page | Feature's own `presentation/widgets/` |
| Multiple sections within the same feature/sub-feature group | Parent's `presentation/widgets/` |
| Across different top-level features | `core/widgets/` |

Current shared widgets in `profile/presentation/widgets/`:
- `section_edit_actions.dart` — used by 6+ sections
- `document_file_row.dart` — used by 2 sections
- `pending_file_row.dart` — used by 3 sections
- `upload_action_row.dart` — used by 3 sections

---

## Relative Import Path Reference

Relative paths are counted **from the file's containing directory**, not the project root.

| From folder | `../../../../` reaches |
|------------|----------------------|
| `{feature}/data/datasources/` | `lib/features/` |
| `{feature}/data/repositories/` | `lib/features/` |
| `{feature}/domain/repositories/` | `lib/features/` |
| `{feature}/domain/usecases/` | `lib/features/` |
| `{feature}/presentation/providers/` | `lib/features/` |
| `{feature}/presentation/viewmodels/` | `lib/features/` |
| `{feature}/presentation/widgets/` | `lib/features/` |

**Profile sub-feature paths** (one extra level for sub-feature folder):

| From folder | Reaches `profile/` at | Reaches `lib/` at |
|------------|----------------------|-------------------|
| `profile/{sub}/data/datasources/` | `../../../` | `../../../../../` |
| `profile/{sub}/presentation/providers/` | `../../../` | `../../../../../` |
| `profile/{sub}/presentation/widgets/` | `../../../` | `../../../../../` |

**Section files** (`profile/presentation/pages/{tab}/sections/`):

| Depth | Folder |
|-------|--------|
| `../` | `{tab}/` (e.g., `personal_details/`) |
| `../../` | `pages/` |
| `../../../` | `presentation/` |
| `../../../../` | `profile/` ← use for sub-feature imports |
| `../../../../../` | `features/` |
| `../../../../../../` | `lib/` ← use for `core/` imports |

---

## Generated Files (Freezed / Riverpod)

`build_runner` generates `.freezed.dart` and `.g.dart` files. When creating a new feature manually (without running the generator):

1. Copy the closest existing generated file pair (e.g., `licenses_viewmodel.freezed.dart` + `.g.dart`).
2. Global-replace the class name throughout (`LicensesState` → `XState`, `LicensesViewModel` → `XViewModel`).
3. Update field declarations in `_XState` and all `CopyWith` impls to match the new state fields.
4. The hash string in `.g.dart` (`_$xViewModelHash()`) can be any hex-like string — it is used only for debug display.
5. After the feature is complete, run `dart run build_runner build --delete-conflicting-outputs` to regenerate cleanly.

---

## Naming Conventions

| Type | File pattern | Class pattern |
|------|-------------|---------------|
| Entity | `{name}.dart` | `JobExperience` |
| Model (Freezed) | `{name}_model.dart` | `JobExperienceModel` |
| Datasource abstract | `{name}_datasource.dart` | `XDataSource` |
| Datasource impl | same file | `XDataSourceImpl` |
| Repository abstract | `{name}_repository.dart` | `XRepository` |
| Repository impl | `{name}_repository_impl.dart` | `XRepositoryImpl` |
| Use case | `{verb}_{noun}.dart` | `AddJobExperience`, `GetJobs` |
| ViewModel state | `{name}_viewmodel.dart` | `JobExperienceState` |
| ViewModel | same file | `JobExperienceViewModel` |
| Providers | `{name}_providers.dart` | plain `Provider<T>` constants |
| Widget | `{descriptive_name}.dart` | `JobExperienceCard` |
| Section (profile) | `{name}_section.dart` | `JobExperienceSectionCard` |

---

## Checklist for Creating a New Feature

Before marking a feature as done, verify:

- [ ] **Domain**: entity (Equatable), repository interface (Either), use case(s)
- [ ] **Data**: model (Freezed + toEntity), datasource (abstract+impl or extends base), repository impl (catches exceptions, returns Failure)
- [ ] **Presentation**: providers file wires full chain, viewmodel has @freezed state + @riverpod notifier, generated files exist
- [ ] **Widgets**: placed in correct folder per co-location rules
- [ ] **Imports**: relative paths verified at correct depth — no cross-sub-feature imports
- [ ] **Cache invalidation**: viewmodel calls `ref.invalidate(xProvider)` after any mutation
- [ ] **Side effects**: success/error snackbars driven by `ref.listen` on `justSaved`/`error` state fields, never called directly in button handlers
- [ ] **No direct API calls from widgets or pages**: all API calls go through use case → repository → datasource

## When to Split into Sub-Features

Split a feature into sub-features when it has:
- 3 or more independent data resources (each with their own endpoint)
- Independent edit flows per section
- Each section can be saved/updated independently

Pattern: the parent feature holds shared infrastructure (base datasource, helpers, shared entity), each sub-feature is a self-contained module with its own domain/data/presentation layers.

## Commands

```bash
# Run tests
flutter test

# Code generation (Freezed, json_serializable, Riverpod)
dart run build_runner build --delete-conflicting-outputs

# Run app
flutter run
```
