// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'assignment_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AssignmentModel _$AssignmentModelFromJson(Map<String, dynamic> json) =>
    _AssignmentModel(
      id: json['id'] as String,
      numberId: (json['numberId'] as num).toInt(),
      jobTitle: json['jobTitle'] as String?,
      clientName: json['clientName'] as String?,
      location: json['location'] as String?,
      status: json['status'] as String?,
      startAt: json['startAt'] == null
          ? null
          : DateTime.parse(json['startAt'] as String),
      startWorking: json['startWorking'] == null
          ? null
          : DateTime.parse(json['startWorking'] as String),
      endedAt: json['endedAt'] == null
          ? null
          : DateTime.parse(json['endedAt'] as String),
      hoursWorked: (json['hoursWorked'] as num?)?.toDouble() ?? 0,
    );

Map<String, dynamic> _$AssignmentModelToJson(_AssignmentModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'numberId': instance.numberId,
      'jobTitle': instance.jobTitle,
      'clientName': instance.clientName,
      'location': instance.location,
      'status': instance.status,
      'startAt': instance.startAt?.toIso8601String(),
      'startWorking': instance.startWorking?.toIso8601String(),
      'endedAt': instance.endedAt?.toIso8601String(),
      'hoursWorked': instance.hoursWorked,
    };
