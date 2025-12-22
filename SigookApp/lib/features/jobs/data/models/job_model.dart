import 'package:json_annotation/json_annotation.dart';
import '../../domain/entities/job.dart';

part 'job_model.g.dart';

@JsonSerializable()
class JobModel extends Job {
  const JobModel({
    required super.id,
    required super.jobTitle,
    required super.numberId,
    required super.workersQuantity,
    super.location,
    super.entrance,
    required super.agencyFullName,
    super.agencyLogo,
    super.status,
    required super.isAsap,
    super.workerApprovedToWork,
    required super.workerRate,
    super.workerSalary,
    required super.createdAt,
    super.finishAt,
    required super.startAt,
    super.durationTerm,
  });

  factory JobModel.fromJson(Map<String, dynamic> json) =>
      _$JobModelFromJson(json);

  Map<String, dynamic> toJson() => _$JobModelToJson(this);
}
