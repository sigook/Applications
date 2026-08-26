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
  const tTokenModel = AuthTokenModel(accessToken: 'access-123', refreshToken: 'ref-456');

  setUp(() {
    mockSignIn = MockSignIn();
    mockLogout = MockLogout();
    mockResend = MockResendConfirmationLink();
    mockAuthRepo = MockAuthRepository();
    mockLocal = MockAuthLocalDataSource();
    mockAnalytics = MockAnalyticsService();
    mockCrash = MockCrashReportingService();

    // Default: no cached token → no auto-login on build
    when(() => mockLocal.getCachedToken()).thenAnswer((_) async => null);

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

  // ── signIn ─────────────────────────────────────────────────────────────────

  group('signIn — role check', () {
    test('sets isAuthenticated=true for worker role', () async {
      final container = buildTestContainer();
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Right('worker'));

      await container
          .read(authViewModelProvider.notifier)
          .signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
      expect(state.token, tToken);
      expect(state.error, isNull);
    });

    test('denies access and sets error for non-worker role', () async {
      final container = buildTestContainer();
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Right('admin'));
      when(() => mockLogout.call(any()))
          .thenAnswer((_) async => const Right(null));

      await container
          .read(authViewModelProvider.notifier)
          .signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.error, isNotNull);
    });

    test('allows login when role check fails (graceful degradation)', () async {
      final container = buildTestContainer();
      when(() => mockSignIn.call(any()))
          .thenAnswer((_) async => const Right(tToken));
      when(() => mockAuthRepo.getUserRole(any()))
          .thenAnswer((_) async => const Left(ServerFailure(message: 'role error')));

      await container
          .read(authViewModelProvider.notifier)
          .signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, true);
    });

    test('sets error and stays unauthenticated on signIn failure', () async {
      final container = buildTestContainer();
      when(() => mockSignIn.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'Invalid credentials')),
      );

      await container
          .read(authViewModelProvider.notifier)
          .signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.error, 'Invalid credentials');
      expect(state.isLoading, false);
    });

    test('exposes the failure code in errorCode', () async {
      final container = buildTestContainer();
      when(() => mockSignIn.call(any())).thenAnswer(
        (_) async => const Left(
          ServerFailure(
            message: 'Email not confirmed',
            statusCode: 400,
            code: 'email_not_confirmed',
          ),
        ),
      );

      await container
          .read(authViewModelProvider.notifier)
          .signIn(email: 'test@example.com', password: 'password123');

      final state = container.read(authViewModelProvider);
      expect(state.errorCode, 'email_not_confirmed');
      expect(state.error, 'Email not confirmed');
    });
  });

  // ── resendConfirmationLink ─────────────────────────────────────────────────

  group('resendConfirmationLink', () {
    test('pulses justConfirmationSent on success', () async {
      final container = buildTestContainer();
      when(() => mockResend.call(any()))
          .thenAnswer((_) async => const Right(null));

      var pulsed = false;
      container.listen(authViewModelProvider, (previous, next) {
        if (previous?.justConfirmationSent != true &&
            next.justConfirmationSent) {
          pulsed = true;
        }
      });

      await container
          .read(authViewModelProvider.notifier)
          .resendConfirmationLink('test@example.com');

      expect(pulsed, true);
      expect(
        container.read(authViewModelProvider).justConfirmationSent,
        false,
      );
      verify(() => mockResend.call(any())).called(1);
    });

    test('pulses error on failure', () async {
      final container = buildTestContainer();
      when(() => mockResend.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'send failed')),
      );

      var errorSeen = false;
      container.listen(authViewModelProvider, (previous, next) {
        if (next.error == 'send failed') {
          errorSeen = true;
        }
      });

      await container
          .read(authViewModelProvider.notifier)
          .resendConfirmationLink('test@example.com');

      expect(errorSeen, true);
    });
  });

  // ── logout ─────────────────────────────────────────────────────────────────

  group('logout', () {
    test('clears auth state on success', () async {
      final container = buildTestContainer();
      when(() => mockLogout.call(any()))
          .thenAnswer((_) async => const Right(null));

      await container.read(authViewModelProvider.notifier).logout();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.error, isNull);
    });

    test('clears auth state even on failure', () async {
      final container = buildTestContainer();
      when(() => mockLogout.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'logout failed')),
      );

      await container.read(authViewModelProvider.notifier).logout();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
      expect(state.isLoading, false);
    });
  });

  // ── deactivateAccount ──────────────────────────────────────────────────────

  group('deactivateAccount', () {
    test('resets state on success', () async {
      final container = buildTestContainer();

      // Manually set a token in state so deactivateAccount has an accessToken
      when(() => mockAuthRepo.deactivateAccount(any()))
          .thenAnswer((_) async => const Right(null));

      // Pre-load token via cache
      when(() => mockLocal.getCachedToken())
          .thenAnswer((_) async => tTokenModel);
      await Future<void>.delayed(Duration.zero); // let _loadCachedToken complete

      await container.read(authViewModelProvider.notifier).deactivateAccount();

      final state = container.read(authViewModelProvider);
      expect(state.isAuthenticated, false);
      expect(state.token, isNull);
    });
  });
}
