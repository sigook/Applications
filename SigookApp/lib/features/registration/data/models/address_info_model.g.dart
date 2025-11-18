// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'address_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$AddressInfoModelImpl _$$AddressInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$AddressInfoModelImpl(
  country: json['country'] as String,
  provinceState: json['provinceState'] as String,
  city: json['city'] as String,
  address: json['address'] as String,
);

Map<String, dynamic> _$$AddressInfoModelImplToJson(
  _$AddressInfoModelImpl instance,
) => <String, dynamic>{
  'country': instance.country,
  'provinceState': instance.provinceState,
  'city': instance.city,
  'address': instance.address,
};
