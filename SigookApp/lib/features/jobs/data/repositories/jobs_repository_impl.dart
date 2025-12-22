import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/paginated_jobs.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/repositories/jobs_repository.dart';
import '../datasources/jobs_remote_datasource.dart';

class JobsRepositoryImpl implements JobsRepository {
  final JobsRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  JobsRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, PaginatedJobs>> getJobs({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  }) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final result = await remoteDataSource.getJobs(
        sortBy: sortBy,
        isDescending: isDescending,
        pageIndex: pageIndex,
        pageSize: pageSize,
      );
      return Right(result);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }

  @override
  Future<Either<Failure, JobDetails>> getJobDetails(String jobId) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final result = await remoteDataSource.getJobDetails(jobId);
      return Right(result);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
