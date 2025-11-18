// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'personal_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$PersonalInfoModelImpl _$$PersonalInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$PersonalInfoModelImpl(
  firstName: json['firstName'] as String,
  lastName: json['lastName'] as String,
  dateOfBirth: DateTime.parse(json['dateOfBirth'] as String),
  genderId: json['genderId'] as String?,
  genderName: json['genderName'] as String,
);

Map<String, dynamic> _$$PersonalInfoModelImplToJson(
  _$PersonalInfoModelImpl instance,
) => <String, dynamic>{
  'firstName': instance.firstName,
  'lastName': instance.lastName,
  'dateOfBirth': instance.dateOfBirth.toIso8601String(),
  'genderId': instance.genderId,
  'genderName': instance.genderName,
};
