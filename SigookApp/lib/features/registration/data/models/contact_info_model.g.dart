// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'contact_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ContactInfoModel _$ContactInfoModelFromJson(Map<String, dynamic> json) =>
    _ContactInfoModel(
      email: json['email'] as String,
      password: json['password'] as String,
      identification: json['identification'] as String,
      identificationTypeId: json['identificationTypeId'] as String?,
      identificationTypeName: json['identificationTypeName'] as String,
      mobileNumber: json['mobileNumber'] as String,
    );

Map<String, dynamic> _$ContactInfoModelToJson(_ContactInfoModel instance) =>
    <String, dynamic>{
      'email': instance.email,
      'password': instance.password,
      'identification': instance.identification,
      'identificationTypeId': instance.identificationTypeId,
      'identificationTypeName': instance.identificationTypeName,
      'mobileNumber': instance.mobileNumber,
    };
