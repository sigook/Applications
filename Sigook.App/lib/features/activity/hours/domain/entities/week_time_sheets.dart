import 'package:equatable/equatable.dart';

class DayTimeSheet extends Equatable {
  final String id;
  final DateTime date;
  final String clientName;
  final String jobTitle;
  final DateTime? clockIn;
  final DateTime? clockOut;
  final Duration breakDuration;
  final bool breakIsPaid;
  final double regularHours;
  final double overtimeHours;
  final double totalHours;
  final String status;
  final Duration missingHours;
  final Duration missingOvertime;
  final double bonus;
  final String? bonusDescription;
  final double deductions;
  final String? deductionsDescription;
  final double reimbursements;
  final String? reimbursementsDescription;

  const DayTimeSheet({
    required this.id,
    required this.date,
    required this.clientName,
    required this.jobTitle,
    required this.clockIn,
    required this.clockOut,
    required this.breakDuration,
    required this.breakIsPaid,
    required this.regularHours,
    required this.overtimeHours,
    required this.totalHours,
    required this.status,
    this.missingHours = Duration.zero,
    this.missingOvertime = Duration.zero,
    this.bonus = 0,
    this.bonusDescription,
    this.deductions = 0,
    this.deductionsDescription,
    this.reimbursements = 0,
    this.reimbursementsDescription,
  });

  @override
  List<Object?> get props => [
        id,
        date,
        clientName,
        jobTitle,
        clockIn,
        clockOut,
        breakDuration,
        breakIsPaid,
        regularHours,
        overtimeHours,
        totalHours,
        status,
        missingHours,
        missingOvertime,
        bonus,
        bonusDescription,
        deductions,
        deductionsDescription,
        reimbursements,
        reimbursementsDescription,
      ];
}

class WeekTimeSheets extends Equatable {
  final DateTime weekStart;
  final double totalHours;
  final double overtimeHours;
  final double pendingApprovalHours;
  final List<DayTimeSheet> days;

  const WeekTimeSheets({
    required this.weekStart,
    required this.totalHours,
    required this.overtimeHours,
    required this.pendingApprovalHours,
    required this.days,
  });

  @override
  List<Object?> get props => [weekStart, totalHours, overtimeHours, pendingApprovalHours, days];
}
