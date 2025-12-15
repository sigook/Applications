import '../../domain/entities/job.dart';

/// ============================================================================
/// MOCK JOBS DATA - FOR TESTING/DEVELOPMENT ONLY
/// ============================================================================
///
/// This file contains mock job data for testing purposes.
///
/// TO REMOVE MOCK DATA COMPLETELY:
/// 1. Delete this entire file: lib/features/jobs/data/mock/mock_jobs_data.dart
/// 2. Set FeatureFlags.useMockJobs to false in: lib/core/config/feature_flags.dart
/// 3. Remove mock-related imports and logic from: jobs_viewmodel.dart
///
/// TO TOGGLE MOCK DATA ON/OFF:
/// - Set FeatureFlags.useMockJobs = true (enable mocks as fallback)
/// - Set FeatureFlags.useMockJobs = false (disable mocks, show real errors)
///
/// ============================================================================
class MockJobsData {
  static List<Job> getMockJobs() {
    final now = DateTime.now();
    final nextMonday = now.add(
      Duration(days: (DateTime.monday - now.weekday + 7) % 7),
    );
    final mondayStart = DateTime(
      nextMonday.year,
      nextMonday.month,
      nextMonday.day,
      7,
      0,
    );
    final mondayEnd = DateTime(
      nextMonday.year,
      nextMonday.month,
      nextMonday.day,
      16,
      0,
    );

    return [
      Job(
        id: 'mock-2201',
        jobTitle: 'General Labor - Valdosta GA',
        numberId: 2201,
        workersQuantity: 10,
        location: 'Valdosta, GA',
        entrance: 'Main Entrance',
        agencyFullName: 'MOCA',
        agencyLogo: null,
        status: 'Open',
        isAsap: false,
        workerApprovedToWork: false,
        workerRate: 15.50,
        workerSalary: 124.0,
        createdAt: now.subtract(const Duration(days: 2)),
        finishAt: mondayEnd,
        startAt: mondayStart,
        durationTerm: '6-7 weeks',
      ),
      Job(
        id: 'mock-hr-001',
        jobTitle: 'HR Administrator',
        numberId: 1003,
        workersQuantity: 1,
        location: 'Medley, FL',
        entrance: 'Main Office',
        agencyFullName: 'E-Air',
        agencyLogo: null,
        status: 'Open',
        isAsap: false,
        workerApprovedToWork: false,
        workerRate: 33.17,
        workerSalary: 265.36,
        createdAt: now.subtract(const Duration(days: 5)),
        finishAt: now.add(const Duration(days: 30, hours: 8)),
        startAt: now.add(const Duration(days: 7, hours: 9)),
        durationTerm: 'Full-time',
      ),
    ];
  }
}
