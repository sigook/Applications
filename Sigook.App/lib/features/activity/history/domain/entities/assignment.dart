import 'package:equatable/equatable.dart';

class Assignment extends Equatable {
  final String id;
  final int numberId;
  final String jobTitle;
  final String clientName;
  final String? location;
  final String status;
  final DateTime? startedAt;
  final DateTime? endedAt;
  final double hoursWorked;

  const Assignment({
    required this.id,
    required this.numberId,
    required this.jobTitle,
    required this.clientName,
    this.location,
    required this.status,
    this.startedAt,
    this.endedAt,
    required this.hoursWorked,
  });

  @override
  List<Object?> get props => [
        id,
        numberId,
        jobTitle,
        clientName,
        location,
        status,
        startedAt,
        endedAt,
        hoursWorked,
      ];
}

class AssignmentPage extends Equatable {
  final List<Assignment> items;
  final int pageIndex;
  final bool hasMore;

  const AssignmentPage({
    required this.items,
    required this.pageIndex,
    required this.hasMore,
  });

  @override
  List<Object?> get props => [items, pageIndex, hasMore];
}
