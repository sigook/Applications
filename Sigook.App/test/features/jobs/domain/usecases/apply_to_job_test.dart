import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/apply_to_job.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockJobsRepository mockRepo;
  late ApplyToJob usecase;

  setUp(() {
    mockRepo = MockJobsRepository();
    usecase = ApplyToJob(mockRepo);
  });

  test('returns void on success', () async {
    when(() => mockRepo.applyToJob('job-1'))
        .thenAnswer((_) async => const Right(null));

    final result = await usecase(ApplyToJobParams(jobId: 'job-1'));

    expect(result.isRight(), true);
    verify(() => mockRepo.applyToJob('job-1')).called(1);
  });

  test('passes jobId to repository', () async {
    when(() => mockRepo.applyToJob(any()))
        .thenAnswer((_) async => const Right(null));

    await usecase(ApplyToJobParams(jobId: 'specific-job'));

    verify(() => mockRepo.applyToJob('specific-job')).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'application failed');
    when(() => mockRepo.applyToJob(any()))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(ApplyToJobParams(jobId: 'job-1'));

    expect(result, const Left(failure));
  });

  test('returns NetworkFailure when offline', () async {
    const failure = NetworkFailure();
    when(() => mockRepo.applyToJob(any()))
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(ApplyToJobParams(jobId: 'job-1'));

    expect(result.isLeft(), true);
    result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
  });
}
