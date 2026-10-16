/// Feature flags for enabling/disabling app features
///
/// To easily remove mock data functionality:
/// 1. Set `useMockJobs` to false (or delete this entire class)
/// 2. Delete the file: lib/features/jobs/data/mock/mock_jobs_data.dart
/// 3. Remove the conditional mock logic in jobs_viewmodel.dart
class FeatureFlags {
  /// Enable mock jobs fallback when API fails
  /// Set to `true` for development/testing
  /// Set to `false` for production (shows actual errors)
  static const bool useMockJobs = false;

  // Add more feature flags here as needed
  // static const bool enableNewFeature = false;
}
