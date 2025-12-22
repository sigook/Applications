import 'package:equatable/equatable.dart';

class Job extends Equatable {
  final String id;
  final String jobTitle;
  final int numberId;
  final int workersQuantity;
  final String? location;
  final String? entrance;
  final String agencyFullName;
  final String? agencyLogo;
  final String? status;
  final bool isAsap;
  final bool? workerApprovedToWork;
  final double workerRate;
  final double? workerSalary;
  final DateTime createdAt;
  final DateTime? finishAt;
  final DateTime startAt;
  final String? durationTerm;

  const Job({
    required this.id,
    required this.jobTitle,
    required this.numberId,
    required this.workersQuantity,
    this.location,
    this.entrance,
    required this.agencyFullName,
    this.agencyLogo,
    this.status,
    required this.isAsap,
    this.workerApprovedToWork,
    required this.workerRate,
    this.workerSalary,
    required this.createdAt,
    this.finishAt,
    required this.startAt,
    this.durationTerm,
  });

  @override
  List<Object?> get props => [
    id,
    jobTitle,
    numberId,
    workersQuantity,
    location,
    entrance,
    agencyFullName,
    agencyLogo,
    status,
    isAsap,
    workerApprovedToWork,
    workerRate,
    workerSalary,
    createdAt,
    finishAt,
    startAt,
    durationTerm,
  ];
}
