import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/paginated_jobs.dart';
import 'job_model.dart';

part 'paginated_jobs_model.freezed.dart';
part 'paginated_jobs_model.g.dart';

@freezed
abstract class PaginatedJobsModel with _$PaginatedJobsModel {
  const PaginatedJobsModel._();

  const factory PaginatedJobsModel({
    required List<JobModel> items,
    required int pageIndex,
    required int totalPages,
    required int totalItems,
  }) = _PaginatedJobsModel;

  factory PaginatedJobsModel.fromJson(Map<String, dynamic> json) =>
      _$PaginatedJobsModelFromJson(json);

  PaginatedJobs toEntity() {
    return PaginatedJobs(
      items: items.map((model) => model.toEntity()).toList(),
      pageIndex: pageIndex,
      totalPages: totalPages,
      totalItems: totalItems,
    );
  }

  @override
  Map<String, dynamic> toJson() {
    return {
      'items': items.map((job) => job.toJson()).toList(),
      'pageIndex': pageIndex,
      'totalPages': totalPages,
      'totalItems': totalItems,
    };
  }
}
