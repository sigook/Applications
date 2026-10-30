import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../../core/constants/error_messages.dart';
import '../../domain/entities/week_time_sheets.dart';
import '../providers/hours_providers.dart';

part 'hours_viewmodel.freezed.dart';
part 'hours_viewmodel.g.dart';

DateTime weekStartOf(DateTime day) {
  final date = DateTime(day.year, day.month, day.day);
  return date.subtract(Duration(days: date.weekday % 7));
}

@freezed
abstract class HoursState with _$HoursState {
  const HoursState._();

  const factory HoursState({
    required DateTime weekStart,
    required DateTime currentWeekStart,
    WeekTimeSheets? week,
    @Default(false) bool isLoading,
    String? error,
  }) = _HoursState;

  bool get isCurrentWeek => !weekStart.isBefore(currentWeekStart);
}

@riverpod
class HoursViewModel extends _$HoursViewModel {
  @override
  HoursState build() {
    final current = weekStartOf(DateTime.now());
    return HoursState(weekStart: current, currentWeekStart: current);
  }

  Future<void> load() async {
    final weekStart = state.weekStart;
    state = state.copyWith(isLoading: true, error: null);
    final result = await ref.read(getWeekTimeSheetsUseCaseProvider)(weekStart);
    if (!ref.mounted || state.weekStart != weekStart) return;
    result.fold(
      (failure) => state = state.copyWith(
        isLoading: false,
        week: null,
        error: ErrorMessages.fromException(Exception(failure.message)),
      ),
      (week) => state = state.copyWith(isLoading: false, week: week),
    );
  }

  Future<void> moveWeek(int direction) async {
    if (direction > 0 && state.isCurrentWeek) return;
    final start = state.weekStart;
    state = state.copyWith(
      weekStart: DateTime(start.year, start.month, start.day + 7 * direction),
      week: null,
    );
    await load();
  }
}
