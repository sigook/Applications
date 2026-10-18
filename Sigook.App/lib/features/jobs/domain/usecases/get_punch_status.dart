import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/punch_status.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';

class GetPunchStatus implements UseCase<PunchStatus, GetPunchStatusParams> {
  final TimesheetRepository repository;

  GetPunchStatus(this.repository);

  @override
  Future<Either<Failure, PunchStatus>> call(GetPunchStatusParams params) {
    return repository.getPunchStatus(
      requestId: params.requestId,
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}

class GetPunchStatusParams {
  final String requestId;
  final double latitude;
  final double longitude;

  GetPunchStatusParams({
    required this.requestId,
    required this.latitude,
    required this.longitude,
  });
}
