// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'account_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$AccountInfoModelImpl _$$AccountInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$AccountInfoModelImpl(
  email: json['email'] as String,
  password: json['password'] as String,
  confirmPassword: json['confirmPassword'] as String,
  termsAccepted: json['termsAccepted'] as bool,
);

Map<String, dynamic> _$$AccountInfoModelImplToJson(
  _$AccountInfoModelImpl instance,
) => <String, dynamic>{
  'email': instance.email,
  'password': instance.password,
  'confirmPassword': instance.confirmPassword,
  'termsAccepted': instance.termsAccepted,
};
