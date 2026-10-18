import 'package:equatable/equatable.dart';

class JobDetails extends Equatable {
  final String id;
  final String jobTitle;
  final int numberId;
  final int workersQuantity;
  final String? location;
  final String? entrance;
  final String status;
  final bool isAsap;
  final bool? workerApprovedToWork;
  final double workerRate;
  final double? workerSalary;
  final DateTime createdAt;
  final DateTime? finishAt;
  final DateTime startAt;
  final String? durationTerm;
  final bool isApplicant;
  final bool punchCardOptionEnabled;
  final String? description;
  final String? requirements;
  final String? jobPosition;
  final bool holidayIsPaid;
  final bool breakIsPaid;
  final String? incentive;
  final String? incentiveDescription;
  final String? durationBreak;
  final String? requestStatus;
  final double? latitude;
  final double? longitude;
  final double? checkpointLatitude;
  final double? checkpointLongitude;
  final double? checkpointRadiusMeters;

  const JobDetails({
    required this.id,
    required this.jobTitle,
    required this.numberId,
    required this.workersQuantity,
    this.location,
    this.entrance,
    required this.status,
    required this.isAsap,
    this.workerApprovedToWork,
    required this.workerRate,
    this.workerSalary,
    required this.createdAt,
    this.finishAt,
    required this.startAt,
    this.durationTerm,
    required this.isApplicant,
    required this.punchCardOptionEnabled,
    this.description,
    this.requirements,
    this.jobPosition,
    required this.holidayIsPaid,
    required this.breakIsPaid,
    this.incentive,
    this.incentiveDescription,
    this.durationBreak,
    this.requestStatus,
    this.latitude,
    this.longitude,
    this.checkpointLatitude,
    this.checkpointLongitude,
    this.checkpointRadiusMeters,
  });

  bool get hasCoordinates => latitude != null && longitude != null;

  bool get hasPunchCheckpoint =>
      checkpointLatitude != null &&
      checkpointLongitude != null &&
      checkpointRadiusMeters != null;

  bool get shouldShowApplyButton =>
      status.toLowerCase() != 'booked' && !isApplicant;

  bool get shouldShowTimesheetAndPunchcard =>
      punchCardOptionEnabled && status.toLowerCase() == 'booked';

  @override
  List<Object?> get props => [
    id,
    jobTitle,
    numberId,
    workersQuantity,
    location,
    entrance,
    status,
    isAsap,
    workerApprovedToWork,
    workerRate,
    workerSalary,
    createdAt,
    finishAt,
    startAt,
    durationTerm,
    isApplicant,
    punchCardOptionEnabled,
    description,
    requirements,
    jobPosition,
    holidayIsPaid,
    breakIsPaid,
    incentive,
    incentiveDescription,
    durationBreak,
    requestStatus,
    latitude,
    longitude,
    checkpointLatitude,
    checkpointLongitude,
    checkpointRadiusMeters,
  ];
}
