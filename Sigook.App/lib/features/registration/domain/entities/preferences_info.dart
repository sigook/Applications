import 'package:equatable/equatable.dart';
import 'lifting_capacity.dart';
import 'language.dart';
import 'skill.dart';
import 'availability_type.dart';
import 'available_time.dart';
import 'day_of_week.dart';

class PreferencesInfo extends Equatable {
  final AvailabilityType availabilityType;
  final List<AvailableTime> availableTimes;
  final List<DayOfWeekEntity> availableDays;

  final LiftingCapacity? liftingCapacity;
  final bool hasVehicle;

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

  List<String> get languageValues => languages.map((l) => l.value).toList();
  List<String> get skillValues => skills.map((s) => s.value).toList();

  bool get isValid {
    return true;
  }

  String? get availabilityTypeError => null;
  String? get availableTimesError => null;
  String? get availableDaysError => null;
  String? get liftingCapacityError => null;
  String? get languagesError => null;
  String? get skillsError => null;

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
