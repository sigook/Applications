import '../../domain/entities/job_details.dart';

class JobDetailsModel extends JobDetails {
  const JobDetailsModel({
    required super.id,
    required super.jobTitle,
    required super.numberId,
    required super.workersQuantity,
    super.location,
    super.entrance,
    required super.agencyFullName,
    super.agencyLogo,
    required super.status,
    required super.isAsap,
    super.workerApprovedToWork,
    required super.workerRate,
    super.workerSalary,
    required super.createdAt,
    super.finishAt,
    required super.startAt,
    super.durationTerm,
    required super.isApplicant,
    required super.punchCardOptionEnabled,
    super.description,
    super.requirements,
    super.jobPosition,
    required super.holidayIsPaid,
    required super.breakIsPaid,
    super.incentive,
    super.incentiveDescription,
    super.durationBreak,
    super.requestStatus,
  });

  factory JobDetailsModel.fromJson(Map<String, dynamic> json) {
    return JobDetailsModel(
      id: json['id'] as String,
      jobTitle: json['jobTitle'] as String? ?? '',
      numberId: json['numberId'] as int? ?? 0,
      workersQuantity: json['workersQuantity'] as int? ?? 0,
      location: json['location'] as String?,
      entrance: json['entrance'] as String?,
      agencyFullName: json['agencyFullName'] as String? ?? '',
      agencyLogo: json['agencyLogo'] as String?,
      status: json['status'] as String? ?? '',
      isAsap: json['isAsap'] as bool? ?? false,
      workerApprovedToWork: json['workerApprovedToWork'] as bool?,
      workerRate: (json['workerRate'] as num?)?.toDouble() ?? 0.0,
      workerSalary: (json['workerSalary'] as num?)?.toDouble(),
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'] as String)
          : DateTime.now(),
      finishAt: json['finishAt'] != null
          ? DateTime.parse(json['finishAt'] as String)
          : null,
      startAt: json['startAt'] != null
          ? DateTime.parse(json['startAt'] as String)
          : DateTime.now(),
      durationTerm: json['durationTerm'] as String?,
      isApplicant: json['isApplicant'] as bool? ?? false,
      punchCardOptionEnabled: json['punchCardOptionEnabled'] as bool? ?? false,
      description: json['description'] as String?,
      requirements: json['requirements'] as String?,
      jobPosition: json['jobPosition'] as String?,
      holidayIsPaid: json['holidayIsPaid'] as bool? ?? false,
      breakIsPaid: json['breakIsPaid'] as bool? ?? false,
      incentive: json['incentive'] as String?,
      incentiveDescription: json['incentiveDescription'] as String?,
      durationBreak: json['durationBreak'] as String?,
      requestStatus: json['requestStatus'] as String?,
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
      'finishAt': finishAt?.toIso8601String(),
      'startAt': startAt.toIso8601String(),
      'durationTerm': durationTerm,
      'isApplicant': isApplicant,
      'punchCardOptionEnabled': punchCardOptionEnabled,
      'description': description,
      'requirements': requirements,
      'jobPosition': jobPosition,
      'holidayIsPaid': holidayIsPaid,
      'breakIsPaid': breakIsPaid,
      'incentive': incentive,
      'incentiveDescription': incentiveDescription,
      'durationBreak': durationBreak,
      'requestStatus': requestStatus,
    };
  }
}
