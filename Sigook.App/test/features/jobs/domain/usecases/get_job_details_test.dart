import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/job_details.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_job_details.dart';

import '../../../../helpers/mocks.dart';

final _tNow = DateTime(2024, 1, 1);

final _tJobDetails = JobDetails(
  id: 'job-123',
  jobTitle: 'Warehouse Associate',
  numberId: 42,
  workersQuantity: 5,
  agencyFullName: 'Test Agency',
  status: 'Open',
  isAsap: false,
  workerRate: 20.0,
  createdAt: _tNow,
  startAt: _tNow,
  isApplicant: false,
  punchCardOptionEnabled: true,
  holidayIsPaid: false,
  breakIsPaid: true,
);

void main() {
  late MockJobsRepository mockRepo;
  late GetJobDetails usecase;

  setUp(() {
    mockRepo = MockJobsRepository();
    usecase = GetJobDetails(mockRepo);
  });

  test('returns JobDetails on success', () async {
    when(() => mockRepo.getJobDetails('job-123'))
        .thenAnswer((_) async => Right(_tJobDetails));

    final result = await usecase(GetJobDetailsParams(jobId: 'job-123'));

    expect(result, Right(_tJobDetails));
    verify(() => mockRepo.getJobDetails('job-123')).called(1);
  });

  test('passes jobId to repository', () async {
    when(() => mockRepo.getJobDetails(any()))
        .thenAnswer((_) async => Right(_tJobDetails));

    await usecase(GetJobDetailsParams(jobId: 'specific-id'));

    verify(() => mockRepo.getJobDetails('specific-id')).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'not found');
    when(() => mockRepo.getJobDetails(any()))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(GetJobDetailsParams(jobId: 'job-123'));

    expect(result, const Left(failure));
  });
}
