import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/data/models/auth_token_model.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';
import 'package:sigook_app_flutter/core/usecases/usecase.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/logout.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/resend_confirmation_link.dart';
import 'package:sigook_app_flutter/features/auth/domain/usecases/sign_in.dart';
import 'package:sigook_app_flutter/features/auth/presentation/providers/auth_providers.dart';
import 'package:sigook_app_flutter/features/auth/presentation/viewmodels/auth_viewmodel.dart';
import 'package:sigook_app_flutter/core/providers/analytics_providers.dart';

import '../../../../helpers/mocks.dart';
import '../../../../helpers/riverpod_test_helpers.dart';

// ── Mock use cases ───────────────────────────────────────────────────────────
class MockSignIn extends Mock implements SignIn {}
class MockLogout extends Mock implements Logout {}
class MockResendConfirmationLink extends Mock
    implements ResendConfirmationLink {}

void main() {
  setUpAll(() {
    registerFallbackValue(NoParams());
    registerFallbackValue(SignInParams(email: '', password: ''));
    registerFallbackValue(ResendConfirmationLinkParams(email: ''));
  });

  late MockSignIn mockSignIn;
  late MockLogout mockLogout;
  late MockResendConfirmationLink mockResend;
  late MockAuthRepository mockAuthRepo;
  late MockAuthLocalDataSource mockLocal;
  late MockAnalyticsService mockAnalytics;
  late MockCrashReportingService mockCrash;

  const tToken = AuthToken(accessToken: 'access-123', refreshToken: 'ref-456');
  const tExpiredTokenModel =
      AuthTokenModel(accessToken: 'access-123', refreshToken: 'ref-456');
  final tValidTokenModel = AuthTokenModel(
    accessToken: 'access-123',
    refreshToken: 'ref-456',
    expirationDateTime: DateTime.now().add(const Duration(hours: 1)),
  );
  final tRefreshedToken = AuthToken(
    accessToken: 'access-new',
    refreshToken: 'ref-new',
    expirationDateTime: DateTime.now().add(const Duration(hours: 1)),
  );
  const tRejectedRefresh = ServerFailure(
    message: 'Session expired',
    statusCode: 400,
    code: 'invalid_grant',
  );

  setUp(() {
    mockSignIn = MockSignIn();
    mockLogout = MockLogout();
    mockResend = MockResendConfirmationLink();
    mockAuthRepo = MockAuthRepository();
    mockLocal = MockAuthLocalDataSource();
    mockAnalytics = MockAnalyticsService();
    mockCrash = MockCrashReportingService();

    // Default: no cached token → session restore resolves unauthenticated
    when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);
    when(() => mockAuthRepo.clearSession())
        .thenAnswer((_) async => const Right(null));

    // Default analytics stubs (fire-and-forget, always succeed)
    when(() => mockAnalytics.setUserId(any())).thenAnswer((_) async {});
    when(() => mockAnalytics.logLogin(method: any(named: 'method')))
        .thenAnswer((_) async {});
    when(() => mockAnalytics.logEvent(
          name: any(named: 'name'),
          parameters: any(named: 'parameters'),
        )).thenAnswer((_) async {});
    when(() => mockCrash.setUserId(any())).thenAnswer((_) async {});
  });

  ProviderContainer buildTestContainer() {
    return buildContainer(ProviderContainer(overrides: [
      signInProvider.overrideWithValue(mockSignIn),
      logoutProvider.overrideWithValue(mockLogout),
      resendConfirmationLinkProvider.overrideWithValue(mockResend),
      authRepositoryProvider.overrideWithValue(mockAuthRepo),
      authLocalDataSourceProvider.overrideWithValue(mockLocal),
      analyticsServiceProvider.overrideWithValue(mockAnalytics),
      crashReportingServiceProvider.overrideWithValue(mockCrash),
    ]));
  }

  Future<AuthViewModel> restoredNotifier(ProviderContainer container) async {
    final notifier = container.read(authViewModelProvider.notifier);
    await notifier.sessionRestore;
    return notifier;
  }

  Future<void> flushMicrotasks() async {
    await Future<void>.delayed(Duration.zero);
    await Future<void>.delayed(Duration.zero);
  }

  // ── session restore ────────────────────────────────────────────────────────

  group('session restore', () {
    test('starts restoring and resolves unauthenticated without a cached token',
        () async {
      final container = buildTestContainer();
      expect(container.read(authViewModelProvider).isRestoringSession, true);

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.unauthenticated);
      final state = container.read(authViewModelProvider);
      expect(state.isRestoringSession, false);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      verifyNever(() => mockAuthRepo.refreshToken(any()));
    });

    test('authenticates with a valid cached token without refreshing',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.authenticated);
      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
      expect(state.token, tValidTokenModel.toEntity());
      verifyNever(() => mockAuthRepo.refreshToken(any()));
    });

    test('refreshes an expired cached token and authenticates with the new one',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tExpiredTokenModel);
      when(() => mockAuthRepo.refreshToken('ref-456'))
          .thenAnswer((_) async => Right(tRefreshedToken));
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.authenticated);
      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
      expect(state.token, tRefreshedToken);
      verify(() => mockAuthRepo.refreshToken('ref-456')).called(1);
    });

    test('clears the session when the refresh is rejected', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tExpiredTokenModel);
      when(() => mockAuthRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(tRejectedRefresh));
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.sessionExpired);
      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.sessionExpired, true);
      verify(() => mockAuthRepo.clearSession()).called(1);
      verify(() => mockAnalytics.logEvent(
            name: 'session_expired',
            parameters: any(named: 'parameters'),
          )).called(1);
    });

    test('clears the session when the expired token has no refresh token',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => const AuthTokenModel(accessToken: 'a'));
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.sessionExpired);
      expect(container.read(authViewModelProvider).sessionExpired, true);
      verify(() => mockAuthRepo.clearSession()).called(1);
      verifyNever(() => mockAuthRepo.refreshToken(any()));
    });

    test('keeps the stale token when the refresh fails transiently', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tExpiredTokenModel);
      when(() => mockAuthRepo.refreshToken(any()))
          .thenAnswer((_) async => const Left(NetworkFailure()));
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.refreshDeferred);
      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
      expect(state.token, tToken);
      expect(state.sessionExpired, false);
      verifyNever(() => mockAuthRepo.clearSession());
    });

    test('resolves unauthenticated when the storage read throws', () async {
      when(() => mockLocal.getCachedToken()).thenThrow(Exception('boom'));
      final container = buildTestContainer();

      final result =
          await container.read(authViewModelProvider.notifier).sessionRestore;

      expect(result, SessionRestoreResult.unauthenticated);
      expect(container.read(authViewModelProvider).isAuthenticated, false);
    });
  });

  // ── expireSession ──────────────────────────────────────────────────────────

  group('expireSession', () {
    test('clears the session when the signal fires during a session',
        () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();
      await restoredNotifier(container);
      expect(container.read(authViewModelProvider).isAuthenticated, true);

      container.read(sessionExpiredSignalProvider.notifier).emit();
      await flushMicrotasks();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.sessionExpired, true);
      verify(() => mockAuthRepo.clearSession()).called(1);
    });

    test('is idempotent when the signal fires again', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();
      await restoredNotifier(container);

      container.read(sessionExpiredSignalProvider.notifier).emit();
      await flushMicrotasks();
      container.read(sessionExpiredSignalProvider.notifier).emit();
      await flushMicrotasks();

      verify(() => mockAuthRepo.clearSession()).called(1);
    });

    test('ignores the signal when there is no session', () async {
      final container = buildTestContainer();
      await restoredNotifier(container);

      container.read(sessionExpiredSignalProvider.notifier).emit();
      await flushMicrotasks();

      expect(container.read(authViewModelProvider).sessionExpired, false);
      verifyNever(() => mockAuthRepo.clearSession());
    });

    test('acknowledgeSessionExpired clears the one-shot flag', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);

      container.read(sessionExpiredSignalProvider.notifier).emit();
      await flushMicrotasks();
      expect(container.read(authViewModelProvider).sessionExpired, true);

      notifier.acknowledgeSessionExpired();

      expect(container.read(authViewModelProvider).sessionExpired, false);
      expect(container.read(authViewModelProvider).isAuthenticated, false);
    });
  });

  // ── signIn ─────────────────────────────────────────────────────────────────

  group('signIn — role check', () {
    test('sets isAuthenticated=true for worker role', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Right('worker'));

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
      expect(state.token, tToken);
      expect(state.error, isNull);
    });

    test('denies access and sets error for non-worker role', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Right('admin'));
      when(() => mockLogout.call(any()))
          .thenAnswer((_) async => const Right(null));

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.error, isNotNull);
    });

    test('allows login when role check fails (graceful degradation)', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Left(ServerFailure(message: 'role error')));

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
    });

    test('sets error and stays unauthenticated on signIn failure', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockSignIn.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'Invalid credentials')),
      );

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.error, 'Invalid credentials');
      expect(state.isLoading, false);
    });

    test('exposes the failure code in errorCode', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockSignIn.call(any())).thenAnswer(
        (_) async => const Left(
          ServerFailure(
            message: 'Email not confirmed',
            statusCode: 400,
            code: 'email_not_confirmed',
          ),
        ),
      );

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.errorCode, 'email_not_confirmed');
      expect(state.error, 'Email not confirmed');
    });

    test('clears a pending sessionExpired flag', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => const AuthTokenModel(accessToken: 'a'));
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      expect(container.read(authViewModelProvider).sessionExpired, true);
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Right('worker'));

      await notifier.signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.sessionExpired, false);
      expect(state.isAuthenticated, true);
    });
  });

  // ── resendConfirmationLink ─────────────────────────────────────────────────

  group('resendConfirmationLink', () {
    test('pulses justConfirmationSent on success', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockResend.call(any()))
          .thenAnswer((_) async => const Right(null));

      var pulsed = false;
      container.listen(authViewModelProvider, (previous, next) {
        if (previous?.justConfirmationSent != true &&
            next.justConfirmationSent) {
          pulsed = true;
        }
      });

      await notifier.resendConfirmationLink('test@example.com');

      expect(pulsed, true);
      expect(
        container.read(authViewModelProvider).justConfirmationSent,
        false,
      );
      verify(() => mockResend.call(any())).called(1);
    });

    test('pulses error on failure', () async {
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockResend.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'send failed')),
      );

      var errorSeen = false;
      container.listen(authViewModelProvider, (previous, next) {
        if (next.error == 'send failed') {
          errorSeen = true;
        }
      });

      await notifier.resendConfirmationLink('test@example.com');

      expect(errorSeen, true);
    });
  });

  // ── logout ─────────────────────────────────────────────────────────────────

  group('logout', () {
    test('clears auth state on success', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockLogout.call(any()))
          .thenAnswer((_) async => const Right(null));

      await notifier.logout();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.error, isNull);
    });

    test('clears auth state even on failure', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      when(() => mockLogout.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'logout failed')),
      );

      await notifier.logout();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.isLoading, false);
    });
  });

  // ── deactivateAccount ──────────────────────────────────────────────────────

  group('deactivateAccount', () {
    test('resets state on success', () async {
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tValidTokenModel);
      when(() => mockAuthRepo.deactivateAccount(any()))
          .thenAnswer((_) async => const Right(null));
      final container = buildTestContainer();
      final notifier = await restoredNotifier(container);
      expect(container.read(authViewModelProvider).token, isNotNull);

      await notifier.deactivateAccount();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      verify(() => mockAuthRepo.deactivateAccount('access-123')).called(1);
    });
  });
}
