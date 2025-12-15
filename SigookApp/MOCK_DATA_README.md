# Mock Data Management Guide

This guide explains how to manage mock job data in the application.

## Current Status

Mock data is currently **DISABLED** (set to `false` in feature flags).

When API calls fail, users will see user-friendly error messages instead of mock data.

## Quick Toggle

To enable/disable mock data fallback:

1. Open: `lib/core/config/feature_flags.dart`
2. Change `useMockJobs` value:
   - `true` = Mock jobs load when API fails (for development/testing)
   - `false` = Show real error messages when API fails (for production)

```dart
class FeatureFlags {
  static const bool useMockJobs = false; // Change this value
}
```

## Complete Removal (3 Simple Steps)

To completely remove mock data functionality from the codebase:

### Step 1: Delete Mock Data File
```
Delete: lib/features/jobs/data/mock/mock_jobs_data.dart
```

### Step 2: Remove Feature Flag
```
Delete or set to false: lib/core/config/feature_flags.dart
```

### Step 3: Clean Up ViewModel
Open `lib/features/jobs/presentation/viewmodels/jobs_viewmodel.dart` and:

1. Remove these imports:
   ```dart
   import '../../../../core/config/feature_flags.dart';
   import '../../data/mock/mock_jobs_data.dart';
   ```

2. Delete the entire mock fallback block (lines 36-49):
   ```dart
   // Delete this entire section:
   if (FeatureFlags.useMockJobs) {
     final mockJobs = MockJobsData.getMockJobs();
     state = state.copyWith(
       isLoading: false,
       jobs: mockJobs,
       currentPage: 1,
       hasMore: false,
       error: null,
     );
   } else {
     // Keep this else block, just remove the if/else wrapper
   }
   ```

3. Keep only the error handling logic inside the `failure` fold case.

## Files Involved

- **Feature Flag**: `lib/core/config/feature_flags.dart`
- **Mock Data**: `lib/features/jobs/data/mock/mock_jobs_data.dart`
- **Usage**: `lib/features/jobs/presentation/viewmodels/jobs_viewmodel.dart` (lines 36-49)

## Current Mock Jobs

Two mock jobs are defined:
1. General Labor - Valdosta GA (Order #2201)
2. HR Administrator - Medley, FL (Order #1003)

These are only loaded when `FeatureFlags.useMockJobs = true` AND API calls fail.

---

**Note**: With mocks disabled, users get helpful error messages like:
- "No internet connection. Please check your network and try again."
- "Your session has expired. Please sign in again."
- Etc., with retry and sign-out options.
