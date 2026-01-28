import 'package:equatable/equatable.dart';
import 'available_time.dart';
import 'availability_type.dart';
import '../../../../core/constants/enums.dart' as enums;

class AvailabilityInfo extends Equatable {
  final AvailabilityType availabilityType;
  final List<AvailableTime> availableTimes;
  final List<enums.DayOfWeek> availableDays;

  const AvailabilityInfo({
    required this.availabilityType,
    required this.availableTimes,
    required this.availableDays,
  });

  bool get isValid {
    return availabilityType.value.isNotEmpty &&
        availableTimes.isNotEmpty &&
        availableDays.isNotEmpty;
  }

  String? get availabilityTypeError =>
      availabilityType.value.isEmpty ? 'Please select availability type' : null;
  String? get availableTimesError =>
      availableTimes.isEmpty ? 'Please select at least one time slot' : null;
  String? get availableDaysError =>
      availableDays.isEmpty ? 'Please select at least one day' : null;

  AvailabilityInfo copyWith({
    AvailabilityType? availabilityType,
    List<AvailableTime>? availableTimes,
    List<enums.DayOfWeek>? availableDays,
  }) {
    return AvailabilityInfo(
      availabilityType: availabilityType ?? this.availabilityType,
      availableTimes: availableTimes ?? this.availableTimes,
      availableDays: availableDays ?? this.availableDays,
    );
  }

  @override
  List<Object?> get props => [availabilityType, availableTimes, availableDays];
}
