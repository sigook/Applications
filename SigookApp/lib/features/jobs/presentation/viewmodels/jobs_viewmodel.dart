import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/config/feature_flags.dart';
import '../../data/mock/mock_jobs_data.dart';
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
        // ===================================================================
        // MOCK DATA FALLBACK (controlled by FeatureFlags.useMockJobs)
        // To remove: Set FeatureFlags.useMockJobs = false or delete this block
        // ===================================================================
        if (FeatureFlags.useMockJobs) {
          // Load mock jobs instead of showing error
          final mockJobs = MockJobsData.getMockJobs();
          state = state.copyWith(
            isLoading: false,
            jobs: mockJobs,
            currentPage: 1,
            hasMore: false,
            error: null,
          );
        } else {
          // Show user-friendly error message based on failure type
          String errorMessage = _getUserFriendlyErrorMessage(failure.message);
          state = state.copyWith(
            isLoading: false,
            jobs: [],
            currentPage: 0,
            hasMore: false,
            error: errorMessage,
          );
        }
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

  /// Convert technical error messages to user-friendly ones
  String _getUserFriendlyErrorMessage(String technicalError) {
    final lowerError = technicalError.toLowerCase();

    if (lowerError.contains('network') || lowerError.contains('internet')) {
      return 'No internet connection. Please check your network and try again.';
    } else if (lowerError.contains('timeout')) {
      return 'Request timed out. Please check your connection and try again.';
    } else if (lowerError.contains('401') ||
        lowerError.contains('unauthorized')) {
      return 'Your session has expired. Please sign in again.';
    } else if (lowerError.contains('403') || lowerError.contains('forbidden')) {
      return 'You don\'t have permission to access this content.';
    } else if (lowerError.contains('404') || lowerError.contains('not found')) {
      return 'Job listings are currently unavailable. Please try again later.';
    } else if (lowerError.contains('500') || lowerError.contains('server')) {
      return 'Server error. Our team has been notified. Please try again later.';
    } else {
      return 'Unable to load jobs. Please try again.\n\nTechnical details: $technicalError';
    }
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
