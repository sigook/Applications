// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'professional_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ProfessionalInfoModel _$ProfessionalInfoModelFromJson(
  Map<String, dynamic> json,
) => _ProfessionalInfoModel(
  languages: Map<String, String>.from(json['languages'] as Map),
  skills: (json['skills'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$ProfessionalInfoModelToJson(
  _ProfessionalInfoModel instance,
) => <String, dynamic>{
  'languages': instance.languages,
  'skills': instance.skills,
};
