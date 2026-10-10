import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:geolocator/geolocator.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/services/location_service.dart';
import 'package:sigook_app_flutter/core/services/location_service_impl.dart';

import '../../helpers/mocks.dart';

void main() {
  late MockGeolocatorPlatform geolocator;
  late LocationServiceImpl service;

  setUpAll(() {
    registerFallbackValue(const LocationSettings());
  });

  setUp(() {
    geolocator = MockGeolocatorPlatform();
    service = LocationServiceImpl(geolocator: geolocator);
  });

  tearDown(() {
    debugDefaultTargetPlatformOverride = null;
  });

  Position makePosition({
    double accuracy = 5,
    DateTime? timestamp,
    bool isMocked = false,
  }) {
    return Position(
      longitude: -79.3832,
      latitude: 43.6532,
      timestamp: timestamp ?? DateTime.now(),
      accuracy: accuracy,
      altitude: 0,
      altitudeAccuracy: 0,
      heading: 0,
      headingAccuracy: 0,
      speed: 0,
      speedAccuracy: 0,
      isMocked: isMocked,
    );
  }

  void stubPermissionsGranted() {
    when(
      () => geolocator.isLocationServiceEnabled(),
    ).thenAnswer((_) async => true);
    when(
      () => geolocator.checkPermission(),
    ).thenAnswer((_) async => LocationPermission.whileInUse);
  }

  void stubPositionStream(Stream<Position> stream) {
    when(
      () => geolocator.getPositionStream(
        locationSettings: any(named: 'locationSettings'),
      ),
    ).thenAnswer((_) => stream);
  }

  group('getFreshPosition', () {
    test('returns first fresh fix that meets the target accuracy', () async {
      final controller = StreamController<Position>();
      var cancelled = false;
      controller.onCancel = () => cancelled = true;
      stubPositionStream(controller.stream);

      final fix = makePosition(accuracy: 10);
      controller.add(fix);

      final result = await service.getFreshPosition();

      expect(result, fix);
      expect(cancelled, isTrue);
    });

    test('discards stale fixes and returns null when only stale arrive', () async {
      final stale = makePosition(
        accuracy: 5,
        timestamp: DateTime.now().subtract(const Duration(minutes: 5)),
      );
      stubPositionStream(Stream.fromIterable([stale, stale]));

      final result = await service.getFreshPosition();

      expect(result, isNull);
    });

    test('keeps best accuracy when stream ends without reaching target', () async {
      final worse = makePosition(accuracy: 80);
      final best = makePosition(accuracy: 45);
      final middle = makePosition(accuracy: 60);
      stubPositionStream(Stream.fromIterable([worse, best, middle]));

      final result = await service.getFreshPosition();

      expect(result, best);
    });

    test('returns best-so-far when overall timeout passes with imprecise fixes', () async {
      stubPositionStream(
        Stream.periodic(
          const Duration(milliseconds: 50),
          (_) => makePosition(accuracy: 50),
        ).take(100),
      );

      final result = await service.getFreshPosition(
        overallTimeout: const Duration(milliseconds: 250),
      );

      expect(result, isNotNull);
      expect(result!.accuracy, 50);
    });

    test('returns the freshest fix when the best one aged past maxFixAge', () async {
      stubPositionStream(() async* {
        yield makePosition(accuracy: 30);
        await Future<void>.delayed(const Duration(milliseconds: 150));
        yield makePosition(accuracy: 60);
        await Future<void>.delayed(const Duration(milliseconds: 150));
        yield makePosition(accuracy: 70);
      }());

      final result = await service.getFreshPosition(
        targetAccuracyMeters: 5,
        maxFixAge: const Duration(milliseconds: 100),
        overallTimeout: const Duration(milliseconds: 250),
        fixDroughtTimeout: const Duration(milliseconds: 500),
      );

      expect(result, isNotNull);
      expect(result!.accuracy, isNot(30));
      expect(
        DateTime.now().difference(result.timestamp),
        lessThanOrEqualTo(const Duration(milliseconds: 100)),
      );
    });

    test('returns null when every fix aged out before the stream ended', () async {
      final controller = StreamController<Position>();
      stubPositionStream(controller.stream);
      controller.add(makePosition(accuracy: 50));

      final result = await service.getFreshPosition(
        targetAccuracyMeters: 5,
        maxFixAge: const Duration(milliseconds: 50),
        overallTimeout: const Duration(seconds: 5),
        fixDroughtTimeout: const Duration(milliseconds: 200),
      );

      expect(result, isNull);
      await controller.close();
    });

    test('never returns a fix older than maxFixAge on the deadline path', () async {
      stubPositionStream(
        Stream.periodic(
          const Duration(milliseconds: 40),
          (_) => makePosition(accuracy: 90),
        ).take(20),
      );

      final result = await service.getFreshPosition(
        targetAccuracyMeters: 5,
        maxFixAge: const Duration(milliseconds: 120),
        overallTimeout: const Duration(milliseconds: 300),
        fixDroughtTimeout: const Duration(milliseconds: 500),
      );

      expect(result, isNotNull);
      expect(
        DateTime.now().difference(result!.timestamp),
        lessThanOrEqualTo(const Duration(milliseconds: 120)),
      );
    });

    test('returns null on fix drought', () async {
      final controller = StreamController<Position>();
      stubPositionStream(controller.stream);

      final result = await service.getFreshPosition(
        fixDroughtTimeout: const Duration(milliseconds: 100),
      );

      expect(result, isNull);
      await controller.close();
    });

    test('returns null when the stream errors', () async {
      stubPositionStream(
        Stream<Position>.error(const LocationServiceDisabledException()),
      );

      final result = await service.getFreshPosition();

      expect(result, isNull);
    });
  });

  group('resolveLocationPrecision', () {
    test('returns precise without requesting full accuracy', () async {
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.precise);

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.precise);
      verifyNever(
        () => geolocator.requestTemporaryFullAccuracy(
          purposeKey: any(named: 'purposeKey'),
        ),
      );
    });

    test('returns undetermined when status is unknown', () async {
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.unknown);

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.undetermined);
    });

    test('iOS reduced upgrades to precise via temporary full accuracy', () async {
      debugDefaultTargetPlatformOverride = TargetPlatform.iOS;
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.reduced);
      when(
        () => geolocator.requestTemporaryFullAccuracy(
          purposeKey: any(named: 'purposeKey'),
        ),
      ).thenAnswer((_) async => LocationAccuracyStatus.precise);

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.precise);
      verify(
        () =>
            geolocator.requestTemporaryFullAccuracy(purposeKey: 'TimesheetPunch'),
      ).called(1);
    });

    test('iOS reduced stays reduced', () async {
      debugDefaultTargetPlatformOverride = TargetPlatform.iOS;
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.reduced);
      when(
        () => geolocator.requestTemporaryFullAccuracy(
          purposeKey: any(named: 'purposeKey'),
        ),
      ).thenAnswer((_) async => LocationAccuracyStatus.reduced);

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.reduced);
    });

    test('iOS reduced returns undetermined when the upgrade throws', () async {
      debugDefaultTargetPlatformOverride = TargetPlatform.iOS;
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.reduced);
      when(
        () => geolocator.requestTemporaryFullAccuracy(
          purposeKey: any(named: 'purposeKey'),
        ),
      ).thenThrow(Exception('missing plist entry'));

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.undetermined);
    });

    test('Android reduced returns reduced without requesting full accuracy', () async {
      debugDefaultTargetPlatformOverride = TargetPlatform.android;
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenAnswer((_) async => LocationAccuracyStatus.reduced);

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.reduced);
      verifyNever(
        () => geolocator.requestTemporaryFullAccuracy(
          purposeKey: any(named: 'purposeKey'),
        ),
      );
    });

    test('returns undetermined when getLocationAccuracy throws', () async {
      when(
        () => geolocator.getLocationAccuracy(),
      ).thenThrow(Exception('not supported'));

      final result = await service.resolveLocationPrecision();

      expect(result, LocationPrecision.undetermined);
    });
  });

  group('ensurePermissions', () {
    test('returns false when location service is disabled', () async {
      when(
        () => geolocator.isLocationServiceEnabled(),
      ).thenAnswer((_) async => false);

      final result = await service.ensurePermissions();

      expect(result, isFalse);
      verifyNever(() => geolocator.checkPermission());
    });

    test('requests permission when denied and returns false if still denied', () async {
      when(
        () => geolocator.isLocationServiceEnabled(),
      ).thenAnswer((_) async => true);
      when(
        () => geolocator.checkPermission(),
      ).thenAnswer((_) async => LocationPermission.denied);
      when(
        () => geolocator.requestPermission(),
      ).thenAnswer((_) async => LocationPermission.denied);

      final result = await service.ensurePermissions();

      expect(result, isFalse);
      verify(() => geolocator.requestPermission()).called(1);
    });

    test('returns false when permission is denied forever', () async {
      when(
        () => geolocator.isLocationServiceEnabled(),
      ).thenAnswer((_) async => true);
      when(
        () => geolocator.checkPermission(),
      ).thenAnswer((_) async => LocationPermission.deniedForever);

      final result = await service.ensurePermissions();

      expect(result, isFalse);
      verifyNever(() => geolocator.requestPermission());
    });

    test('returns true when permission is whileInUse without requesting', () async {
      stubPermissionsGranted();

      final result = await service.ensurePermissions();

      expect(result, isTrue);
      verifyNever(() => geolocator.requestPermission());
    });
  });

  group('getCurrentPosition', () {
    test('delegates with high accuracy settings and returns the position', () async {
      stubPermissionsGranted();
      final fix = makePosition(accuracy: 8);
      when(
        () => geolocator.getCurrentPosition(
          locationSettings: any(named: 'locationSettings'),
        ),
      ).thenAnswer((_) async => fix);

      final result = await service.getCurrentPosition();

      expect(result, fix);
      final captured =
          verify(
                () => geolocator.getCurrentPosition(
                  locationSettings: captureAny(named: 'locationSettings'),
                ),
              ).captured.single
              as LocationSettings;
      expect(captured.accuracy, LocationAccuracy.high);
    });

    test('returns null when permissions are not granted', () async {
      when(
        () => geolocator.isLocationServiceEnabled(),
      ).thenAnswer((_) async => false);

      final result = await service.getCurrentPosition();

      expect(result, isNull);
      verifyNever(
        () => geolocator.getCurrentPosition(
          locationSettings: any(named: 'locationSettings'),
        ),
      );
    });

    test('returns null when the platform call throws', () async {
      stubPermissionsGranted();
      when(
        () => geolocator.getCurrentPosition(
          locationSettings: any(named: 'locationSettings'),
        ),
      ).thenThrow(const LocationServiceDisabledException());

      final result = await service.getCurrentPosition();

      expect(result, isNull);
    });
  });
}
