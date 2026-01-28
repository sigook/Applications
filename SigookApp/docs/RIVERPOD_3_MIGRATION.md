# Riverpod 3.0 Migration Strategy

## Overview

This document outlines the strategy for migrating from the current Riverpod implementation to Riverpod 3.0, leveraging new features and best practices.

---

## Current State Analysis

### Current Implementation
- Using `riverpod_annotation` for code generation
- Providers are generated with `@riverpod` annotation
- State management using Freezed classes
- AsyncValue for async operations

### Files Using Riverpod
- **Auth:** `auth_viewmodel.dart`, `auth_providers.dart`
- **Profile:** `profile_viewmodel.dart`, `profile_providers.dart`
- **Jobs:** `jobs_viewmodel.dart`, `jobs_providers.dart`
- **Catalog:** `catalog_notifiers.dart`, `catalog_providers.dart`
- **Registration:** `registration_viewmodel.dart`, `registration_providers.dart`

---

## Riverpod 3.0 New Features

### 1. Enhanced Code Generation
- More efficient code generation
- Better type inference
- Improved hot reload

### 2. New Modifiers
- `.family` for parameterized providers
- `.autoDispose` for automatic cleanup
- `.select` for granular rebuilds

### 3. Better DevTools Integration
- Enhanced provider inspector
- State timeline
- Dependency graph visualization

### 4. Notifier API Improvements
- Simplified notifier lifecycle
- Better error handling
- Automatic dependency tracking

---

## Migration Plan

### Phase 1: Preparation (Low Risk)
**Duration:** 1-2 days

**Tasks:**
1. Update dependencies in `pubspec.yaml`
   ```yaml
   dependencies:
     flutter_riverpod: ^3.0.0
     riverpod_annotation: ^3.0.0
   
   dev_dependencies:
     riverpod_generator: ^3.0.0
     riverpod_lint: ^3.0.0
   ```

2. Run `flutter pub get`
3. Run `flutter pub run build_runner clean`
4. Run `flutter pub run build_runner build --delete-conflicting-outputs`
5. Fix any breaking changes flagged by compiler

**Risk:** Low - Mostly dependency updates

---

### Phase 2: Provider Modernization (Medium Risk)
**Duration:** 2-3 days

**Tasks:**

#### 2.1 Convert Simple Providers

**Before:**
```dart
@riverpod
Future<List<Job>> jobs(JobsRef ref) async {
  final repository = ref.read(jobsRepositoryProvider);
  return repository.getJobs();
}
```

**After (with autoDispose):**
```dart
@riverpod
Future<List<Job>> jobs(JobsRef ref) async {
  ref.keepAlive();  // Or let it auto-dispose
  final repository = ref.read(jobsRepositoryProvider);
  return repository.getJobs();
}
```

#### 2.2 Family Providers

**For parameterized providers:**
```dart
@riverpod
Future<JobDetails> jobDetails(
  JobDetailsRef ref,
  String jobId,
) async {
  final repository = ref.read(jobsRepositoryProvider);
  return repository.getJobDetails(jobId);
}
```

**Risk:** Medium - Requires testing each provider

---

### Phase 3: Notifier Refactoring (Medium Risk)
**Duration:** 3-4 days

**Tasks:**

#### 3.1 Update ViewModel Pattern

**Current:**
```dart
@riverpod
class JobsViewModel extends _$JobsViewModel {
  @override
  JobsState build() {
    return const JobsState();
  }
  
  Future<void> loadJobs() async {
    state = state.copyWith(isLoading: true);
    // ... load logic
  }
}
```

**Enhanced with Riverpod 3.0:**
```dart
@riverpod
class JobsViewModel extends _$JobsViewModel {
  @override
  JobsState build() {
    // Initialize and optionally start loading
    ref.onDispose(() {
      // Cleanup logic
    });
    return const JobsState();
  }
  
  Future<void> loadJobs() async {
    state = state.copyWith(isLoading: true);
    
    // Better error handling with new patterns
    try {
      final jobs = await ref.read(getJobsUseCaseProvider)(params);
      state = state.copyWith(isLoading: false, jobs: jobs);
    } catch (error, stackTrace) {
      // Enhanced error logging with DevTools integration
      ref.read(loggerProvider).logError(error, stackTrace);
      state = state.copyWith(
        isLoading: false,
        error: ErrorMessages.fromException(error),
      );
    }
  }
}
```

**Risk:** Medium - Requires thorough testing

---

### Phase 4: Performance Optimization (Low-Medium Risk)
**Duration:** 2-3 days

**Tasks:**

#### 4.1 Use `.select()` for Granular Rebuilds

**Before:**
```dart
final state = ref.watch(jobsViewModelProvider);
// Rebuilds on any state change
```

**After:**
```dart
final jobs = ref.watch(
  jobsViewModelProvider.select((state) => state.jobs),
);
// Only rebuilds when jobs list changes
```

#### 4.2 Add AutoDispose Where Appropriate

```dart
@riverpod
class CachedData extends _$CachedData {
  @override
  Future<Data> build() async {
    // Auto-dispose when no longer watched
    ref.cacheFor(const Duration(minutes: 5));
    return fetchData();
  }
}
```

#### 4.3 Implement `keepAlive()` for Critical Data

```dart
@riverpod
Future<UserProfile> userProfile(UserProfileRef ref) async {
  ref.keepAlive();  // Never auto-dispose
  return ref.read(profileRepositoryProvider).getProfile();
}
```

**Risk:** Low-Medium - Improves performance but requires careful selection

---

### Phase 5: DevTools Integration (Low Risk)
**Duration:** 1 day

**Tasks:**
1. Enable Riverpod DevTools
2. Add provider names for better debugging
3. Set up provider logging
4. Create provider dependency documentation

**Example:**
```dart
@Riverpod(debugLabel: 'JobsViewModel')
class JobsViewModel extends _$JobsViewModel {
  // ...
}
```

**Risk:** Low - Only enhances debugging

---

### Phase 6: Testing & Validation (Critical)
**Duration:** 3-5 days

**Tasks:**

#### 6.1 Unit Tests
- Test each migrated provider
- Verify state transitions
- Test error handling

#### 6.2 Integration Tests
- Test provider interactions
- Verify dependency injection
- Test lifecycle management

#### 6.3 Manual Testing
- Full app regression testing
- Performance validation
- Memory leak detection

**Example Test:**
```dart
test('JobsViewModel loads jobs successfully', () async {
  final container = ProviderContainer(
    overrides: [
      jobsRepositoryProvider.overrideWithValue(
        MockJobsRepository(),
      ),
    ],
  );
  
  final viewModel = container.read(jobsViewModelProvider.notifier);
  await viewModel.loadJobs();
  
  final state = container.read(jobsViewModelProvider);
  expect(state.isLoading, false);
  expect(state.jobs, isNotEmpty);
});
```

**Risk:** Critical - Must be thorough

---

## Breaking Changes & Fixes

### Common Breaking Changes

#### 1. Provider API Changes
**Before:**
```dart
ref.read(providerProvider)
```

**After:**
```dart
ref.read(providerProvider.future)  // For async providers
```

#### 2. Family Syntax
**Before:**
```dart
@riverpod
Future<Job> job(JobRef ref, {required String id}) async {
  // ...
}
```

**After:**
```dart
@riverpod
Future<Job> job(JobRef ref, String id) async {
  // Named parameters removed from positional family args
}
```

#### 3. StateNotifier Deprecation
If using any StateNotifier:
```dart
// Migrate to Notifier or AsyncNotifier
@riverpod
class MyNotifier extends _$MyNotifier {
  @override
  MyState build() => MyState();
}
```

---

## Rollback Strategy

### If Issues Arise

1. **Revert Dependencies**
   ```bash
   git checkout pubspec.yaml pubspec.lock
   flutter pub get
   ```

2. **Revert Generated Files**
   ```bash
   flutter pub run build_runner build --delete-conflicting-outputs
   ```

3. **Feature Flags**
   - Implement feature flags for gradual rollout
   - Test with small user percentage first

---

## Success Criteria

- [ ] All providers compile without errors
- [ ] All unit tests pass
- [ ] All integration tests pass
- [ ] No performance regressions
- [ ] No memory leaks
- [ ] DevTools shows provider dependencies correctly
- [ ] Hot reload works correctly
- [ ] App functions identically to pre-migration

---

## Timeline Summary

| Phase | Duration | Risk | Priority |
|-------|----------|------|----------|
| 1. Preparation | 1-2 days | Low | High |
| 2. Provider Modernization | 2-3 days | Medium | High |
| 3. Notifier Refactoring | 3-4 days | Medium | High |
| 4. Performance Optimization | 2-3 days | Low-Med | Medium |
| 5. DevTools Integration | 1 day | Low | Low |
| 6. Testing & Validation | 3-5 days | Critical | High |

**Total Estimated Time:** 12-18 days

---

## Resources

- [Riverpod 3.0 Documentation](https://riverpod.dev)
- [Migration Guide](https://riverpod.dev/docs/migration)
- [Best Practices](https://riverpod.dev/docs/concepts/reading)
- [DevTools Setup](https://riverpod.dev/docs/concepts/devtools)

---

## Notes

- **Do NOT migrate all at once** - Migrate feature by feature
- **Start with simplest providers** (e.g., catalog data)
- **End with most complex** (e.g., auth, registration)
- **Maintain backward compatibility** during migration
- **Use feature flags** for progressive rollout

---

**Prepared By:** Sigook Development Team
**Date:** December 2024
**Status:** Planning Phase
