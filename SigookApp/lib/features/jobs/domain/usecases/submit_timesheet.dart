import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';

class SubmitTimesheet
    implements UseCase<TimesheetResponse, SubmitTimesheetParams> {
  final TimesheetRepository repository;

  SubmitTimesheet(this.repository);

  @override
  Future<Either<Failure, TimesheetResponse>> call(
    SubmitTimesheetParams params,
  ) async {
    return await repository.submitTimesheet(
      jobId: params.jobId,
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}

class SubmitTimesheetParams {
  final String jobId;
  final double latitude;
  final double longitude;

  SubmitTimesheetParams({
    required this.jobId,
    required this.latitude,
    required this.longitude,
  });
}
