import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/constants/error_messages.dart';
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
