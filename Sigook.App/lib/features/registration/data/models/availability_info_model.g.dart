// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'availability_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AvailabilityInfoModel _$AvailabilityInfoModelFromJson(
  Map<String, dynamic> json,
) => _AvailabilityInfoModel(
  availabilityTypeId: json['availabilityTypeId'] as String?,
  availabilityTypeName: json['availabilityTypeName'] as String,
  availableTimes: Map<String, String>.from(json['availableTimes'] as Map),
  availableDays: (json['availableDays'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
);

Map<String, dynamic> _$AvailabilityInfoModelToJson(
  _AvailabilityInfoModel instance,
) => <String, dynamic>{
  'availabilityTypeId': instance.availabilityTypeId,
  'availabilityTypeName': instance.availabilityTypeName,
  'availableTimes': instance.availableTimes,
  'availableDays': instance.availableDays,
};
