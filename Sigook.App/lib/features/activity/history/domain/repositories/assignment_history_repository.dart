import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/assignment.dart';

abstract class AssignmentHistoryRepository {
  Future<Either<Failure, AssignmentPage>> getHistory({
    String? status,
    required int pageIndex,
    required int pageSize,
  });
}
