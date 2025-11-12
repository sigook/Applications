import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/preferences_info.dart';
import '../../domain/entities/lifting_capacity.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';

part 'preferences_info_model.freezed.dart';
part 'preferences_info_model.g.dart';

@freezed
class PreferencesInfoModel with _$PreferencesInfoModel {
  const PreferencesInfoModel._();

  const factory PreferencesInfoModel({
    required String availabilityType,
    required List<String> availableTimes,
    required List<String> availableDays,
    required String liftingCapacity, // Stored as string (enum label)
    required bool hasVehicle,
    required Map<String, String> languages, // id -> value
    required List<String> skills, // List of skill values
  }) = _PreferencesInfoModel;

  /// Convert from domain entity
  factory PreferencesInfoModel.fromEntity(PreferencesInfo entity) {
    // Convert languages to map
    final languagesMap = <String, String>{};
    for (final lang in entity.languages) {
      languagesMap[lang.id ?? lang.value] = lang.value;
    }

    // Convert skills to list of values
    final skillsList = entity.skills.map((s) => s.value).toList();

    return PreferencesInfoModel(
      availabilityType: entity.availabilityType,
      availableTimes: entity.availableTimes,
      availableDays: entity.availableDays,
      liftingCapacity: entity.liftingCapacity?.label ?? '',
      hasVehicle: entity.hasVehicle,
      languages: languagesMap,
      skills: skillsList,
    );
  }

  /// Convert to domain entity
  PreferencesInfo toEntity() {
    // Convert languages map back to list
    final languagesList = languages.entries
        .map((e) => Language(id: e.key, value: e.value))
        .toList();

    // Convert skills list back to Skill objects
    final skillsList = skills.map((s) => Skill(skill: s)).toList();

    // Parse lifting capacity from string
    LiftingCapacityType? capacity;
    for (final type in LiftingCapacityType.values) {
      if (type.label == liftingCapacity) {
        capacity = type;
        break;
      }
    }

    return PreferencesInfo(
      availabilityType: availabilityType,
      availableTimes: availableTimes,
      availableDays: availableDays,
      liftingCapacity: capacity,
      hasVehicle: hasVehicle,
      languages: languagesList,
      skills: skillsList,
    );
  }

  /// From JSON
  factory PreferencesInfoModel.fromJson(Map<String, dynamic> json) =>
      _$PreferencesInfoModelFromJson(json);
}
