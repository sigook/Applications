import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';

abstract class TimesheetRepository {
  Future<Either<Failure, ClockType>> getClockType({
    required DateTime date,
    required String requestId,
  });

  Future<Either<Failure, TimesheetResponse>> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  });
}
