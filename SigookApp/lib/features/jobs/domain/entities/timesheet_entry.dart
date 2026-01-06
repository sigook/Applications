import 'package:equatable/equatable.dart';

class TimesheetEntry extends Equatable {
  final String id;
  final DateTime date;
  final DateTime? startTime;
  final DateTime? endTime;
  final double? workedHours;
  final double? approvedHours;
  final String status;
  final String? notes;
  final String? workerName;

  const TimesheetEntry({
    required this.id,
    required this.date,
    this.startTime,
    this.endTime,
    this.workedHours,
    this.approvedHours,
    required this.status,
    this.notes,
    this.workerName,
  });

  @override
  List<Object?> get props => [
    id,
    date,
    startTime,
    endTime,
    workedHours,
    approvedHours,
    status,
    notes,
    workerName,
  ];
}
