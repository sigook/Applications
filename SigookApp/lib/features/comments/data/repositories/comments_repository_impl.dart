import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/worker_comment.dart';
import '../../domain/repositories/comments_repository.dart';
import '../datasources/comments_remote_datasource.dart';

class CommentsRepositoryImpl implements CommentsRepository {
  final CommentsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  CommentsRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, List<WorkerComment>>> getComments(
    String workerId, {
    int pageSize = 10,
    int pageIndex = 1,
  }) async {
    if (!await networkInfo.isConnected) return Left(NetworkFailure());

    try {
      final models = await datasource.getComments(
        workerId,
        pageSize: pageSize,
        pageIndex: pageIndex,
      );
      return Right(models.map((m) => m.toEntity()).toList());
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
