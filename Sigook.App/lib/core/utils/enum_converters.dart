import '../constants/enums.dart';

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

int clockTypeToInt(ClockType type) {
  switch (type) {
    case ClockType.none:
      return 0;
    case ClockType.clockIn:
      return 1;
    case ClockType.clockOut:
      return 2;
  }
}
