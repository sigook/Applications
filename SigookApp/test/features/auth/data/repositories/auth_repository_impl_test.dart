import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/data/models/auth_token_model.dart';
import 'package:sigook_app_flutter/features/auth/data/repositories/auth_repository_impl.dart';

import '../../../../helpers/mocks.dart';

const _tTokenModel = AuthTokenModel(
  accessToken: 'access-123',
  refreshToken: 'refresh-456',
  idToken: null,
);

void main() {
  // AuthTokenModel is a Freezed class — mocktail needs a fallback value for any().
  setUpAll(() {
    registerFallbackValue(const AuthTokenModel());
  });

  late MockAuthRemoteDataSource mockRemote;
  late MockAuthLocalDataSource mockLocal;
  late MockNetworkInfo mockNetwork;
  late AuthRepositoryImpl repository;

  setUp(() {
    mockRemote = MockAuthRemoteDataSource();
    mockLocal = MockAuthLocalDataSource();
    mockNetwork = MockNetworkInfo();
    repository = AuthRepositoryImpl(
      remote: mockRemote,
      local: mockLocal,
      networkInfo: mockNetwork,
    );

    // Default stubs for local datasource
    when(() => mockLocal.cacheToken(any())).thenAnswer((_) async {});
    when(() => mockLocal.clearToken()).thenAnswer((_) async {});
    when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);
  });

  // ── signIn ────────────────────────────────────────────────────────────────

  group('signIn', () {
    const tEmail = 'test@example.com';
    const tPassword = 'password123';

    test('returns NetworkFailure when device is offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result.isLeft(), true);
      result.fold(
        (f) => expect(f, isA<NetworkFailure>()),
        (_) => fail('Expected Left'),
      );
      verifyNever(() => mockRemote.signIn(
            email: any(named: 'email'),
            password: any(named: 'password'),
          ));
    });

    test('returns AuthToken and caches it on success', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.signIn(
            email: any(named: 'email'),
            password: any(named: 'password'),
          )).thenAnswer((_) async => _tTokenModel);

      final result = await repository.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result, isA<Right>());
      result.fold(
        (_) => fail('Expected Right'),
        (token) => expect(token.accessToken, 'access-123'),
      );
      verify(() => mockRemote.signIn(email: tEmail, password: tPassword))
          .called(1);
      verify(() => mockLocal.cacheToken(_tTokenModel)).called(1);
    });

    test('returns ServerFailure when datasource throws ServerException',
        () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.signIn(
            email: any(named: 'email'),
            password: any(named: 'password'),
          )).thenThrow(
        ServerException(message: 'Invalid credentials', statusCode: 401),
      );

      final result = await repository.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result.isLeft(), true);
      result.fold(
        (f) => expect(f, isA<ServerFailure>()),
        (_) => fail('Expected Left'),
      );
    });
  });

  // ── refreshToken ─────────────────────────────────────────────────────────

  group('refreshToken', () {
    const tRefreshToken = 'refresh-token-xyz';

    test('returns NetworkFailure when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.refreshToken(tRefreshToken);

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });

    test('returns new AuthToken and caches it on success', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.refreshToken(tRefreshToken))
          .thenAnswer((_) async => _tTokenModel);

      final result = await repository.refreshToken(tRefreshToken);

      expect(result.isRight(), true);
      verify(() => mockLocal.cacheToken(_tTokenModel)).called(1);
    });
  });

  // ── validateToken ─────────────────────────────────────────────────────────

  group('validateToken', () {
    const tAccessToken = 'access-token-abc';

    test('returns Right(true) when token is valid', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.validateToken(tAccessToken))
          .thenAnswer((_) async => true);

      final result = await repository.validateToken(tAccessToken);

      expect(result, const Right(true));
    });

    test('returns NetworkFailure when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.validateToken(tAccessToken);

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });
  });

  // ── logout ────────────────────────────────────────────────────────────────

  group('logout', () {
    test('clears local token even when network call fails', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.logout(any()))
          .thenThrow(Exception('logout endpoint error'));

      final result = await repository.logout();

      // Result is still Right because graceful degradation
      expect(result.isRight(), true);
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears local token when no cached token exists', () async {
      when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);

      final result = await repository.logout();

      expect(result.isRight(), true);
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears token even when offline', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.logout();

      expect(result.isRight(), true);
      verify(() => mockLocal.clearToken()).called(1);
    });
  });
}
