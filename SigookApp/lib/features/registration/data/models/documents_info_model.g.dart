// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'documents_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$DocumentsInfoModelImpl _$$DocumentsInfoModelImplFromJson(
  Map<String, dynamic> json,
) => _$DocumentsInfoModelImpl(
  documents: (json['documents'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  licenses: (json['licenses'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  certificates: (json['certificates'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
  resume: json['resume'] as String?,
);

Map<String, dynamic> _$$DocumentsInfoModelImplToJson(
  _$DocumentsInfoModelImpl instance,
) => <String, dynamic>{
  'documents': instance.documents,
  'licenses': instance.licenses,
  'certificates': instance.certificates,
  'resume': instance.resume,
};
