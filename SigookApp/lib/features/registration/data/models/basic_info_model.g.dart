// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'basic_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$BasicInfoModelImpl _$$BasicInfoModelImplFromJson(Map<String, dynamic> json) =>
    _$BasicInfoModelImpl(
      firstName: json['firstName'] as String,
      lastName: json['lastName'] as String,
      dateOfBirth: json['dateOfBirth'] as String,
      genderId: json['genderId'] as String,
      genderValue: json['genderValue'] as String,
      country: json['country'] as String,
      provinceState: json['provinceState'] as String,
      city: json['city'] as String,
      address: json['address'] as String,
      zipCode: json['zipCode'] as String,
      mobileNumber: json['mobileNumber'] as String,
    );

Map<String, dynamic> _$$BasicInfoModelImplToJson(
  _$BasicInfoModelImpl instance,
) => <String, dynamic>{
  'firstName': instance.firstName,
  'lastName': instance.lastName,
  'dateOfBirth': instance.dateOfBirth,
  'genderId': instance.genderId,
  'genderValue': instance.genderValue,
  'country': instance.country,
  'provinceState': instance.provinceState,
  'city': instance.city,
  'address': instance.address,
  'zipCode': instance.zipCode,
  'mobileNumber': instance.mobileNumber,
};
