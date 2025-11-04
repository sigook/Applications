import 'package:equatable/equatable.dart';
import '../../../../core/constants/enums.dart';

/// Availability information entity
/// Represents the availability section of the registration form
class AvailabilityInfo extends Equatable {
  final AvailabilityType availabilityType;
  final List<TimeOfDay> availableTimes;
  final List<DayOfWeek> availableDays;

  const AvailabilityInfo({
    required this.availabilityType,
    required this.availableTimes,
    required this.availableDays,
  });

  /// Validates all fields
  bool get isValid {
    return availableTimes.isNotEmpty && availableDays.isNotEmpty;
  }

  /// Validation error messages
  String? get availableTimesError =>
      availableTimes.isEmpty ? 'Please select at least one time slot' : null;
  String? get availableDaysError =>
      availableDays.isEmpty ? 'Please select at least one day' : null;

  /// Creates a copy with updated fields
  AvailabilityInfo copyWith({
    AvailabilityType? availabilityType,
    List<TimeOfDay>? availableTimes,
    List<DayOfWeek>? availableDays,
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
