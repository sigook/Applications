import '../../domain/entities/timesheet_entry.dart';

class TimesheetEntryModel extends TimesheetEntry {
  const TimesheetEntryModel({
    required super.id,
    required super.date,
    super.startTime,
    super.endTime,
    super.workedHours,
    super.approvedHours,
    required super.status,
    super.notes,
    super.workerName,
  });

  factory TimesheetEntryModel.fromJson(Map<String, dynamic> json) {
    return TimesheetEntryModel(
      id: json['id'] as String,
      date: DateTime.parse(json['date'] as String),
      startTime: json['startTime'] != null
          ? DateTime.parse(json['startTime'] as String)
          : null,
      endTime: json['endTime'] != null
          ? DateTime.parse(json['endTime'] as String)
          : null,
      workedHours: json['workedHours'] != null
          ? (json['workedHours'] as num).toDouble()
          : null,
      approvedHours: json['approvedHours'] != null
          ? (json['approvedHours'] as num).toDouble()
          : null,
      status: json['status'] as String? ?? 'Unknown',
      notes: json['notes'] as String?,
      workerName: json['workerName'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'date': date.toIso8601String(),
      'startTime': startTime?.toIso8601String(),
      'endTime': endTime?.toIso8601String(),
      'workedHours': workedHours,
      'approvedHours': approvedHours,
      'status': status,
      'notes': notes,
      'workerName': workerName,
    };
  }

  TimesheetEntry toEntity() {
    return TimesheetEntry(
      id: id,
      date: date,
      startTime: startTime,
      endTime: endTime,
      workedHours: workedHours,
      approvedHours: approvedHours,
      status: status,
      notes: notes,
      workerName: workerName,
    );
  }
}
