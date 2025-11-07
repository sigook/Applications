// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'availability_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$AvailabilityInfoModelImpl _$$AvailabilityInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$AvailabilityInfoModelImpl(
  availabilityTypeId: json['availabilityTypeId'] as String?,
  availabilityTypeName: json['availabilityTypeName'] as String,
  availableTimes: Map<String, String>.from(json['availableTimes'] as Map),
  availableDays: (json['availableDays'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
);

Map<String, dynamic> _$$AvailabilityInfoModelImplToJson(
  _$AvailabilityInfoModelImpl instance,
) => <String, dynamic>{
  'availabilityTypeId': instance.availabilityTypeId,
  'availabilityTypeName': instance.availabilityTypeName,
  'availableTimes': instance.availableTimes,
  'availableDays': instance.availableDays,
};
