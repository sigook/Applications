import 'package:equatable/equatable.dart';
import 'lifting_capacity.dart';
import 'language.dart';
import 'skill.dart';
import 'availability_type.dart';
import 'available_time.dart';
import 'day_of_week.dart';

/// Preferences information entity
/// Combines availability, physical capabilities, and professional skills
class PreferencesInfo extends Equatable {
  // Availability
  final AvailabilityType availabilityType;
  final List<AvailableTime> availableTimes;
  final List<DayOfWeekEntity> availableDays;
  
  // Physical capabilities
  final LiftingCapacity? liftingCapacity;
  final bool hasVehicle;
  
  // Professional
  final List<Language> languages;
  final List<Skill> skills;

  const PreferencesInfo({
    required this.availabilityType,
    required this.availableTimes,
    required this.availableDays,
    this.liftingCapacity,
    required this.hasVehicle,
    required this.languages,
    required this.skills,
  });

  /// Helper to get language values as strings
  List<String> get languageValues => languages.map((l) => l.value).toList();
  
  /// Helper to get skill values as strings
  List<String> get skillValues => skills.map((s) => s.value).toList();

  /// Validates all required fields
  bool get isValid {
    return availabilityType.value.isNotEmpty &&
        availableTimes.isNotEmpty &&
        availableDays.isNotEmpty &&
        liftingCapacity != null &&
        languages.isNotEmpty &&
        skills.isNotEmpty;
  }

  /// Validation error messages
  String? get availabilityTypeError =>
      availabilityType.value.isEmpty ? 'Availability type is required' : null;
  String? get availableTimesError =>
      availableTimes.isEmpty ? 'At least one time slot is required' : null;
  String? get availableDaysError =>
      availableDays.isEmpty ? 'At least one day is required' : null;
  String? get liftingCapacityError =>
      liftingCapacity == null ? 'Lifting capacity is required' : null;
  String? get languagesError =>
      languages.isEmpty ? 'At least one language is required' : null;
  String? get skillsError =>
      skills.isEmpty ? 'At least one skill is required' : null;

  /// Creates a copy with updated fields
  PreferencesInfo copyWith({
    AvailabilityType? availabilityType,
    List<AvailableTime>? availableTimes,
    List<DayOfWeekEntity>? availableDays,
    LiftingCapacity? liftingCapacity,
    bool? hasVehicle,
    List<Language>? languages,
    List<Skill>? skills,
  }) {
    return PreferencesInfo(
      availabilityType: availabilityType ?? this.availabilityType,
      availableTimes: availableTimes ?? this.availableTimes,
      availableDays: availableDays ?? this.availableDays,
      liftingCapacity: liftingCapacity ?? this.liftingCapacity,
      hasVehicle: hasVehicle ?? this.hasVehicle,
      languages: languages ?? this.languages,
      skills: skills ?? this.skills,
    );
  }

  @override
  List<Object?> get props => [
        availabilityType,
        availableTimes,
        availableDays,
        liftingCapacity,
        hasVehicle,
        languages,
        skills,
      ];
}
