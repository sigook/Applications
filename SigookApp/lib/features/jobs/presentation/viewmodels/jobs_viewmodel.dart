import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/entities/job.dart';
import '../../domain/usecases/get_jobs.dart';
import '../providers/jobs_providers.dart';
import 'jobs_state.dart';

part 'jobs_viewmodel.g.dart';

@riverpod
class JobsViewModel extends _$JobsViewModel {
  @override
  JobsState build() {
    return const JobsState();
  }

  List<Job> _getMockJobs() {
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

  Future<void> loadJobs() async {
    if (!ref.mounted) return;

    state = state.copyWith(isLoading: true, error: null);

    final getJobs = ref.read(getJobsUseCaseProvider);
    final result = await getJobs(
      const GetJobsParams(
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        final mockJobs = _getMockJobs();
        state = state.copyWith(
          isLoading: false,
          jobs: mockJobs,
          currentPage: 1,
          hasMore: false,
          error: null,
        );
      },
      (paginatedJobs) => state = state.copyWith(
        isLoading: false,
        jobs: paginatedJobs.items,
        currentPage: paginatedJobs.pageIndex,
        hasMore: paginatedJobs.hasMore,
        error: null,
      ),
    );
  }

  Future<void> loadMore() async {
    if (!ref.mounted || state.isLoadingMore || !state.hasMore) return;

    state = state.copyWith(isLoadingMore: true);

    final getJobs = ref.read(getJobsUseCaseProvider);
    final result = await getJobs(
      GetJobsParams(
        sortBy: 0,
        isDescending: false,
        pageIndex: state.currentPage + 1,
        pageSize: 30,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) =>
          state = state.copyWith(isLoadingMore: false, error: failure.message),
      (paginatedJobs) => state = state.copyWith(
        isLoadingMore: false,
        jobs: [...state.jobs, ...paginatedJobs.items],
        currentPage: paginatedJobs.pageIndex,
        hasMore: paginatedJobs.hasMore,
      ),
    );
  }

  Future<void> refresh() async {
    state = const JobsState();
    await loadJobs();
  }
}
