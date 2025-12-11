import 'package:riverpod_annotation/riverpod_annotation.dart';
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

  Future<void> loadJobs() async {
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

    result.fold(
      (failure) =>
          state = state.copyWith(isLoading: false, error: failure.message),
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
    if (state.isLoadingMore || !state.hasMore) return;

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
