import 'package:equatable/equatable.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';

class PunchStatus extends Equatable {
  final ClockType clockType;
  final DateTime? clockIn;
  final DateTime? clockOut;
  final double dayHours;
  final double weekHours;

  const PunchStatus({
    required this.clockType,
    this.clockIn,
    this.clockOut,
    this.dayHours = 0,
    this.weekHours = 0,
  });

  static const none = PunchStatus(clockType: ClockType.none);

  @override
  List<Object?> get props => [clockType, clockIn, clockOut, dayHours, weekHours];
}
