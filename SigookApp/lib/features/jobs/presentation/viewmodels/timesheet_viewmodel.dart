import 'package:flutter/foundation.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/entities/timesheet_entry.dart';
import '../../domain/usecases/get_timesheet_entries.dart';
import '../providers/timesheet_providers.dart';

part 'timesheet_viewmodel.freezed.dart';
part 'timesheet_viewmodel.g.dart';

@freezed
sealed class TimesheetState with _$TimesheetState {
  const factory TimesheetState({
    @Default([]) List<TimesheetEntry> entries,
    @Default(false) bool isLoading,
    @Default(false) bool isLoadingMore,
    String? error,
    @Default(1) int currentPage,
    @Default(1) int totalPages,
    @Default(0) int totalItems,
  }) = _TimesheetState;
}

@riverpod
class TimesheetViewModel extends _$TimesheetViewModel {
  @override
  TimesheetState build(String jobId) {
    Future.microtask(() => loadTimesheetEntries());
    return const TimesheetState(isLoading: true);
  }

  Future<void> loadTimesheetEntries({bool refresh = false}) async {
    if (refresh) {
      state = const TimesheetState(isLoading: true);
    } else if (state.isLoading || state.isLoadingMore) {
      return;
    } else {
      state = state.copyWith(isLoading: true, error: null);
    }

    final useCase = ref.read(getTimesheetEntriesUseCaseProvider);
    final result = await useCase(
      GetTimesheetEntriesParams(
        jobId: jobId,
        pageIndex: 1,
        pageSize: 10,
        isDescending: false,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        debugPrint('Failed to load timesheet: ${failure.message}');
        state = state.copyWith(isLoading: false, error: failure.message);
      },
      (paginatedTimesheet) {
        state = state.copyWith(
          isLoading: false,
          entries: paginatedTimesheet.items,
          currentPage: paginatedTimesheet.pageIndex,
          totalPages: paginatedTimesheet.totalPages,
          totalItems: paginatedTimesheet.totalItems,
          error: null,
        );
      },
    );
  }

  Future<void> loadMore() async {
    if (state.isLoadingMore ||
        state.isLoading ||
        state.currentPage >= state.totalPages) {
      return;
    }

    state = state.copyWith(isLoadingMore: true);

    final useCase = ref.read(getTimesheetEntriesUseCaseProvider);
    final result = await useCase(
      GetTimesheetEntriesParams(
        jobId: jobId,
        pageIndex: state.currentPage + 1,
        pageSize: 10,
        isDescending: false,
      ),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        debugPrint('Failed to load more timesheet entries: ${failure.message}');
        state = state.copyWith(isLoadingMore: false);
      },
      (paginatedTimesheet) {
        state = state.copyWith(
          isLoadingMore: false,
          entries: [...state.entries, ...paginatedTimesheet.items],
          currentPage: paginatedTimesheet.pageIndex,
          totalPages: paginatedTimesheet.totalPages,
          totalItems: paginatedTimesheet.totalItems,
        );
      },
    );
  }
}
