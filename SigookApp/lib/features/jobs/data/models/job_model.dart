import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/job.dart';

part 'job_model.freezed.dart';
part 'job_model.g.dart';

@freezed
abstract class JobModel with _$JobModel {
  const JobModel._();

  const factory JobModel({
    required String id,
    required String jobTitle,
    required int numberId,
    required int workersQuantity,
    String? location,
    String? entrance,
    required String agencyFullName,
    String? agencyLogo,
    String? status,
    required bool isAsap,
    bool? workerApprovedToWork,
    required double workerRate,
    double? workerSalary,
    required DateTime createdAt,
    DateTime? finishAt,
    required DateTime startAt,
    String? durationTerm,
  }) = _JobModel;

  factory JobModel.fromJson(Map<String, dynamic> json) =>
      _$JobModelFromJson(json);

  Job toEntity() {
    return Job(
      id: id,
      jobTitle: jobTitle,
      numberId: numberId,
      workersQuantity: workersQuantity,
      location: location,
      entrance: entrance,
      agencyFullName: agencyFullName,
      agencyLogo: agencyLogo,
      status: status,
      isAsap: isAsap,
      workerApprovedToWork: workerApprovedToWork,
      workerRate: workerRate,
      workerSalary: workerSalary,
      createdAt: createdAt,
      finishAt: finishAt,
      startAt: startAt,
      durationTerm: durationTerm,
    );
  }

  factory JobModel.fromEntity(Job job) {
    return JobModel(
      id: job.id,
      jobTitle: job.jobTitle,
      numberId: job.numberId,
      workersQuantity: job.workersQuantity,
      location: job.location,
      entrance: job.entrance,
      agencyFullName: job.agencyFullName,
      agencyLogo: job.agencyLogo,
      status: job.status,
      isAsap: job.isAsap,
      workerApprovedToWork: job.workerApprovedToWork,
      workerRate: job.workerRate,
      workerSalary: job.workerSalary,
      createdAt: job.createdAt,
      finishAt: job.finishAt,
      startAt: job.startAt,
      durationTerm: job.durationTerm,
    );
  }
}
