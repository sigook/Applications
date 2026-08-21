import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_clock_type.dart';

import '../../../../helpers/mocks.dart';

void main() {
  late MockTimesheetRepository mockRepo;
  late GetClockType usecase;

  final tDate = DateTime(2024, 6, 15);
  const tRequestId = 'req-001';
  const tLatitude = 43.6532;
  const tLongitude = -79.3832;

  setUp(() {
    mockRepo = MockTimesheetRepository();
    usecase = GetClockType(mockRepo);
  });

  test('returns ClockType.clockIn when worker can clock in', () async {
    when(() => mockRepo.getClockType(
          date: tDate,
          requestId: tRequestId,
          latitude: tLatitude,
          longitude: tLongitude,
        )).thenAnswer((_) async => const Right(ClockType.clockIn));

    final result = await usecase(
      GetClockTypeParams(
        date: tDate,
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    expect(result, const Right(ClockType.clockIn));
  });

  test('returns ClockType.clockOut when worker must clock out', () async {
    when(() => mockRepo.getClockType(
          date: any(named: 'date'),
          requestId: any(named: 'requestId'),
          latitude: any(named: 'latitude'),
          longitude: any(named: 'longitude'),
        )).thenAnswer((_) async => const Right(ClockType.clockOut));

    final result = await usecase(
      GetClockTypeParams(
        date: tDate,
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    expect(result, const Right(ClockType.clockOut));
  });

  test('passes date, requestId and location to repository', () async {
    when(() => mockRepo.getClockType(
          date: tDate,
          requestId: tRequestId,
          latitude: tLatitude,
          longitude: tLongitude,
        )).thenAnswer((_) async => const Right(ClockType.clockIn));

    await usecase(
      GetClockTypeParams(
        date: tDate,
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    verify(() => mockRepo.getClockType(
          date: tDate,
          requestId: tRequestId,
          latitude: tLatitude,
          longitude: tLongitude,
        )).called(1);
  });

  test('returns ServerFailure when repository fails', () async {
    const failure = ServerFailure(message: 'timesheet error');
    when(() => mockRepo.getClockType(
          date: any(named: 'date'),
          requestId: any(named: 'requestId'),
          latitude: any(named: 'latitude'),
          longitude: any(named: 'longitude'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(
      GetClockTypeParams(
        date: tDate,
        requestId: tRequestId,
        latitude: tLatitude,
        longitude: tLongitude,
      ),
    );

    expect(result, const Left(failure));
  });
}
