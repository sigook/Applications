// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'availability_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$AvailabilityInfoModelImpl _$$AvailabilityInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$AvailabilityInfoModelImpl(
  availabilityType: $enumDecode(
    _$AvailabilityTypeEnumMap,
    json['availabilityType'],
  ),
  availableTimes: (json['availableTimes'] as List<dynamic>)
      .map((e) => $enumDecode(_$TimeOfDayEnumMap, e))
      .toList(),
  availableDays: (json['availableDays'] as List<dynamic>)
      .map((e) => $enumDecode(_$DayOfWeekEnumMap, e))
      .toList(),
);

Map<String, dynamic> _$$AvailabilityInfoModelImplToJson(
  _$AvailabilityInfoModelImpl instance,
) => <String, dynamic>{
  'availabilityType': _$AvailabilityTypeEnumMap[instance.availabilityType]!,
  'availableTimes': instance.availableTimes
      .map((e) => _$TimeOfDayEnumMap[e]!)
      .toList(),
  'availableDays': instance.availableDays
      .map((e) => _$DayOfWeekEnumMap[e]!)
      .toList(),
};

const _$AvailabilityTypeEnumMap = {
  AvailabilityType.fullTime: 'fullTime',
  AvailabilityType.partTime: 'partTime',
};

const _$TimeOfDayEnumMap = {
  TimeOfDay.morning: 'morning',
  TimeOfDay.afternoon: 'afternoon',
  TimeOfDay.evening: 'evening',
};

const _$DayOfWeekEnumMap = {
  DayOfWeek.monday: 'monday',
  DayOfWeek.tuesday: 'tuesday',
  DayOfWeek.wednesday: 'wednesday',
  DayOfWeek.thursday: 'thursday',
  DayOfWeek.friday: 'friday',
  DayOfWeek.saturday: 'saturday',
  DayOfWeek.sunday: 'sunday',
};
