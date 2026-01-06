import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/paginated_timesheet.dart';

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

  Future<Either<Failure, PaginatedTimesheet>> getTimesheetEntries({
    required String jobId,
    int pageIndex = 1,
    int pageSize = 5,
    bool isDescending = false,
  });
}
