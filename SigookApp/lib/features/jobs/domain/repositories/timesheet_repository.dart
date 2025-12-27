import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/clock_type.dart';

abstract class TimesheetRepository {
  Future<Either<Failure, ClockType>> getClockType({
    required DateTime date,
    required String requestId,
  });
}
