import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/network/crash_reporting_interceptor.dart';

import '../../helpers/mocks.dart';

// Minimal fake handler so super.onError() has somewhere to forward to.
class _FakeErrorInterceptorHandler extends Fake
    implements ErrorInterceptorHandler {
  DioException? forwarded;

  @override
  void next(DioException err) {
    forwarded = err;
  }
}

DioException _makeDioError({
  int? statusCode,
  DioExceptionType type = DioExceptionType.badResponse,
}) {
  final response = statusCode != null
      ? Response<dynamic>(
          requestOptions: RequestOptions(path: '/test'),
          statusCode: statusCode,
        )
      : null;

  return DioException(
    requestOptions: RequestOptions(path: '/test', method: 'GET'),
    response: response,
    type: type,
  );
}

void main() {
  late MockCrashReportingService mockCrash;
  late CrashReportingInterceptor interceptor;

  setUp(() {
    mockCrash = MockCrashReportingService();
    interceptor = CrashReportingInterceptor(mockCrash);

    // Default stub: recordError does nothing
    when(() => mockCrash.recordError(
          any(),
          any(),
          fatal: any(named: 'fatal'),
          information: any(named: 'information'),
        )).thenAnswer((_) async {});
  });

  group('5xx errors → calls recordError', () {
    test('500 Internal Server Error', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 500);

      interceptor.onError(err, handler);

      verify(() => mockCrash.recordError(
            any(),
            any(),
            fatal: false,
            information: any(named: 'information'),
          )).called(1);
    });

    test('503 Service Unavailable', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 503);

      interceptor.onError(err, handler);

      verify(() => mockCrash.recordError(
            any(),
            any(),
            fatal: false,
            information: any(named: 'information'),
          )).called(1);
    });
  });

  group('4xx errors → does NOT call recordError', () {
    test('404 Not Found', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 404);

      interceptor.onError(err, handler);

      verifyNever(() => mockCrash.recordError(
            any(),
            any(),
            fatal: any(named: 'fatal'),
            information: any(named: 'information'),
          ));
    });

    test('400 Bad Request', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 400);

      interceptor.onError(err, handler);

      verifyNever(() => mockCrash.recordError(
            any(),
            any(),
            fatal: any(named: 'fatal'),
            information: any(named: 'information'),
          ));
    });

    test('401 Unauthorized', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 401);

      interceptor.onError(err, handler);

      verifyNever(() => mockCrash.recordError(
            any(),
            any(),
            fatal: any(named: 'fatal'),
            information: any(named: 'information'),
          ));
    });
  });

  group('Connection errors → calls recordError', () {
    test('connectionError type', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(
        statusCode: null,
        type: DioExceptionType.connectionError,
      );

      interceptor.onError(err, handler);

      verify(() => mockCrash.recordError(
            any(),
            any(),
            fatal: false,
            information: any(named: 'information'),
          )).called(1);
    });

    test('connectionTimeout type', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(
        statusCode: null,
        type: DioExceptionType.connectionTimeout,
      );

      interceptor.onError(err, handler);

      verify(() => mockCrash.recordError(
            any(),
            any(),
            fatal: false,
            information: any(named: 'information'),
          )).called(1);
    });
  });

  group('Always forwards to next handler', () {
    test('forwards 5xx error', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 500);

      interceptor.onError(err, handler);

      expect(handler.forwarded, isNotNull);
    });

    test('forwards 4xx error', () {
      final handler = _FakeErrorInterceptorHandler();
      final err = _makeDioError(statusCode: 400);

      interceptor.onError(err, handler);

      expect(handler.forwarded, isNotNull);
    });
  });
}
