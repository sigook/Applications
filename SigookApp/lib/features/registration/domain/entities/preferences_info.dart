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

  /// Validates all required fields - ALL FIELDS ARE OPTIONAL
  bool get isValid {
    return true;
  }

  /// Validation error messages - No errors since all fields are optional
  String? get availabilityTypeError => null;
  String? get availableTimesError => null;
  String? get availableDaysError => null;
  String? get liftingCapacityError => null;
  String? get languagesError => null;
  String? get skillsError => null;

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

  /// Convert to JSON for debugging/logging purposes
  Map<String, dynamic> toJson() {
    return {
      'availabilityType': availabilityType.toJson(),
      'availableTimes': availableTimes.map((t) => t.toJson()).toList(),
      'availableDays': availableDays.map((d) => d.toJson()).toList(),
      if (liftingCapacity != null) 'liftingCapacity': liftingCapacity!.toJson(),
      'hasVehicle': hasVehicle,
      'languages': languages.map((l) => l.toJson()).toList(),
      'skills': skills.map((s) => s.toJson()).toList(),
    };
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
