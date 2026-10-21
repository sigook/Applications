import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/constants/error_messages.dart';
import '../../domain/entities/jobs_filter.dart';
import '../../domain/usecases/get_jobs.dart';
import '../providers/jobs_providers.dart';
import 'jobs_state.dart';

part 'jobs_viewmodel.g.dart';

@riverpod
class JobsViewModel extends _$JobsViewModel {
  static const int _pageSize = 30;

  @override
  JobsState build() {
    return const JobsState();
  }

  Future<void> loadJobs() async {
    if (!ref.mounted) return;

    state = state.copyWith(isLoading: true, error: null);

    final getJobs = ref.read(getJobsUseCaseProvider);
    final result = await getJobs(
      GetJobsParams(filter: state.filter, pageIndex: 1, pageSize: _pageSize),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        String errorMessage = _getUserFriendlyErrorMessage(failure.message);
        state = state.copyWith(
          isLoading: false,
          jobs: [],
          currentPage: 0,
          hasMore: false,
          error: errorMessage,
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

  String _getUserFriendlyErrorMessage(String technicalError) {
    return ErrorMessages.fromException(Exception(technicalError));
  }

  Future<void> loadMore() async {
    if (!ref.mounted || state.isLoadingMore || !state.hasMore) return;

    state = state.copyWith(isLoadingMore: true);

    final getJobs = ref.read(getJobsUseCaseProvider);
    final result = await getJobs(
      GetJobsParams(
        filter: state.filter,
        pageIndex: state.currentPage + 1,
        pageSize: _pageSize,
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

  Future<void> applyFilter(JobsFilter filter) async {
    if (filter == state.filter) return;
    state = state.copyWith(filter: filter);
    await refresh();
  }

  Future<void> clearSearch() => applyFilter(state.filter.copyWith(search: ''));

  Future<void> refresh() async {
    state = state.copyWith(
      isLoadingMore: false,
      error: null,
      currentPage: 1,
      hasMore: true,
    );
    await loadJobs();
  }
}
