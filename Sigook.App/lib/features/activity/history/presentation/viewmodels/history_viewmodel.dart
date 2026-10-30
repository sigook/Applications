import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/constants/error_messages.dart';
import '../../domain/entities/assignment.dart';
import '../providers/history_providers.dart';

part 'history_viewmodel.freezed.dart';
part 'history_viewmodel.g.dart';

const historyStatuses = ['Completed', 'Ended'];
const _pastStatus = 'Past';

@freezed
abstract class AssignmentHistoryState with _$AssignmentHistoryState {
  const factory AssignmentHistoryState({
    @Default([]) List<Assignment> items,
    String? status,
    @Default(false) bool isLoading,
    @Default(false) bool isLoadingMore,
    String? error,
    @Default(1) int currentPage,
    @Default(false) bool hasMore,
  }) = _AssignmentHistoryState;
}

@riverpod
class AssignmentHistoryViewModel extends _$AssignmentHistoryViewModel {
  static const _pageSize = 30;

  @override
  AssignmentHistoryState build() => const AssignmentHistoryState();

  Future<void> load() async {
    state = state.copyWith(isLoading: true, error: null);
    final result = await ref.read(getAssignmentHistoryUseCaseProvider)(
      status: state.status ?? _pastStatus,
      pageSize: _pageSize,
    );
    if (!ref.mounted) return;
    result.fold(
      (failure) => state = state.copyWith(
        isLoading: false,
        items: [],
        hasMore: false,
        error: ErrorMessages.fromException(Exception(failure.message)),
      ),
      (page) => state = state.copyWith(
        isLoading: false,
        items: page.items,
        currentPage: page.pageIndex,
        hasMore: page.hasMore,
      ),
    );
  }

  Future<void> loadMore() async {
    if (state.isLoading || state.isLoadingMore || !state.hasMore) return;
    state = state.copyWith(isLoadingMore: true);
    final result = await ref.read(getAssignmentHistoryUseCaseProvider)(
      status: state.status ?? _pastStatus,
      pageIndex: state.currentPage + 1,
      pageSize: _pageSize,
    );
    if (!ref.mounted) return;
    result.fold(
      (failure) => state = state.copyWith(isLoadingMore: false, hasMore: false),
      (page) => state = state.copyWith(
        isLoadingMore: false,
        items: [...state.items, ...page.items],
        currentPage: page.pageIndex,
        hasMore: page.hasMore,
      ),
    );
  }

  Future<void> filterBy(String? status) async {
    state = AssignmentHistoryState(status: status);
    await load();
  }
}
