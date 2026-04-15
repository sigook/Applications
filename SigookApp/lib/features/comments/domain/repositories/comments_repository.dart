import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/worker_comment.dart';

abstract class CommentsRepository {
  Future<Either<Failure, List<WorkerComment>>> getComments(
    String workerId, {
    int pageSize = 10,
    int pageIndex = 1,
  });
}
