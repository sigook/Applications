import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/week_time_sheets.dart';

part 'week_time_sheets_model.freezed.dart';
part 'week_time_sheets_model.g.dart';

const _timeSheetStatuses = {1: 'Open', 2: 'Pending', 3: 'Approved', 4: 'Paid'};

Duration _parseDuration(String? value) {
  final parts = (value ?? '').split(':').map((p) => int.tryParse(p.split('.').first) ?? 0).toList();
  if (parts.length < 2) return Duration.zero;
  return Duration(hours: parts[0], minutes: parts[1], seconds: parts.length > 2 ? parts[2] : 0);
}

@freezed
abstract class DayTimeSheetModel with _$DayTimeSheetModel {
  const DayTimeSheetModel._();

  const factory DayTimeSheetModel({
    required String id,
    required DateTime date,
    String? clientName,
    String? jobTitle,
    DateTime? clockIn,
    DateTime? clockOut,
    String? durationBreak,
    @Default(false) bool breakIsPaid,
    @Default(0) double regularHours,
    @Default(0) double overtimeHours,
    @Default(0) double totalHours,
    @Default(1) int status,
    String? missingHours,
    String? missingHoursOvertime,
    @Default(0) double bonusOrOthers,
    String? bonusOrOthersDescription,
    @Default(0) double deductionsOthers,
    String? deductionsOthersDescription,
    @Default(0) double reimbursements,
    String? reimbursementsDescription,
  }) = _DayTimeSheetModel;

  factory DayTimeSheetModel.fromJson(Map<String, dynamic> json) =>
      _$DayTimeSheetModelFromJson(json);

  DayTimeSheet toEntity() => DayTimeSheet(
        id: id,
        date: date,
        clientName: clientName ?? '',
        jobTitle: jobTitle ?? '',
        clockIn: clockIn,
        clockOut: clockOut,
        breakDuration: _parseDuration(durationBreak),
        breakIsPaid: breakIsPaid,
        regularHours: regularHours,
        overtimeHours: overtimeHours,
        totalHours: totalHours,
        status: _timeSheetStatuses[status] ?? 'Open',
        missingHours: _parseDuration(missingHours),
        missingOvertime: _parseDuration(missingHoursOvertime),
        bonus: bonusOrOthers,
        bonusDescription: bonusOrOthersDescription,
        deductions: deductionsOthers,
        deductionsDescription: deductionsOthersDescription,
        reimbursements: reimbursements,
        reimbursementsDescription: reimbursementsDescription,
      );
}

@freezed
abstract class WeekHoursModel with _$WeekHoursModel {
  const factory WeekHoursModel({
    @Default(0) double total,
    @Default(0) double regular,
    @Default(0) double overtime,
    @Default(0) double holiday,
    @Default(0) double pendingApproval,
  }) = _WeekHoursModel;

  factory WeekHoursModel.fromJson(Map<String, dynamic> json) => _$WeekHoursModelFromJson(json);
}

@freezed
abstract class WeekTimeSheetsModel with _$WeekTimeSheetsModel {
  const WeekTimeSheetsModel._();

  const factory WeekTimeSheetsModel({
    required DateTime weekStart,
    @Default(WeekHoursModel()) WeekHoursModel hours,
    @Default([]) List<DayTimeSheetModel> items,
  }) = _WeekTimeSheetsModel;

  factory WeekTimeSheetsModel.fromJson(Map<String, dynamic> json) =>
      _$WeekTimeSheetsModelFromJson(json);

  WeekTimeSheets toEntity() => WeekTimeSheets(
        weekStart: weekStart,
        totalHours: hours.total,
        overtimeHours: hours.overtime,
        pendingApprovalHours: hours.pendingApproval,
        days: items.map((i) => i.toEntity()).toList(),
      );
}
