import 'package:equatable/equatable.dart';
import 'available_time.dart';
import 'availability_type.dart';
import '../../../../core/constants/enums.dart' as enums;

/// Availability information entity
/// Represents the availability section of the registration form
///
/// Uses API data (UUIDs) for:
/// - availabilityType: UUID from /Catalog/availability
/// - availableTimes: UUIDs from /Catalog/availabilityTime
///
/// Uses enum for:
/// - availableDays: Universal constant (not from API)
class AvailabilityInfo extends Equatable {
  final AvailabilityType availabilityType; // Entity from API
  final List<AvailableTime> availableTimes; // Entities from API
  final List<enums.DayOfWeek> availableDays; // Keep enum

  const AvailabilityInfo({
    required this.availabilityType,
    required this.availableTimes,
    required this.availableDays,
  });

  /// Validates all fields
  bool get isValid {
    return availabilityType.value.isNotEmpty &&
        availableTimes.isNotEmpty &&
        availableDays.isNotEmpty;
  }

  /// Validation error messages
  String? get availabilityTypeError => availabilityType.value.isEmpty
      ? 'Please select availability type'
      : null;
  String? get availableTimesError =>
      availableTimes.isEmpty ? 'Please select at least one time slot' : null;
  String? get availableDaysError =>
      availableDays.isEmpty ? 'Please select at least one day' : null;

  /// Creates a copy with updated fields
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
