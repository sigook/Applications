import '../../domain/entities/job.dart';

class JobModel extends Job {
  const JobModel({
    required super.id,
    required super.jobTitle,
    required super.numberId,
    required super.workersQuantity,
    required super.location,
    super.entrance,
    required super.agencyFullName,
    super.agencyLogo,
    required super.status,
    required super.isAsap,
    super.workerApprovedToWork,
    required super.workerRate,
    super.workerSalary,
    required super.createdAt,
    required super.finishAt,
    required super.startAt,
    required super.durationTerm,
  });

  factory JobModel.fromJson(Map<String, dynamic> json) {
    return JobModel(
      id: json['id'] as String,
      jobTitle: json['jobTitle'] as String,
      numberId: json['numberId'] as int,
      workersQuantity: json['workersQuantity'] as int,
      location: json['location'] as String,
      entrance: json['entrance'] as String?,
      agencyFullName: json['agencyFullName'] as String,
      agencyLogo: json['agencyLogo'] as String?,
      status: json['status'] as String,
      isAsap: json['isAsap'] as bool,
      workerApprovedToWork: json['workerApprovedToWork'] as bool?,
      workerRate: (json['workerRate'] as num).toDouble(),
      workerSalary: json['workerSalary'] != null
          ? (json['workerSalary'] as num).toDouble()
          : null,
      createdAt: DateTime.parse(json['createdAt'] as String),
      finishAt: DateTime.parse(json['finishAt'] as String),
      startAt: DateTime.parse(json['startAt'] as String),
      durationTerm: json['durationTerm'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'jobTitle': jobTitle,
      'numberId': numberId,
      'workersQuantity': workersQuantity,
      'location': location,
      'entrance': entrance,
      'agencyFullName': agencyFullName,
      'agencyLogo': agencyLogo,
      'status': status,
      'isAsap': isAsap,
      'workerApprovedToWork': workerApprovedToWork,
      'workerRate': workerRate,
      'workerSalary': workerSalary,
      'createdAt': createdAt.toIso8601String(),
      'finishAt': finishAt.toIso8601String(),
      'startAt': startAt.toIso8601String(),
      'durationTerm': durationTerm,
    };
  }
}
