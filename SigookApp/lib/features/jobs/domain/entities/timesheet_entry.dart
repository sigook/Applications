import 'package:equatable/equatable.dart';

class TimesheetEntry extends Equatable {
  final String id;
  final DateTime day;
  final DateTime? clockIn;
  final DateTime? clockOut;
  final DateTime? clockInRounded;
  final DateTime? clockOutRounded;
  final DateTime? timeIn;
  final DateTime? timeOut;
  final int numberId;
  final String? comment;
  final DateTime? timeInApproved;
  final DateTime? timeOutApproved;
  final bool canUpdate;
  final bool wasApproved;
  final String? missingHours;
  final String? missingHoursOvertime;
  final double missingRateWorker;
  final double missingRateAgency;
  final double deductionsOthers;
  final int week;
  final double bonusOrOthers;
  final String? deductionsOthersDescription;
  final String? bonusOrOthersDescription;
  final double reimbursements;
  final String? reimbursementsDescription;
  final double totalHours;
  final double totalHoursApproved;

  const TimesheetEntry({
    required this.id,
    required this.day,
    this.clockIn,
    this.clockOut,
    this.clockInRounded,
    this.clockOutRounded,
    this.timeIn,
    this.timeOut,
    required this.numberId,
    this.comment,
    this.timeInApproved,
    this.timeOutApproved,
    required this.canUpdate,
    required this.wasApproved,
    this.missingHours,
    this.missingHoursOvertime,
    required this.missingRateWorker,
    required this.missingRateAgency,
    required this.deductionsOthers,
    required this.week,
    required this.bonusOrOthers,
    this.deductionsOthersDescription,
    this.bonusOrOthersDescription,
    required this.reimbursements,
    this.reimbursementsDescription,
    required this.totalHours,
    required this.totalHoursApproved,
  });

  @override
  List<Object?> get props => [
    id,
    day,
    clockIn,
    clockOut,
    clockInRounded,
    clockOutRounded,
    timeIn,
    timeOut,
    numberId,
    comment,
    timeInApproved,
    timeOutApproved,
    canUpdate,
    wasApproved,
    missingHours,
    missingHoursOvertime,
    missingRateWorker,
    missingRateAgency,
    deductionsOthers,
    week,
    bonusOrOthers,
    deductionsOthersDescription,
    bonusOrOthersDescription,
    reimbursements,
    reimbursementsDescription,
    totalHours,
    totalHoursApproved,
  ];
}
