// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'contact_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$ContactInfoModelImpl _$$ContactInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$ContactInfoModelImpl(
  email: json['email'] as String,
  password: json['password'] as String,
  identification: json['identification'] as String,
  identificationTypeId: json['identificationTypeId'] as String?,
  identificationTypeName: json['identificationTypeName'] as String,
);

Map<String, dynamic> _$$ContactInfoModelImplToJson(
  _$ContactInfoModelImpl instance,
) => <String, dynamic>{
  'email': instance.email,
  'password': instance.password,
  'identification': instance.identification,
  'identificationTypeId': instance.identificationTypeId,
  'identificationTypeName': instance.identificationTypeName,
};
