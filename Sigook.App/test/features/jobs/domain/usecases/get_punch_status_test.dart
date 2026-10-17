import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/punch_status.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_punch_status.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockTimesheetRepository mockRepo;
  late GetPunchStatus usecase;

  const tRequestId = 'req-001';
  const tLatitude = 43.6532;
  const tLongitude = -79.3832;
  final tStatus = PunchStatus(
    clockType: ClockType.clockOut,
    clockIn: DateTime(2026, 10, 17, 8, 30),
    weekHours: 12.5,
  );

  setUp(() {
    mockRepo = MockTimesheetRepository();
    usecase = GetPunchStatus(mockRepo);
  });

  test('returns the punch status from the repository', () async {
    when(() => mockRepo.getPunchStatus(
          requestId: tRequestId,
          latitude: tLatitude,
          longitude: tLongitude,
        )).thenAnswer((_) async => Right(tStatus));

    final result = await usecase(
      GetPunchStatusParams(
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    expect(result, Right(tStatus));
    verify(() => mockRepo.getPunchStatus(
          requestId: tRequestId,
          latitude: tLatitude,
          longitude: tLongitude,
        )).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'timesheet error');
    when(() => mockRepo.getPunchStatus(
          requestId: any(named: 'requestId'),
          latitude: any(named: 'latitude'),
          longitude: any(named: 'longitude'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      GetPunchStatusParams(
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    expect(result, const Left(failure));
  });
}
