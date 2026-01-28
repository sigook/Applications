// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'paginated_jobs_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_PaginatedJobsModel _$PaginatedJobsModelFromJson(Map<String, dynamic> json) =>
    _PaginatedJobsModel(
      items: (json['items'] as List<dynamic>)
          .map((e) => JobModel.fromJson(e as Map<String, dynamic>))
          .toList(),
      pageIndex: (json['pageIndex'] as num).toInt(),
      totalPages: (json['totalPages'] as num).toInt(),
      totalItems: (json['totalItems'] as num).toInt(),
    );

Map<String, dynamic> _$PaginatedJobsModelToJson(_PaginatedJobsModel instance) =>
    <String, dynamic>{
      'items': instance.items,
      'pageIndex': instance.pageIndex,
      'totalPages': instance.totalPages,
      'totalItems': instance.totalItems,
    };
