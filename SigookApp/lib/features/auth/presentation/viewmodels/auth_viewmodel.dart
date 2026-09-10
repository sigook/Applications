import 'package:flutter/foundation.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/auth_token.dart';
import '../../domain/usecases/resend_confirmation_link.dart';
import '../../domain/usecases/sign_in.dart';
import '../providers/auth_providers.dart';

part 'auth_viewmodel.freezed.dart';
part 'auth_viewmodel.g.dart';

enum SessionRestoreResult {
  authenticated,
  refreshDeferred,
  unauthenticated,
  sessionExpired,
}

@freezed
sealed class AuthState with _$AuthState {
  const factory AuthState({
    @Default(false) bool isLoading,
    String? error,
    String? errorCode,
    AuthToken? token,
    @Default(false) bool isAuthenticated,
    @Default(false) bool isRestoringSession,
    @Default(false) bool sessionExpired,
    @Default(false) bool justConfirmationSent,
  }) = _AuthState;
}

@Riverpod(keepAlive: true)
class AuthViewModel extends _$AuthViewModel {
  Future<SessionRestoreResult>? _sessionRestore;

  @override
  AuthState build() {
    ref.listen(sessionExpiredSignalProvider, (previous, next) {
      expireSession();
    });
    debugPrint('🔑 [AUTH] AuthViewModel build() called, starting session restore');
    _sessionRestore = _restoreSession();
    return const AuthState(isRestoringSession: true);
  }

  Future<SessionRestoreResult> get sessionRestore =>
      _sessionRestore ??= _restoreSession();

  Future<SessionRestoreResult> _restoreSession() async {
    final cachedToken = await _loadCachedToken();
    if (!ref.mounted) return SessionRestoreResult.unauthenticated;

    if (cachedToken == null || !cachedToken.hasAccessToken) {
      debugPrint('🔑 [AUTH] No cached token found');
      state = const AuthState();
      return SessionRestoreResult.unauthenticated;
    }

    if (!cachedToken.isExpired(leeway: AuthToken.defaultExpiryLeeway)) {
      debugPrint('🔑 [AUTH] Cached token still valid');
      state = AuthState(token: cachedToken, isAuthenticated: true);
      return SessionRestoreResult.authenticated;
    }

    final refreshToken = cachedToken.refreshToken;
    if (refreshToken == null || refreshToken.isEmpty) {
      debugPrint('🔑 [AUTH] Cached token expired and no refresh token');
      await _clearExpiredSession(reason: 'no_refresh_token');
      return SessionRestoreResult.sessionExpired;
    }

    debugPrint('🔑 [AUTH] Cached token expired, refreshing...');
    final result = await ref
        .read(authRepositoryProvider)
        .refreshToken(refreshToken);
    if (!ref.mounted) return SessionRestoreResult.unauthenticated;

    return result.fold(
      (failure) async {
        if (failure.isDefinitiveAuthFailure) {
          debugPrint('🔑 [AUTH] Refresh rejected: ${failure.message}');
          await _clearExpiredSession(
            reason: 'refresh_rejected',
            code: failure is ServerFailure ? failure.code : null,
          );
          return SessionRestoreResult.sessionExpired;
        }
        debugPrint('🔑 [AUTH] Refresh deferred: ${failure.message}');
        state = AuthState(token: cachedToken, isAuthenticated: true);
        return SessionRestoreResult.refreshDeferred;
      },
      (refreshedToken) async {
        debugPrint('🔑 [AUTH] Token refreshed');
        state = AuthState(token: refreshedToken, isAuthenticated: true);
        return SessionRestoreResult.authenticated;
      },
    );
  }

  Future<AuthToken?> _loadCachedToken() async {
    try {
      final cached = await ref.read(authLocalDataSourceProvider).getCachedToken();
      return cached?.toEntity();
    } catch (e) {
      debugPrint('🔑 [AUTH] Failed to load cached token: $e');
      return null;
    }
  }

  Future<void> _clearExpiredSession({
    required String reason,
    String? code,
  }) async {
    await ref.read(authRepositoryProvider).clearSession();
    if (!ref.mounted) return;
    state = const AuthState(sessionExpired: true);
    ref.read(analyticsServiceProvider).logEvent(
      name: 'session_expired',
      parameters: {
        'reason': reason,
        'code': ?code,
        'timestamp': DateTime.now().toIso8601String(),
      },
    );
  }

  Future<void> expireSession() async {
    if (!state.isAuthenticated && state.token == null) return;
    await _clearExpiredSession(reason: 'interceptor');
  }

  void acknowledgeSessionExpired() {
    if (!state.sessionExpired) return;
    state = state.copyWith(sessionExpired: false);
  }

  Future<void> signIn({required String email, required String password}) async {
    state = state.copyWith(
      isLoading: true,
      error: null,
      errorCode: null,
      sessionExpired: false,
    );

    final signInUseCase = ref.read(signInProvider);
    final result = await signInUseCase(
      SignInParams(email: email, password: password),
    );

    if (!ref.mounted) return;

    await result.fold(
      (failure) async {
        state = state.copyWith(
          isLoading: false,
          error: failure.message,
          errorCode: failure is ServerFailure ? failure.code : null,
        );
        ref.read(analyticsServiceProvider).logEvent(
          name: 'sign_in_failed',
          parameters: {
            'error': failure.message,
            'timestamp': DateTime.now().toIso8601String(),
          },
        );
      },
      (token) async {
        debugPrint(
          '🔑 [AUTH] Sign-in successful! Token received, checking user role...',
        );

        // Check user role via /connect/userinfo
        if (token.accessToken != null && token.accessToken!.isNotEmpty) {
          final authRepo = ref.read(authRepositoryProvider);
          final roleResult = await authRepo.getUserRole(token.accessToken!);

          if (!ref.mounted) return;

          await roleResult.fold(
            (failure) async {
              debugPrint('🔑 [AUTH] Failed to fetch user role: ${failure.message}');
              // Allow login even if role check fails (graceful degradation)
              state = state.copyWith(
                isLoading: false,
                token: token,
                isAuthenticated: true,
                error: null,
              );
              _trackLogin(token);
            },
            (role) async {
              if (role.toLowerCase() == 'worker') {
                debugPrint('🔑 [AUTH] User role is worker - access granted');
                state = state.copyWith(
                  isLoading: false,
                  token: token,
                  isAuthenticated: true,
                  error: null,
                );
                _trackLogin(token);
              } else {
                debugPrint(
                  '🔑 [AUTH] User role is "$role" - access denied, logging out',
                );
                ref.read(analyticsServiceProvider).logEvent(
                  name: 'sign_in_access_denied',
                  parameters: {
                    'role': role,
                    'timestamp': DateTime.now().toIso8601String(),
                  },
                );
                final logoutUseCase = ref.read(logoutProvider);
                await logoutUseCase(NoParams());

                if (!ref.mounted) return;

                state = state.copyWith(
                  isLoading: false,
                  token: null,
                  isAuthenticated: false,
                  error:
                      'This app is only available for workers. '
                      'Your account does not have the required permissions.',
                );
              }
            },
          );
        } else {
          state = state.copyWith(
            isLoading: false,
            token: token,
            isAuthenticated: true,
            error: null,
          );
          _trackLogin(token);
        }
      },
    );
  }

  Future<void> resendConfirmationLink(String email) async {
    final useCase = ref.read(resendConfirmationLinkProvider);
    final result = await useCase(ResendConfirmationLinkParams(email: email));

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        state = state.copyWith(error: failure.message, errorCode: null);
        state = state.copyWith(error: null);
      },
      (_) {
        state = state.copyWith(justConfirmationSent: true);
        state = state.copyWith(justConfirmationSent: false);
      },
    );
  }

  Future<void> deactivateAccount() async {
    final accessToken = state.token?.accessToken;
    if (accessToken == null) return;

    state = state.copyWith(isLoading: true, error: null);

    final result = await ref
        .read(authRepositoryProvider)
        .deactivateAccount(accessToken);

    if (!ref.mounted) return;

    result.fold(
      (failure) =>
          state = state.copyWith(isLoading: false, error: failure.message),
      (_) {
        ref.read(analyticsServiceProvider).logEvent(name: 'account_deactivated');
        state = const AuthState();
      },
    );
  }

  void _trackLogin(AuthToken token) {
    final subject =
        token.userInfo?.sub ?? _subjectFromAccessToken(token.accessToken);
    if (subject.isNotEmpty) {
      ref.read(analyticsServiceProvider).setUserId(subject);
      ref.read(crashReportingServiceProvider).setUserId(subject);
    }
    ref.read(analyticsServiceProvider).logLogin(method: 'password');
  }

  String _subjectFromAccessToken(String? accessToken) {
    if (accessToken == null || accessToken.isEmpty) return '';
    try {
      return JwtDecoder.decode(accessToken)['sub']?.toString() ?? '';
    } catch (_) {
      return '';
    }
  }

  Future<void> logout() async {
    state = state.copyWith(isLoading: true, error: null);

    // Track logout event
    ref
        .read(analyticsServiceProvider)
        .logEvent(
          name: 'user_logout',
          parameters: {'timestamp': DateTime.now().toIso8601String()},
        );

    final logout = ref.read(logoutProvider);
    final result = await logout(NoParams());

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        ref
            .read(analyticsServiceProvider)
            .logEvent(
              name: 'logout_failed',
              parameters: {'error': failure.message},
            );
        state = const AuthState();
      },
      (success) {
        state = const AuthState();
      },
    );
  }
}
