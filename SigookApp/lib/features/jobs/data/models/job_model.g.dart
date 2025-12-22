// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'job_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

JobModel _$JobModelFromJson(Map<String, dynamic> json) => JobModel(
  id: json['id'] as String,
  jobTitle: json['jobTitle'] as String,
  numberId: (json['numberId'] as num).toInt(),
  workersQuantity: (json['workersQuantity'] as num).toInt(),
  location: json['location'] as String?,
  entrance: json['entrance'] as String?,
  agencyFullName: json['agencyFullName'] as String,
  agencyLogo: json['agencyLogo'] as String?,
  status: json['status'] as String?,
  isAsap: json['isAsap'] as bool,
  workerApprovedToWork: json['workerApprovedToWork'] as bool?,
  workerRate: (json['workerRate'] as num).toDouble(),
  workerSalary: (json['workerSalary'] as num?)?.toDouble(),
  createdAt: DateTime.parse(json['createdAt'] as String),
  finishAt: json['finishAt'] == null
      ? null
      : DateTime.parse(json['finishAt'] as String),
  startAt: DateTime.parse(json['startAt'] as String),
  durationTerm: json['durationTerm'] as String?,
);

Map<String, dynamic> _$JobModelToJson(JobModel instance) => <String, dynamic>{
  'id': instance.id,
  'jobTitle': instance.jobTitle,
  'numberId': instance.numberId,
  'workersQuantity': instance.workersQuantity,
  'location': instance.location,
  'entrance': instance.entrance,
  'agencyFullName': instance.agencyFullName,
  'agencyLogo': instance.agencyLogo,
  'status': instance.status,
  'isAsap': instance.isAsap,
  'workerApprovedToWork': instance.workerApprovedToWork,
  'workerRate': instance.workerRate,
  'workerSalary': instance.workerSalary,
  'createdAt': instance.createdAt.toIso8601String(),
  'finishAt': instance.finishAt?.toIso8601String(),
  'startAt': instance.startAt.toIso8601String(),
  'durationTerm': instance.durationTerm,
};
