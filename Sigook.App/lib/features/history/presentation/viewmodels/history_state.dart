import 'package:freezed_annotation/freezed_annotation.dart';
import '../../../jobs/domain/entities/job.dart';

part 'history_state.freezed.dart';

@freezed
abstract class HistoryState with _$HistoryState {
  const factory HistoryState({
    @Default([]) List<Job> items,
    @Default(false) bool isLoading,
    @Default(false) bool isLoadingMore,
    String? error,
    @Default(1) int currentPage,
    @Default(true) bool hasMore,
  }) = _HistoryState;
}
