import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/network/auth_interceptor.dart';
import 'package:sigook_app_flutter/features/auth/data/models/auth_token_model.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';

import '../../helpers/mocks.dart';

class MockDio extends Mock implements Dio {}

class _FakeRequestHandler extends Fake implements RequestInterceptorHandler {
  RequestOptions? forwarded;
  DioException? rejected;
  bool? rejectCallFollowing;

  @override
  void next(RequestOptions requestOptions) {
    forwarded = requestOptions;
  }

  @override
  void reject(DioException error, [bool callFollowingErrorInterceptor = false]) {
    rejected = error;
    rejectCallFollowing = callFollowingErrorInterceptor;
  }
}

class _FakeErrorHandler extends Fake implements ErrorInterceptorHandler {
  DioException? forwarded;
  Response<dynamic>? resolved;

  @override
  void next(DioException err) {
    forwarded = err;
  }

  @override
  void resolve(
    Response<dynamic> response, [
    bool callFollowingResponseInterceptor = false,
  ]) {
    resolved = response;
  }
}

void main() {
  setUpAll(() {
    registerFallbackValue(RequestOptions(path: '/'));
  });

  late MockAuthRepository mockRepo;
  late MockAuthLocalDataSource mockLocal;
  late MockDio mockRetryDio;
  late int sessionExpiredCalls;
  late AuthInterceptor interceptor;

  final validToken = AuthTokenModel(
    accessToken: 'valid-access',
    refreshToken: 'rt-1',
    expirationDateTime: DateTime.now().add(const Duration(hours: 1)),
  );
  final expiredToken = AuthTokenModel(
    accessToken: 'stale-access',
    refreshToken: 'rt-1',
    expirationDateTime: DateTime.now().subtract(const Duration(minutes: 1)),
  );
  final expiredTokenWithoutRefresh = AuthTokenModel(
    accessToken: 'stale-access',
    expirationDateTime: DateTime.now().subtract(const Duration(minutes: 1)),
  );
  final refreshedToken = AuthToken(
    accessToken: 'fresh-access',
    refreshToken: 'rt-2',
    expirationDateTime: DateTime.now().add(const Duration(hours: 1)),
  );
  const rejectedRefresh = ServerFailure(
    message: 'Session expired',
    statusCode: 400,
    code: 'invalid_grant',
  );

  DioException unauthorized({
    String? authorization,
    Map<String, dynamic> extra = const {},
    Object? data,
  }) {
    final options = RequestOptions(
      path: '/WorkerRequest',
      headers: {'Authorization': ?authorization},
      extra: Map<String, dynamic>.from(extra),
      data: data,
    );
    return DioException(
      requestOptions: options,
      response: Response<dynamic>(requestOptions: options, statusCode: 401),
      type: DioExceptionType.badResponse,
    );
  }

  void stubRetrySuccess() {
    when(() => mockRetryDio.fetch<dynamic>(any())).thenAnswer((invocation) async {
      final options = invocation.positionalArguments.first as RequestOptions;
      return Response<dynamic>(
        requestOptions: options,
        statusCode: 200,
        data: 'ok',
      );
    });
  }

  RequestOptions capturedRetryOptions() =>
      verify(() => mockRetryDio.fetch<dynamic>(captureAny())).captured.single
          as RequestOptions;

  setUp(() {
    mockRepo = MockAuthRepository();
    mockLocal = MockAuthLocalDataSource();
    mockRetryDio = MockDio();
    sessionExpiredCalls = 0;
    interceptor = AuthInterceptor(
      authRepository: mockRepo,
      localDataSource: mockLocal,
      retryDio: mockRetryDio,
      onSessionExpired: () => sessionExpiredCalls++,
    );
  });

  group('onRequest', () {
    test('stamps the bearer for a valid cached token', () async {
      when(() => mockLocal.getCachedToken()).thenAnswer((_) async => validToken);
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded, isNotNull);
      expect(handler.forwarded!.headers['Authorization'], 'Bearer valid-access');
      expect(handler.forwarded!.headers['Accept'], 'application/json');
      verifyNever(() => mockRepo.refreshToken(any()));
    });

    test('forwards without a bearer when nothing is cached', () async {
      when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded, isNotNull);
      expect(handler.forwarded!.headers.containsKey('Authorization'), false);
    });

    test('refreshes proactively when the cached token is expired', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken('rt-1'))
          .thenAnswer((_) async => Right(refreshedToken));
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded!.headers['Authorization'], 'Bearer fresh-access');
      verify(() => mockRepo.refreshToken('rt-1')).called(1);
      expect(sessionExpiredCalls, 0);
    });

    test('keeps the stale bearer when the refresh fails transiently', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(NetworkFailure()));
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded!.headers['Authorization'], 'Bearer stale-access');
      expect(handler.rejected, isNull);
      expect(sessionExpiredCalls, 0);
    });

    test('rejects with a synthetic 401 and signals expiry when the refresh is rejected',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(rejectedRefresh));
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded, isNull);
      expect(handler.rejected, isNotNull);
      expect(handler.rejected!.response?.statusCode, 401);
      expect(handler.rejected!.type, DioExceptionType.badResponse);
      expect(handler.rejectCallFollowing, true);
      expect(
        handler.rejected!.requestOptions
            .extra[AuthInterceptor.sessionExpiredExtraKey],
        true,
      );
      expect(sessionExpiredCalls, 1);
    });

    test('does not call the refresh endpoint again with a rejected refresh token',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(rejectedRefresh));

      await interceptor.onRequest(
        RequestOptions(path: '/x'),
        _FakeRequestHandler(),
      );
      final second = _FakeRequestHandler();
      await interceptor.onRequest(RequestOptions(path: '/y'), second);

      expect(second.rejected, isNotNull);
      verify(() => mockRepo.refreshToken(any())).called(1);
      expect(sessionExpiredCalls, 1);
    });

    test('stamps the stale bearer when the expired token has no refresh token',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredTokenWithoutRefresh);
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded!.headers['Authorization'], 'Bearer stale-access');
      verifyNever(() => mockRepo.refreshToken(any()));
    });

    test('forwards the request when reading the cache throws', () async {
      when(() => mockLocal.getCachedToken()).thenThrow(Exception('storage'));
      final handler = _FakeRequestHandler();

      await interceptor.onRequest(RequestOptions(path: '/x'), handler);

      expect(handler.forwarded, isNotNull);
      expect(handler.rejected, isNull);
    });
  });

  group('onError', () {
    test('forwards non-401 errors untouched', () async {
      final options = RequestOptions(path: '/x');
      final err = DioException(
        requestOptions: options,
        response: Response<dynamic>(requestOptions: options, statusCode: 500),
        type: DioExceptionType.badResponse,
      );
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(err));
      verifyNever(() => mockLocal.getCachedToken());
    });

    test('forwards a 401 when there is no refresh token', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredTokenWithoutRefresh);
      final err = unauthorized(authorization: 'Bearer stale-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(err));
      verifyNever(() => mockRepo.refreshToken(any()));
    });

    test('forwards a 401 already flagged as session expired without refreshing',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      final err = unauthorized(
        authorization: 'Bearer stale-access',
        extra: {AuthInterceptor.sessionExpiredExtraKey: true},
      );
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(err));
      verifyNever(() => mockRepo.refreshToken(any()));
    });

    test('refreshes on 401 and retries with the new bearer through retryDio',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken('rt-1'))
          .thenAnswer((_) async => Right(refreshedToken));
      stubRetrySuccess();
      final err = unauthorized(authorization: 'Bearer stale-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.resolved, isNotNull);
      expect(handler.resolved!.statusCode, 200);
      expect(handler.forwarded, isNull);
      expect(
        capturedRetryOptions().headers['Authorization'],
        'Bearer fresh-access',
      );
      expect(sessionExpiredCalls, 0);
    });

    test('retries with the cached bearer without refreshing when the token already rotated',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => validToken);
      stubRetrySuccess();
      final err = unauthorized(authorization: 'Bearer old-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.resolved, isNotNull);
      expect(
        capturedRetryOptions().headers['Authorization'],
        'Bearer valid-access',
      );
      verifyNever(() => mockRepo.refreshToken(any()));
    });

    test('signals expiry and forwards the error when the refresh is rejected',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(rejectedRefresh));
      final err = unauthorized(authorization: 'Bearer stale-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(err));
      expect(handler.resolved, isNull);
      expect(sessionExpiredCalls, 1);
      verifyNever(() => mockRetryDio.fetch<dynamic>(any()));
    });

    test('forwards the error without signalling when the refresh fails transiently',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(NetworkFailure()));
      final err = unauthorized(authorization: 'Bearer stale-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(err));
      expect(sessionExpiredCalls, 0);
    });

    test('forwards the retry error when the retried request fails', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => Right(refreshedToken));
      final retryError = unauthorized(authorization: 'Bearer fresh-access');
      when(() => mockRetryDio.fetch<dynamic>(any())).thenThrow(retryError);
      final err = unauthorized(authorization: 'Bearer stale-access');
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      expect(handler.forwarded, same(retryError));
      expect(handler.resolved, isNull);
    });

    test('clones FormData bodies before retrying', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => expiredToken);
      when(() => mockRepo.refreshToken(any()))
          .thenAnswer((_) async => Right(refreshedToken));
      stubRetrySuccess();
      final originalBody = FormData.fromMap({'field': 'value'});
      final err = unauthorized(
        authorization: 'Bearer stale-access',
        data: originalBody,
      );
      final handler = _FakeErrorHandler();

      await interceptor.onError(err, handler);

      final retriedBody = capturedRetryOptions().data;
      expect(retriedBody, isA<FormData>());
      expect(identical(retriedBody, originalBody), false);
      expect((retriedBody as FormData).fields, originalBody.fields);
    });
  });
}
