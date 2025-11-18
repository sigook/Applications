import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/preferences_info.dart';
import '../../domain/entities/lifting_capacity.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/day_of_week.dart';

part 'preferences_info_model.freezed.dart';
part 'preferences_info_model.g.dart';

@freezed
sealed class PreferencesInfoModel with _$PreferencesInfoModel {
  const PreferencesInfoModel._();

  const factory PreferencesInfoModel({
    required Map<String, String> availabilityType, // {id, value}
    required List<Map<String, String>> availableTimes,
    required List<Map<String, String>> availableDays,
    required Map<String, String>? liftingCapacity, // {id, value}
    required bool hasVehicle,
    required List<Map<String, String>> languages,
    required List<String> skills, // Just skill values
  }) = _PreferencesInfoModel;

  /// Convert from domain entity
  factory PreferencesInfoModel.fromEntity(PreferencesInfo entity) {
    return PreferencesInfoModel(
      availabilityType: {
        'id': entity.availabilityType.id ?? '',
        'value': entity.availabilityType.value,
      },
      availableTimes: entity.availableTimes
          .map((t) => {'id': t.id ?? '', 'value': t.value})
          .toList(),
      availableDays: entity.availableDays
          .map((d) => {'id': d.id, 'value': d.value})
          .toList(),
      liftingCapacity: entity.liftingCapacity != null
          ? {
              'id': entity.liftingCapacity!.id ?? '',
              'value': entity.liftingCapacity!.value,
            }
          : null,
      hasVehicle: entity.hasVehicle,
      languages: entity.languages
          .map((l) => {'id': l.id ?? '', 'value': l.value})
          .toList(),
      skills: entity.skills.map((s) => s.value).toList(),
    );
  }

  /// Convert to domain entity
  PreferencesInfo toEntity() {
    return PreferencesInfo(
      availabilityType: AvailabilityType(
        id: availabilityType['id'],
        value: availabilityType['value'] ?? '',
      ),
      availableTimes: availableTimes
          .map((t) => AvailableTime(id: t['id'], value: t['value'] ?? ''))
          .toList(),
      availableDays: availableDays
          .map(
            (d) => DayOfWeekEntity(id: d['id'] ?? '', value: d['value'] ?? ''),
          )
          .toList(),
      liftingCapacity: liftingCapacity != null
          ? LiftingCapacity(
              id: liftingCapacity!['id'],
              value: liftingCapacity!['value'] ?? '',
            )
          : null,
      hasVehicle: hasVehicle,
      languages: languages
          .map((l) => Language(id: l['id'], value: l['value'] ?? ''))
          .toList(),
      skills: skills.map((s) => Skill(skill: s)).toList(),
    );
  }

  /// From JSON
  factory PreferencesInfoModel.fromJson(Map<String, dynamic> json) =>
      _$PreferencesInfoModelFromJson(json);
}
