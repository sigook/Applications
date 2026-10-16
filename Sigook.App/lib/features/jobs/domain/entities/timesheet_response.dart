import 'package:equatable/equatable.dart';

class TimesheetResponse extends Equatable {
  final String timeSheetId;
  final String workerFullName;
  final bool finish;

  const TimesheetResponse({
    required this.timeSheetId,
    required this.workerFullName,
    required this.finish,
  });

  @override
  List<Object?> get props => [timeSheetId, workerFullName, finish];
}
