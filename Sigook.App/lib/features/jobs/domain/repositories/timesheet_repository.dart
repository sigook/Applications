import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/punch_status.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';

abstract class TimesheetRepository {
  Future<Either<Failure, PunchStatus>> getPunchStatus({
    required String requestId,
    required double latitude,
    required double longitude,
  });

  Future<Either<Failure, TimesheetResponse>> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  });
}
