// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'week_time_sheets_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_DayTimeSheetModel _$DayTimeSheetModelFromJson(Map<String, dynamic> json) =>
    _DayTimeSheetModel(
      id: json['id'] as String,
      date: DateTime.parse(json['date'] as String),
      clientName: json['clientName'] as String?,
      jobTitle: json['jobTitle'] as String?,
      clockIn: json['clockIn'] == null
          ? null
          : DateTime.parse(json['clockIn'] as String),
      clockOut: json['clockOut'] == null
          ? null
          : DateTime.parse(json['clockOut'] as String),
      durationBreak: json['durationBreak'] as String?,
      breakIsPaid: json['breakIsPaid'] as bool? ?? false,
      regularHours: (json['regularHours'] as num?)?.toDouble() ?? 0,
      overtimeHours: (json['overtimeHours'] as num?)?.toDouble() ?? 0,
      totalHours: (json['totalHours'] as num?)?.toDouble() ?? 0,
      status: (json['status'] as num?)?.toInt() ?? 1,
      missingHours: json['missingHours'] as String?,
      missingHoursOvertime: json['missingHoursOvertime'] as String?,
      bonusOrOthers: (json['bonusOrOthers'] as num?)?.toDouble() ?? 0,
      bonusOrOthersDescription: json['bonusOrOthersDescription'] as String?,
      deductionsOthers: (json['deductionsOthers'] as num?)?.toDouble() ?? 0,
      deductionsOthersDescription:
          json['deductionsOthersDescription'] as String?,
      reimbursements: (json['reimbursements'] as num?)?.toDouble() ?? 0,
      reimbursementsDescription: json['reimbursementsDescription'] as String?,
    );

Map<String, dynamic> _$DayTimeSheetModelToJson(_DayTimeSheetModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'date': instance.date.toIso8601String(),
      'clientName': instance.clientName,
      'jobTitle': instance.jobTitle,
      'clockIn': instance.clockIn?.toIso8601String(),
      'clockOut': instance.clockOut?.toIso8601String(),
      'durationBreak': instance.durationBreak,
      'breakIsPaid': instance.breakIsPaid,
      'regularHours': instance.regularHours,
      'overtimeHours': instance.overtimeHours,
      'totalHours': instance.totalHours,
      'status': instance.status,
      'missingHours': instance.missingHours,
      'missingHoursOvertime': instance.missingHoursOvertime,
      'bonusOrOthers': instance.bonusOrOthers,
      'bonusOrOthersDescription': instance.bonusOrOthersDescription,
      'deductionsOthers': instance.deductionsOthers,
      'deductionsOthersDescription': instance.deductionsOthersDescription,
      'reimbursements': instance.reimbursements,
      'reimbursementsDescription': instance.reimbursementsDescription,
    };

_WeekHoursModel _$WeekHoursModelFromJson(Map<String, dynamic> json) =>
    _WeekHoursModel(
      total: (json['total'] as num?)?.toDouble() ?? 0,
      regular: (json['regular'] as num?)?.toDouble() ?? 0,
      overtime: (json['overtime'] as num?)?.toDouble() ?? 0,
      holiday: (json['holiday'] as num?)?.toDouble() ?? 0,
      pendingApproval: (json['pendingApproval'] as num?)?.toDouble() ?? 0,
    );

Map<String, dynamic> _$WeekHoursModelToJson(_WeekHoursModel instance) =>
    <String, dynamic>{
      'total': instance.total,
      'regular': instance.regular,
      'overtime': instance.overtime,
      'holiday': instance.holiday,
      'pendingApproval': instance.pendingApproval,
    };

_WeekTimeSheetsModel _$WeekTimeSheetsModelFromJson(Map<String, dynamic> json) =>
    _WeekTimeSheetsModel(
      weekStart: DateTime.parse(json['weekStart'] as String),
      hours: json['hours'] == null
          ? const WeekHoursModel()
          : WeekHoursModel.fromJson(json['hours'] as Map<String, dynamic>),
      items:
          (json['items'] as List<dynamic>?)
              ?.map(
                (e) => DayTimeSheetModel.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
    );

Map<String, dynamic> _$WeekTimeSheetsModelToJson(
  _WeekTimeSheetsModel instance,
) => <String, dynamic>{
  'weekStart': instance.weekStart.toIso8601String(),
  'hours': instance.hours,
  'items': instance.items,
};
