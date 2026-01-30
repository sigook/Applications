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
    debugPrint('\n🟣 [TIMESHEET VM] ===== BUILD METHOD CALLED =====');
    debugPrint('🟣 [TIMESHEET VM] JobId parameter: $jobId');
    debugPrint('🟣 [TIMESHEET VM] Scheduling loadTimesheetEntries...');

    Future.microtask(() {
      debugPrint(
        '🟣 [TIMESHEET VM] microtask executing, calling loadTimesheetEntries...',
      );
      loadTimesheetEntries();
    });

    debugPrint('🟣 [TIMESHEET VM] Returning initial state (isLoading: true)');
    return const TimesheetState(isLoading: true);
  }

  Future<void> loadTimesheetEntries({bool refresh = false}) async {
    debugPrint('\n🔵 [TIMESHEET] ===== LOADING TIMESHEET ENTRIES =====');
    debugPrint('🔵 [TIMESHEET] JobId: $jobId');
    debugPrint('🔵 [TIMESHEET] Refresh: $refresh');
    debugPrint(
      '🔵 [TIMESHEET] Current state - isLoading: ${state.isLoading}, entries: ${state.entries.length}',
    );

    // Skip only if we're already loading AND we have no entries (prevents duplicate initial loads)
    // OR if we're loading more pages
    if (!refresh && state.isLoadingMore) {
      debugPrint('🔵 [TIMESHEET] Already loading more, skipping...');
      return;
    }

    if (!refresh && state.isLoading && state.entries.isNotEmpty) {
      debugPrint(
        '🔵 [TIMESHEET] Already loading with existing data, skipping...',
      );
      return;
    }

    if (refresh) {
      debugPrint('🔵 [TIMESHEET] Refresh requested, resetting state...');
      state = const TimesheetState(isLoading: true);
    } else if (!state.isLoading) {
      debugPrint('🔵 [TIMESHEET] Setting isLoading to true...');
      state = state.copyWith(isLoading: true, error: null);
    } else {
      debugPrint(
        '🔵 [TIMESHEET] Continuing with initial load (isLoading already true)...',
      );
    }

    final params = GetTimesheetEntriesParams(
      jobId: jobId,
      pageIndex: 1,
      pageSize: 10,
      isDescending: false,
    );

    debugPrint('🔵 [TIMESHEET] Request Params:');
    debugPrint('  - JobId: ${params.jobId}');
    debugPrint('  - PageIndex: ${params.pageIndex}');
    debugPrint('  - PageSize: ${params.pageSize}');
    debugPrint('  - IsDescending: ${params.isDescending}');

    final useCase = ref.read(getTimesheetEntriesUseCaseProvider);
    final result = await useCase(params);

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        debugPrint('❌ [TIMESHEET] FAILURE: ${failure.message}');
        debugPrint('❌ [TIMESHEET] Failure Type: ${failure.runtimeType}');
        state = state.copyWith(isLoading: false, error: failure.message);
      },
      (paginatedTimesheet) {
        debugPrint('✅ [TIMESHEET] SUCCESS!');
        debugPrint('✅ [TIMESHEET] Response Data:');
        debugPrint('  - Total Items: ${paginatedTimesheet.totalItems}');
        debugPrint('  - Total Pages: ${paginatedTimesheet.totalPages}');
        debugPrint('  - Current Page: ${paginatedTimesheet.pageIndex}');
        debugPrint('  - Entries Count: ${paginatedTimesheet.items.length}');

        if (paginatedTimesheet.items.isEmpty) {
          debugPrint('⚠️  [TIMESHEET] API returned EMPTY array');
        } else {
          debugPrint('📋 [TIMESHEET] First Entry:');
          final first = paginatedTimesheet.items.first;
          debugPrint('  - ID: ${first.id}');
          debugPrint('  - Day: ${first.day}');
          debugPrint('  - Was Approved: ${first.wasApproved}');
          debugPrint('  - Total Hours: ${first.totalHours}');
        }

        state = state.copyWith(
          isLoading: false,
          entries: paginatedTimesheet.items,
          currentPage: paginatedTimesheet.pageIndex,
          totalPages: paginatedTimesheet.totalPages,
          totalItems: paginatedTimesheet.totalItems,
          error: null,
        );
        debugPrint('🔵 [TIMESHEET] ===== LOADING COMPLETE =====\n');
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
