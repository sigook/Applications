import 'package:dartz/dartz.dart';
import 'package:flutter/foundation.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import '../entities/paginated_timesheet.dart';
import '../repositories/timesheet_repository.dart';

class GetTimesheetEntries
    implements UseCase<PaginatedTimesheet, GetTimesheetEntriesParams> {
  final TimesheetRepository repository;

  GetTimesheetEntries(this.repository);

  @override
  Future<Either<Failure, PaginatedTimesheet>> call(
    GetTimesheetEntriesParams params,
  ) async {
    debugPrint('🟢 [USE CASE] GetTimesheetEntries called');
    debugPrint('🟢 [USE CASE] Calling repository.getTimesheetEntries()...');

    final result = await repository.getTimesheetEntries(
      jobId: params.jobId,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
      isDescending: params.isDescending,
    );

    debugPrint(
      '🟢 [USE CASE] Repository returned: ${result.isRight() ? "SUCCESS" : "FAILURE"}',
    );
    return result;
  }
}

class GetTimesheetEntriesParams {
  final String jobId;
  final int pageIndex;
  final int pageSize;
  final bool isDescending;

  GetTimesheetEntriesParams({
    required this.jobId,
    this.pageIndex = 1,
    this.pageSize = 5,
    this.isDescending = false,
  });
}
