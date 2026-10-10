import 'package:dartz/dartz.dart';
import 'package:flutter/foundation.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/paginated_jobs.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/repositories/jobs_repository.dart';
import '../datasources/jobs_remote_datasource.dart';
import '../datasources/jobs_local_datasource.dart';

class JobsRepositoryImpl implements JobsRepository {
  final JobsRemoteDataSource remoteDataSource;
  final JobsLocalDataSource localDataSource;
  final NetworkInfo networkInfo;

  JobsRepositoryImpl({
    required this.remoteDataSource,
    required this.localDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, PaginatedJobs>> getJobs({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  }) async {
    final isConnected = await networkInfo.isConnected;

    if (!isConnected) {
      // Return cached data if available
      try {
        final cachedJobs = await localDataSource.getCachedJobs();
        if (cachedJobs.isNotEmpty) {
          debugPrint('📦 Returning ${cachedJobs.length} cached jobs (offline)');
          final paginatedJobs = PaginatedJobs(
            items: cachedJobs.map((model) => model.toEntity()).toList(),
            pageIndex: 0,
            totalPages: 1,
            totalItems: cachedJobs.length,
          );
          return Right(paginatedJobs);
        }
      } catch (e) {
        debugPrint('Failed to load cached jobs: $e');
      }
      return Left(
        NetworkFailure(message: 'No internet connection and no cached data'),
      );
    }

    try {
      final resultModel = await remoteDataSource.getJobs(
        sortBy: sortBy,
        isDescending: isDescending,
        pageIndex: pageIndex,
        pageSize: pageSize,
      );

      // Cache the first page of jobs
      if (pageIndex == 0 && resultModel.items.isNotEmpty) {
        try {
          await localDataSource.cacheJobs(resultModel.items);
          debugPrint('💾 Cached ${resultModel.items.length} jobs');
        } catch (e) {
          debugPrint('Failed to cache jobs: $e');
        }
      }

      return Right(resultModel.toEntity());
    } on ServerException catch (e) {
      // Try returning cached data on server error
      try {
        final cachedJobs = await localDataSource.getCachedJobs();
        if (cachedJobs.isNotEmpty) {
          debugPrint(
            '📦 Returning ${cachedJobs.length} cached jobs (server error fallback)',
          );
          final paginatedJobs = PaginatedJobs(
            items: cachedJobs.map((model) => model.toEntity()).toList(),
            pageIndex: 0,
            totalPages: 1,
            totalItems: cachedJobs.length,
          );
          return Right(paginatedJobs);
        }
      } catch (cacheError) {
        debugPrint('Failed to load cached jobs: $cacheError');
      }
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

  @override
  Future<Either<Failure, void>> applyToJob(String jobId) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      await remoteDataSource.applyToJob(jobId);
      return const Right(null);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
