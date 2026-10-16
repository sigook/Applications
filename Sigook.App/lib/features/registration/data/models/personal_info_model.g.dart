// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'personal_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_PersonalInfoModel _$PersonalInfoModelFromJson(Map<String, dynamic> json) =>
    _PersonalInfoModel(
      firstName: json['firstName'] as String,
      lastName: json['lastName'] as String,
      dateOfBirth: DateTime.parse(json['dateOfBirth'] as String),
      genderId: json['genderId'] as String?,
      genderName: json['genderName'] as String,
    );

Map<String, dynamic> _$PersonalInfoModelToJson(_PersonalInfoModel instance) =>
    <String, dynamic>{
      'firstName': instance.firstName,
      'lastName': instance.lastName,
      'dateOfBirth': instance.dateOfBirth.toIso8601String(),
      'genderId': instance.genderId,
      'genderName': instance.genderName,
    };
