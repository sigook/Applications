import 'package:equatable/equatable.dart';

class Job extends Equatable {
  final String id;
  final String jobTitle;
  final int numberId;
  final int workersQuantity;
  final String location;
  final String? entrance;
  final String agencyFullName;
  final String? agencyLogo;
  final String status;
  final bool isAsap;
  final bool? workerApprovedToWork;
  final double workerRate;
  final double? workerSalary;
  final DateTime createdAt;
  final DateTime finishAt;
  final DateTime startAt;
  final String durationTerm;

  const Job({
    required this.id,
    required this.jobTitle,
    required this.numberId,
    required this.workersQuantity,
    required this.location,
    this.entrance,
    required this.agencyFullName,
    this.agencyLogo,
    required this.status,
    required this.isAsap,
    this.workerApprovedToWork,
    required this.workerRate,
    this.workerSalary,
    required this.createdAt,
    required this.finishAt,
    required this.startAt,
    required this.durationTerm,
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
