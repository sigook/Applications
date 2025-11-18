// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preferences_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$PreferencesInfoModelImpl _$$PreferencesInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$PreferencesInfoModelImpl(
  availabilityType: Map<String, String>.from(json['availabilityType'] as Map),
  availableTimes: (json['availableTimes'] as List<dynamic>)
      .map((e) => Map<String, String>.from(e as Map))
      .toList(),
  availableDays: (json['availableDays'] as List<dynamic>)
      .map((e) => Map<String, String>.from(e as Map))
      .toList(),
  liftingCapacity: (json['liftingCapacity'] as Map<String, dynamic>?)?.map(
    (k, e) => MapEntry(k, e as String),
  ),
  hasVehicle: json['hasVehicle'] as bool,
  languages: (json['languages'] as List<dynamic>)
      .map((e) => Map<String, String>.from(e as Map))
      .toList(),
  skills: (json['skills'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$$PreferencesInfoModelImplToJson(
  _$PreferencesInfoModelImpl instance,
) => <String, dynamic>{
  'availabilityType': instance.availabilityType,
  'availableTimes': instance.availableTimes,
  'availableDays': instance.availableDays,
  'liftingCapacity': instance.liftingCapacity,
  'hasVehicle': instance.hasVehicle,
  'languages': instance.languages,
  'skills': instance.skills,
};
