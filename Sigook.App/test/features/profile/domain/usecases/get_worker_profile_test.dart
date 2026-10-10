import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/profile/domain/entities/worker_profile.dart';
import 'package:sigook_app_flutter/features/profile/domain/usecases/get_worker_profile.dart';

import '../../../../helpers/mocks.dart';

const _tProfile = WorkerProfile(
  id: 'worker-001',
  approvedToWork: false,
  havePoliceCheckBackground: false,
  hasVehicle: false,
  availabilities: [],
  availabilityIds: [],
  availabilityTimes: [],
  availabilityTimeIds: [],
  availabilityDays: [],
  availabilityDayIds: [],
  languages: [],
  languageIds: [],
  skills: [],
  skillIds: [],
  hasResume: false,
  licenses: [],
  certificates: [],
  haveAnyHealthProblem: false,
  socialInsuranceExpire: false,
);

void main() {
  late MockProfileRepository mockRepo;
  late GetWorkerProfile usecase;

  setUp(() {
    mockRepo = MockProfileRepository();
    usecase = GetWorkerProfile(mockRepo);
  });

  test('returns WorkerProfile on success', () async {
    when(() => mockRepo.getWorkerBasicInfo())
        .thenAnswer((_) async => const Right(_tProfile));

    final result = await usecase(NoParams());

    expect(result, const Right(_tProfile));
    verify(() => mockRepo.getWorkerBasicInfo()).called(1);
  });

  test('returns NetworkFailure when offline', () async {
    const failure = NetworkFailure(message: 'no connection');
    when(() => mockRepo.getWorkerBasicInfo())
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(NoParams());

    expect(result, const Left(failure));
  });

  test('returns ServerFailure on server error', () async {
    const failure = ServerFailure(message: 'internal server error');
    when(() => mockRepo.getWorkerBasicInfo())
        .thenAnswer((_) async => const Left(failure));

    final result = await usecase(NoParams());

    expect(result.isLeft(), true);
    result.fold((f) => expect(f, isA<ServerFailure>()), (_) => fail(''));
  });
}
