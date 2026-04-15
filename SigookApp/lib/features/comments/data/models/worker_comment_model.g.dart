// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'worker_comment_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_WorkerCommentModel _$WorkerCommentModelFromJson(Map<String, dynamic> json) =>
    _WorkerCommentModel(
      id: json['id'] as String,
      comment: json['comment'] as String,
      rate: (json['rate'] as num).toDouble(),
      logo: json['logo'] as String?,
      numberId: (json['numberId'] as num).toInt(),
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$WorkerCommentModelToJson(_WorkerCommentModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'comment': instance.comment,
      'rate': instance.rate,
      'logo': instance.logo,
      'numberId': instance.numberId,
      'createdAt': instance.createdAt.toIso8601String(),
    };
