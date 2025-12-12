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
    return [
      Job(
        id: 'mock-1',
        jobTitle: 'Warehouse Associate',
        numberId: 1001,
        workersQuantity: 5,
        location: 'Downtown Warehouse',
        entrance: 'Main Entrance',
        agencyFullName: 'Sample Agency Co.',
        agencyLogo: null,
        status: 'Open',
        isAsap: true,
        workerApprovedToWork: true,
        workerRate: 18.50,
        workerSalary: 148.0,
        createdAt: now.subtract(const Duration(days: 1)),
        finishAt: now.add(const Duration(hours: 8)),
        startAt: now.add(const Duration(hours: 1)),
        durationTerm: '8 hours',
      ),
      Job(
        id: 'mock-2',
        jobTitle: 'Delivery Driver',
        numberId: 1002,
        workersQuantity: 3,
        location: 'City Center',
        entrance: 'Loading Dock',
        agencyFullName: 'Express Staffing Inc.',
        agencyLogo: null,
        status: 'Open',
        isAsap: false,
        workerApprovedToWork: true,
        workerRate: 20.0,
        workerSalary: 160.0,
        createdAt: now.subtract(const Duration(hours: 12)),
        finishAt: now.add(const Duration(hours: 10)),
        startAt: now.add(const Duration(hours: 2)),
        durationTerm: '8 hours',
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
