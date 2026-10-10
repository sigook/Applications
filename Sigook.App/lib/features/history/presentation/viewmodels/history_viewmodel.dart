import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/constants/error_messages.dart';
import '../../domain/usecases/get_history.dart';
import '../providers/history_providers.dart';
import 'history_state.dart';

part 'history_viewmodel.g.dart';

@riverpod
class HistoryViewModel extends _$HistoryViewModel {
  @override
  HistoryState build() {
    return const HistoryState();
  }

  Future<void> load() async {
    if (!ref.mounted) return;

    state = state.copyWith(isLoading: true, error: null);

    final getHistory = ref.read(getHistoryUseCaseProvider);
    final result = await getHistory(
      const GetHistoryParams(
        sortBy: 0,
        isDescending: false,
        pageIndex: 1,
        pageSize: 30,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) => state = state.copyWith(
        isLoading: false,
        items: [],
        currentPage: 0,
        hasMore: false,
        error: _friendlyError(failure.message),
      ),
      (paginated) => state = state.copyWith(
        isLoading: false,
        items: paginated.items,
        currentPage: paginated.pageIndex,
        hasMore: paginated.hasMore,
        error: null,
      ),
    );
  }

  Future<void> loadMore() async {
    if (!ref.mounted || state.isLoadingMore || !state.hasMore) return;

    state = state.copyWith(isLoadingMore: true);

    final getHistory = ref.read(getHistoryUseCaseProvider);
    final result = await getHistory(
      GetHistoryParams(
        sortBy: 0,
        isDescending: false,
        pageIndex: state.currentPage + 1,
        pageSize: 30,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) => state = state.copyWith(
        isLoadingMore: false,
        error: failure.message,
      ),
      (paginated) => state = state.copyWith(
        isLoadingMore: false,
        items: [...state.items, ...paginated.items],
        currentPage: paginated.pageIndex,
        hasMore: paginated.hasMore,
      ),
    );
  }

  Future<void> refresh() async {
    state = const HistoryState();
    await load();
  }

  String _friendlyError(String technicalError) {
    return ErrorMessages.fromException(Exception(technicalError));
  }
}
