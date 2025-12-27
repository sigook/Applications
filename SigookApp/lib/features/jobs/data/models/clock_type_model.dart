import 'package:sigook_app_flutter/features/jobs/domain/entities/clock_type.dart';

ClockType clockTypeFromInt(int type) {
  switch (type) {
    case 0:
      return ClockType.none;
    case 1:
      return ClockType.clockIn;
    case 2:
      return ClockType.clockOut;
    default:
      return ClockType.none;
  }
}
