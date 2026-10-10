import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/jobs/domain/repositories/timesheet_repository.dart';

class GetClockType implements UseCase<ClockType, GetClockTypeParams> {
  final TimesheetRepository repository;

  GetClockType(this.repository);

  @override
  Future<Either<Failure, ClockType>> call(GetClockTypeParams params) async {
    return await repository.getClockType(
      date: params.date,
      requestId: params.requestId,
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}

class GetClockTypeParams {
  final DateTime date;
  final String requestId;
  final double latitude;
  final double longitude;

  GetClockTypeParams({
    required this.date,
    required this.requestId,
    required this.latitude,
    required this.longitude,
  });
}
