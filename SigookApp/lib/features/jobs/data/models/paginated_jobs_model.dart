import '../../domain/entities/paginated_jobs.dart';
import 'job_model.dart';

/// PaginatedJobsModel uses manual JSON serialization because it needs to
/// convert between List<Job> (entity) and List<JobModel> (model).
/// json_serializable can't handle this polymorphic conversion automatically.
class PaginatedJobsModel extends PaginatedJobs {
  const PaginatedJobsModel({
    required super.items,
    required super.pageIndex,
    required super.totalPages,
    required super.totalItems,
  });

  factory PaginatedJobsModel.fromJson(Map<String, dynamic> json) {
    return PaginatedJobsModel(
      items: (json['items'] as List<dynamic>)
          .map((item) => JobModel.fromJson(item as Map<String, dynamic>))
          .toList(),
      pageIndex: json['pageIndex'] as int,
      totalPages: json['totalPages'] as int,
      totalItems: json['totalItems'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'items': items.map((job) => (job as JobModel).toJson()).toList(),
      'pageIndex': pageIndex,
      'totalPages': totalPages,
      'totalItems': totalItems,
    };
  }
}
