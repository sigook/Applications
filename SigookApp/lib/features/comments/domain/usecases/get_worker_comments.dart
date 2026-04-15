import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/worker_comment.dart';
import '../repositories/comments_repository.dart';

class GetWorkerComments {
  final CommentsRepository repository;

  GetWorkerComments(this.repository);

  Future<Either<Failure, List<WorkerComment>>> call(
    String workerId, {
    int pageSize = 10,
    int pageIndex = 1,
  }) =>
      repository.getComments(workerId, pageSize: pageSize, pageIndex: pageIndex);
}
