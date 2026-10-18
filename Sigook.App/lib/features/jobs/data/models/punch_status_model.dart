import 'package:sigook_app_flutter/core/utils/enum_converters.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/punch_status.dart';

class PunchStatusModel extends PunchStatus {
  const PunchStatusModel({
    required super.clockType,
    super.clockIn,
    super.clockOut,
    super.dayHours,
    super.weekHours,
  });

  factory PunchStatusModel.fromJson(Map<String, dynamic> json) {
    return PunchStatusModel(
      clockType: clockTypeFromInt(json['clockType'] as int),
      clockIn: json['clockIn'] != null
          ? DateTime.parse(json['clockIn'] as String)
          : null,
      clockOut: json['clockOut'] != null
          ? DateTime.parse(json['clockOut'] as String)
          : null,
      dayHours: (json['dayHours'] as num?)?.toDouble() ?? 0,
      weekHours: (json['weekHours'] as num?)?.toDouble() ?? 0,
    );
  }
}
