import '../../domain/entities/timesheet_entry.dart';

class TimesheetEntryModel extends TimesheetEntry {
  const TimesheetEntryModel({
    required super.id,
    required super.day,
    super.clockIn,
    super.clockOut,
    super.clockInRounded,
    super.clockOutRounded,
    super.timeIn,
    super.timeOut,
    required super.numberId,
    super.comment,
    super.timeInApproved,
    super.timeOutApproved,
    required super.canUpdate,
    required super.wasApproved,
    super.missingHours,
    super.missingHoursOvertime,
    required super.missingRateWorker,
    required super.missingRateAgency,
    required super.deductionsOthers,
    required super.week,
    required super.bonusOrOthers,
    super.deductionsOthersDescription,
    super.bonusOrOthersDescription,
    required super.reimbursements,
    super.reimbursementsDescription,
    required super.totalHours,
    required super.totalHoursApproved,
  });

  factory TimesheetEntryModel.fromJson(Map<String, dynamic> json) {
    return TimesheetEntryModel(
      id: json['id'] as String,
      day: DateTime.parse(json['day'] as String),
      clockIn: json['clockIn'] != null
          ? DateTime.parse(json['clockIn'] as String)
          : null,
      clockOut: json['clockOut'] != null
          ? DateTime.parse(json['clockOut'] as String)
          : null,
      clockInRounded: json['clockInRounded'] != null
          ? DateTime.parse(json['clockInRounded'] as String)
          : null,
      clockOutRounded: json['clockOutRounded'] != null
          ? DateTime.parse(json['clockOutRounded'] as String)
          : null,
      timeIn: json['timeIn'] != null
          ? DateTime.parse(json['timeIn'] as String)
          : null,
      timeOut: json['timeOut'] != null
          ? DateTime.parse(json['timeOut'] as String)
          : null,
      numberId: json['numberId'] as int? ?? 0,
      comment: json['comment'] as String?,
      timeInApproved: json['timeInApproved'] != null
          ? DateTime.parse(json['timeInApproved'] as String)
          : null,
      timeOutApproved: json['timeOutApproved'] != null
          ? DateTime.parse(json['timeOutApproved'] as String)
          : null,
      canUpdate: json['canUpdate'] as bool? ?? false,
      wasApproved: json['wasApproved'] as bool? ?? false,
      missingHours: json['missingHours'] as String?,
      missingHoursOvertime: json['missingHoursOvertime'] as String?,
      missingRateWorker: (json['missingRateWorker'] as num?)?.toDouble() ?? 0.0,
      missingRateAgency: (json['missingRateAgency'] as num?)?.toDouble() ?? 0.0,
      deductionsOthers: (json['deductionsOthers'] as num?)?.toDouble() ?? 0.0,
      week: json['week'] as int? ?? 0,
      bonusOrOthers: (json['bonusOrOthers'] as num?)?.toDouble() ?? 0.0,
      deductionsOthersDescription:
          json['deductionsOthersDescription'] as String?,
      bonusOrOthersDescription: json['bonusOrOthersDescription'] as String?,
      reimbursements: (json['reimbursements'] as num?)?.toDouble() ?? 0.0,
      reimbursementsDescription: json['reimbursementsDescription'] as String?,
      totalHours: (json['totalHours'] as num?)?.toDouble() ?? 0.0,
      totalHoursApproved:
          (json['totalHoursApproved'] as num?)?.toDouble() ?? 0.0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'day': day.toIso8601String(),
      'clockIn': clockIn?.toIso8601String(),
      'clockOut': clockOut?.toIso8601String(),
      'clockInRounded': clockInRounded?.toIso8601String(),
      'clockOutRounded': clockOutRounded?.toIso8601String(),
      'timeIn': timeIn?.toIso8601String(),
      'timeOut': timeOut?.toIso8601String(),
      'numberId': numberId,
      'comment': comment,
      'timeInApproved': timeInApproved?.toIso8601String(),
      'timeOutApproved': timeOutApproved?.toIso8601String(),
      'canUpdate': canUpdate,
      'wasApproved': wasApproved,
      'missingHours': missingHours,
      'missingHoursOvertime': missingHoursOvertime,
      'missingRateWorker': missingRateWorker,
      'missingRateAgency': missingRateAgency,
      'deductionsOthers': deductionsOthers,
      'week': week,
      'bonusOrOthers': bonusOrOthers,
      'deductionsOthersDescription': deductionsOthersDescription,
      'bonusOrOthersDescription': bonusOrOthersDescription,
      'reimbursements': reimbursements,
      'reimbursementsDescription': reimbursementsDescription,
      'totalHours': totalHours,
      'totalHoursApproved': totalHoursApproved,
    };
  }

  TimesheetEntry toEntity() {
    return TimesheetEntry(
      id: id,
      day: day,
      clockIn: clockIn,
      clockOut: clockOut,
      clockInRounded: clockInRounded,
      clockOutRounded: clockOutRounded,
      timeIn: timeIn,
      timeOut: timeOut,
      numberId: numberId,
      comment: comment,
      timeInApproved: timeInApproved,
      timeOutApproved: timeOutApproved,
      canUpdate: canUpdate,
      wasApproved: wasApproved,
      missingHours: missingHours,
      missingHoursOvertime: missingHoursOvertime,
      missingRateWorker: missingRateWorker,
      missingRateAgency: missingRateAgency,
      deductionsOthers: deductionsOthers,
      week: week,
      bonusOrOthers: bonusOrOthers,
      deductionsOthersDescription: deductionsOthersDescription,
      bonusOrOthersDescription: bonusOrOthersDescription,
      reimbursements: reimbursements,
      reimbursementsDescription: reimbursementsDescription,
      totalHours: totalHours,
      totalHoursApproved: totalHoursApproved,
    );
  }
}
