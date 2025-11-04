// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'professional_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$ProfessionalInfoModelImpl _$$ProfessionalInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$ProfessionalInfoModelImpl(
  languages: (json['languages'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  skills: (json['skills'] as List<dynamic>).map((e) => e as String).toList(),
);

Map<String, dynamic> _$$ProfessionalInfoModelImplToJson(
  _$ProfessionalInfoModelImpl instance,
) => <String, dynamic>{
  'languages': instance.languages,
  'skills': instance.skills,
};
