// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'registration_form_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_RegistrationFormModel _$RegistrationFormModelFromJson(
  Map<String, dynamic> json,
) => _RegistrationFormModel(
  basicInfo: json['basicInfo'] == null
      ? null
      : BasicInfoModel.fromJson(json['basicInfo'] as Map<String, dynamic>),
  preferencesInfo: json['preferencesInfo'] == null
      ? null
      : PreferencesInfoModel.fromJson(
          json['preferencesInfo'] as Map<String, dynamic>,
        ),
  documentsInfo: json['documentsInfo'] == null
      ? null
      : DocumentsInfoModel.fromJson(
          json['documentsInfo'] as Map<String, dynamic>,
        ),
  accountInfo: json['accountInfo'] == null
      ? null
      : AccountInfoModel.fromJson(json['accountInfo'] as Map<String, dynamic>),
);

Map<String, dynamic> _$RegistrationFormModelToJson(
  _RegistrationFormModel instance,
) => <String, dynamic>{
  'basicInfo': instance.basicInfo,
  'preferencesInfo': instance.preferencesInfo,
  'documentsInfo': instance.documentsInfo,
  'accountInfo': instance.accountInfo,
};
