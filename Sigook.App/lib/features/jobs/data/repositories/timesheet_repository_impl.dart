import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/network/network_info.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/timesheet_remote_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/punch_status.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';

class TimesheetRepositoryImpl implements TimesheetRepository {
  final TimesheetRemoteDatasource remoteDataSource;
  final NetworkInfo networkInfo;

  TimesheetRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, PunchStatus>> getPunchStatus({
    required String requestId,
    required double latitude,
    required double longitude,
  }) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final result = await remoteDataSource.getPunchStatus(
        requestId,
        latitude,
        longitude,
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
  Future<Either<Failure, TimesheetResponse>> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  }) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final result = await remoteDataSource.submitTimesheet(
        jobId: jobId,
        latitude: latitude,
        longitude: longitude,
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
}
