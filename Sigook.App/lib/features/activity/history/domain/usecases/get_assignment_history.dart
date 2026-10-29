import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/assignment.dart';
import '../repositories/assignment_history_repository.dart';

class GetAssignmentHistory {
  final AssignmentHistoryRepository repository;

  GetAssignmentHistory(this.repository);

  Future<Either<Failure, AssignmentPage>> call({
    String? status,
    int pageIndex = 1,
    int pageSize = 30,
  }) =>
      repository.getHistory(
        status: status,
        pageIndex: pageIndex,
        pageSize: pageSize,
      );
}
