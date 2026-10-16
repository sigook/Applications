import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/timesheet_response.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/submit_timesheet.dart';

import '../../../../helpers/mocks.dart';

const _tResponse = TimesheetResponse(
  timeSheetId: 'ts-001',
  workerFullName: 'John Doe',
  finish: false,
);

void main() {
  late MockTimesheetRepository mockRepo;
  late SubmitTimesheet usecase;

  setUp(() {
    mockRepo = MockTimesheetRepository();
    usecase = SubmitTimesheet(mockRepo);
  });

  test('returns TimesheetResponse on success', () async {
    when(() => mockRepo.submitTimesheet(
          jobId: any(named: 'jobId'),
          latitude: any(named: 'latitude'),
          longitude: any(named: 'longitude'),
        )).thenAnswer((_) async => const Right(_tResponse));

    final result = await usecase(SubmitTimesheetParams(
      jobId: 'job-1',
      latitude: 43.65,
      longitude: -79.38,
    ));

    expect(result, const Right(_tResponse));
  });

  test('passes all params to repository', () async {
    when(() => mockRepo.submitTimesheet(
          jobId: 'job-abc',
          latitude: 43.65,
          longitude: -79.38,
        )).thenAnswer((_) async => const Right(_tResponse));

    await usecase(SubmitTimesheetParams(
      jobId: 'job-abc',
      latitude: 43.65,
      longitude: -79.38,
    ));

    verify(() => mockRepo.submitTimesheet(
          jobId: 'job-abc',
          latitude: 43.65,
          longitude: -79.38,
        )).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'outside radius');
    when(() => mockRepo.submitTimesheet(
          jobId: any(named: 'jobId'),
          latitude: any(named: 'latitude'),
          longitude: any(named: 'longitude'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(SubmitTimesheetParams(
      jobId: 'job-1',
      latitude: 0.0,
      longitude: 0.0,
    ));

    expect(result, const Left(failure));
  });
}
