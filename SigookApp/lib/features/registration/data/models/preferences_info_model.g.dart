// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'preferences_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$PreferencesInfoModelImpl _$$PreferencesInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$PreferencesInfoModelImpl(
  availabilityType: json['availabilityType'] as String,
  availableTimes: (json['availableTimes'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  availableDays: (json['availableDays'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  liftingCapacity: json['liftingCapacity'] as String,
  hasVehicle: json['hasVehicle'] as bool,
  languages: Map<String, String>.from(json['languages'] as Map),
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
