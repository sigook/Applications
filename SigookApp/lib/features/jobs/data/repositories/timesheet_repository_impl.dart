import 'package:dartz/dartz.dart';
import 'package:flutter/foundation.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/network/network_info.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/features/jobs/data/datasources/timesheet_remote_datasource.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/paginated_timesheet.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';

class TimesheetRepositoryImpl implements TimesheetRepository {
  final TimesheetRemoteDatasource remoteDataSource;
  final NetworkInfo networkInfo;

  TimesheetRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, ClockType>> getClockType({
    required DateTime date,
    required String requestId,
  }) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final result = await remoteDataSource.getClockType(date, requestId);
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

  @override
  Future<Either<Failure, PaginatedTimesheet>> getTimesheetEntries({
    required String jobId,
    int pageIndex = 1,
    int pageSize = 5,
    bool isDescending = false,
  }) async {
    debugPrint('🟡 [REPOSITORY] getTimesheetEntries called');
    debugPrint('🟡 [REPOSITORY] Checking network connection...');

    if (!await networkInfo.isConnected) {
      debugPrint('❌ [REPOSITORY] No network connection!');
      return Left(NetworkFailure());
    }

    debugPrint(
      '🟡 [REPOSITORY] Network connected, calling remote datasource...',
    );

    try {
      final result = await remoteDataSource.getTimesheetEntries(
        jobId: jobId,
        pageIndex: pageIndex,
        pageSize: pageSize,
        isDescending: isDescending,
      );
      debugPrint('🟡 [REPOSITORY] Datasource returned successfully');
      debugPrint('🟡 [REPOSITORY] Result items count: ${result.items.length}');
      return Right(result);
    } on ServerException catch (e) {
      debugPrint('❌ [REPOSITORY] ServerException: ${e.message}');
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      debugPrint('❌ [REPOSITORY] NetworkException: ${e.message}');
      return Left(NetworkFailure(message: e.message));
    } catch (e, stackTrace) {
      debugPrint('❌ [REPOSITORY] Unexpected error: $e');
      debugPrint('❌ [REPOSITORY] Stack trace: $stackTrace');
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
