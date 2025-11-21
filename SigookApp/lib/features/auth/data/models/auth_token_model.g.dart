// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'auth_token_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AuthTokenModel _$AuthTokenModelFromJson(Map<String, dynamic> json) =>
    _AuthTokenModel(
      accessToken: json['accessToken'] as String?,
      idToken: json['idToken'] as String?,
      refreshToken: json['refreshToken'] as String?,
      expirationDateTime: json['expirationDateTime'] == null
          ? null
          : DateTime.parse(json['expirationDateTime'] as String),
      tokenType: json['tokenType'] as String?,
      scopes: (json['scopes'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList(),
      userInfo: json['userInfo'] == null
          ? null
          : UserInfoModel.fromJson(json['userInfo'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$AuthTokenModelToJson(_AuthTokenModel instance) =>
    <String, dynamic>{
      'accessToken': instance.accessToken,
      'idToken': instance.idToken,
      'refreshToken': instance.refreshToken,
      'expirationDateTime': instance.expirationDateTime?.toIso8601String(),
      'tokenType': instance.tokenType,
      'scopes': instance.scopes,
      'userInfo': instance.userInfo,
    };
