import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/worker_comment.dart';
import '../repositories/comments_repository.dart';

class GetWorkerComments {
  final CommentsRepository repository;

  GetWorkerComments(this.repository);

  Future<Either<Failure, List<WorkerComment>>> call({
    int pageSize = 10,
    int pageIndex = 1,
  }) => repository.getComments(pageSize: pageSize, pageIndex: pageIndex);
}
