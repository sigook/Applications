import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';

class TimesheetResponseModel extends TimesheetResponse {
  const TimesheetResponseModel({
    required super.timeSheetId,
    required super.workerFullName,
    required super.finish,
  });

  factory TimesheetResponseModel.fromJson(Map<String, dynamic> json) {
    return TimesheetResponseModel(
      timeSheetId: json['timeSheetId'] as String,
      workerFullName: json['workerFullName'] as String,
      finish: json['finish'] as bool,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'timeSheetId': timeSheetId,
      'workerFullName': workerFullName,
      'finish': finish,
    };
  }
}
