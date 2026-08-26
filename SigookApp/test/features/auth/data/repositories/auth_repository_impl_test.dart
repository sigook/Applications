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

    test('returns ServerFailure preserving statusCode and code', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.signIn(
            email: any(named: 'email'),
            password: any(named: 'password'),
          )).thenThrow(
        ServerException(
          message: 'Invalid credentials',
          statusCode: 400,
          code: 'invalid_credentials',
        ),
      );

      final result = await repository.signIn(
        email: tEmail,
        password: tPassword,
      );

      expect(result.isLeft(), true);
      result.fold(
        (f) {
          expect(f, isA<ServerFailure>());
          final failure = f as ServerFailure;
          expect(failure.statusCode, 400);
          expect(failure.code, 'invalid_credentials');
        },
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

  // ── password reset ───────────────────────────────────────────────────────

  group('requestPasswordResetCode', () {
    const tEmail = 'test@example.com';

    test('returns Right(null) on success', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.requestPasswordResetCode(tEmail))
          .thenAnswer((_) async {});

      final result = await repository.requestPasswordResetCode(tEmail);

      expect(result.isRight(), true);
      verify(() => mockRemote.requestPasswordResetCode(tEmail)).called(1);
    });

    test('returns NetworkFailure when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.requestPasswordResetCode(tEmail);

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
      verifyNever(() => mockRemote.requestPasswordResetCode(any()));
    });
  });

  group('resetPassword', () {
    const tEmail = 'test@example.com';

    test('returns Right(null) on success', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.resetPassword(
            email: any(named: 'email'),
            code: any(named: 'code'),
            newPassword: any(named: 'newPassword'),
          )).thenAnswer((_) async {});

      final result = await repository.resetPassword(
        email: tEmail,
        code: '123456',
        newPassword: 'newPass1',
      );

      expect(result.isRight(), true);
      verify(() => mockRemote.resetPassword(
            email: tEmail,
            code: '123456',
            newPassword: 'newPass1',
          )).called(1);
    });

    test('returns ServerFailure preserving code', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.resetPassword(
            email: any(named: 'email'),
            code: any(named: 'code'),
            newPassword: any(named: 'newPassword'),
          )).thenThrow(
        ServerException(
          message: 'Invalid code',
          statusCode: 400,
          code: 'invalid_code',
        ),
      );

      final result = await repository.resetPassword(
        email: tEmail,
        code: '000000',
        newPassword: 'newPass1',
      );

      expect(result.isLeft(), true);
      result.fold(
        (f) => expect((f as ServerFailure).code, 'invalid_code'),
        (_) => fail('Expected Left'),
      );
    });

    test('returns NetworkFailure when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.resetPassword(
        email: tEmail,
        code: '123456',
        newPassword: 'newPass1',
      );

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });
  });

  group('resendConfirmationLink', () {
    const tEmail = 'test@example.com';

    test('returns Right(null) on success', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.resendConfirmationLink(tEmail))
          .thenAnswer((_) async {});

      final result = await repository.resendConfirmationLink(tEmail);

      expect(result.isRight(), true);
      verify(() => mockRemote.resendConfirmationLink(tEmail)).called(1);
    });

    test('returns NetworkFailure when offline', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.resendConfirmationLink(tEmail);

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });
  });

  // ── logout ────────────────────────────────────────────────────────────────

  group('logout', () {
    test('revokes current refresh token and clears local token', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.revokeRefreshToken(any()))
          .thenAnswer((_) async {});

      final result = await repository.logout();

      expect(result.isRight(), true);
      verify(() => mockRemote.revokeRefreshToken('refresh-456')).called(1);
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears local token even when revocation fails', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockRemote.revokeRefreshToken(any()))
          .thenThrow(Exception('revocation endpoint error'));

      final result = await repository.logout();

      // Result is still Right because graceful degradation
      expect(result.isRight(), true);
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears local token when no cached token exists', () async {
      when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);

      final result = await repository.logout();

      expect(result.isRight(), true);
      verifyNever(() => mockRemote.revokeRefreshToken(any()));
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears local token when cached token has no refresh token',
        () async {
      when(() => mockLocal.getCachedToken()).thenAnswer(
        (_) async => const AuthTokenModel(accessToken: 'access-123'),
      );

      final result = await repository.logout();

      expect(result.isRight(), true);
      verifyNever(() => mockRemote.revokeRefreshToken(any()));
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('clears token even when offline', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);

      final result = await repository.logout();

      expect(result.isRight(), true);
      verifyNever(() => mockRemote.revokeRefreshToken(any()));
      verify(() => mockLocal.clearToken()).called(1);
    });
  });

  // ── deactivateAccount ─────────────────────────────────────────────────────

  group('deactivateAccount', () {
    const tAccessToken = 'access-123';

    test('revokes refresh token best-effort and clears local token', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockRemote.deactivateAccount(any())).thenAnswer((_) async {});
      when(() => mockRemote.revokeRefreshToken(any()))
          .thenThrow(Exception('revocation endpoint error'));

      final result = await repository.deactivateAccount(tAccessToken);

      expect(result.isRight(), true);
      verify(() => mockRemote.deactivateAccount(tAccessToken)).called(1);
      verify(() => mockRemote.revokeRefreshToken('refresh-456')).called(1);
      verify(() => mockLocal.clearToken()).called(1);
    });

    test('keeps token and returns failure when deactivation fails', () async {
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => _tTokenModel);
      when(() => mockRemote.deactivateAccount(any()))
          .thenThrow(ServerException(message: 'Deactivation failed'));

      final result = await repository.deactivateAccount(tAccessToken);

      expect(result.isLeft(), true);
      verifyNever(() => mockRemote.revokeRefreshToken(any()));
      verifyNever(() => mockLocal.clearToken());
    });
  });
}
