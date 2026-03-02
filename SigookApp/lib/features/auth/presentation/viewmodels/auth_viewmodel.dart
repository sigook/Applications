import 'package:flutter/foundation.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/auth_token.dart';
import '../../domain/usecases/refresh_token.dart';
import '../providers/auth_providers.dart';

part 'auth_viewmodel.freezed.dart';
part 'auth_viewmodel.g.dart';

@freezed
sealed class AuthState with _$AuthState {
  const factory AuthState({
    @Default(false) bool isLoading,
    String? error,
    AuthToken? token,
    @Default(false) bool isAuthenticated,
  }) = _AuthState;
}

@riverpod
class AuthViewModel extends _$AuthViewModel {
  bool _isInitialized = false;

  @override
  AuthState build() {
    // Reset initialization flag on each build (important for hot reload)
    _isInitialized = false;
    debugPrint(
      '🔑 [AUTH] AuthViewModel build() called (instance: $hashCode), starting token load',
    );
    _loadCachedToken();
    return const AuthState();
  }

  bool get isInitialized => _isInitialized;

  Future<void> _loadCachedToken() async {
    try {
      debugPrint('🔑 [AUTH] Loading cached token from secure storage...');
      final localDataSource = ref.read(authLocalDataSourceProvider);
      final cachedTokenModel = await localDataSource.getCachedToken();

      if (!ref.mounted) return;

      if (cachedTokenModel != null) {
        debugPrint(
          '🔑 [AUTH] Token found in secure storage. Access token: ${cachedTokenModel.accessToken?.substring(0, 20)}...',
        );
        final cachedToken = cachedTokenModel.toEntity();

        // Simply load the token into state
        // Validation will be done by the backend via validateToken API
        state = state.copyWith(token: cachedToken, isAuthenticated: true);
        debugPrint('🔑 [AUTH] Token loaded from cache and set in state');
      } else {
        debugPrint('🔑 [AUTH] No cached token found in secure storage');
        state = const AuthState();
      }
    } catch (e) {
      debugPrint('🔑 [AUTH] Failed to load cached token: $e');
      state = const AuthState();
    } finally {
      _isInitialized = true;
      if (ref.mounted) {
        debugPrint(
          '🔑 [AUTH] _loadCachedToken completed. Token present: ${state.token != null}',
        );
      } else {
        debugPrint(
          '🔑 [AUTH] _loadCachedToken completed but ref was unmounted',
        );
      }
    }
  }

  Future<void> signIn() async {
    state = state.copyWith(isLoading: true, error: null);

    final signIn = ref.read(signInProvider);
    final result = await signIn(NoParams());

    // Guard: provider may have been disposed while OAuth browser was open
    if (!ref.mounted) return;

    await result.fold(
      (failure) async {
        if (failure.message.contains('User cancelled')) {
          state = state.copyWith(isLoading: false, error: null);
        } else {
          state = state.copyWith(isLoading: false, error: failure.message);
        }
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
              } else {
                debugPrint(
                  '🔑 [AUTH] User role is "$role" - access denied, logging out',
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
        }
      },
    );
  }

  Future<void> tryAutoLogin() async {
    if (state.token != null && state.token!.refreshToken != null) {
      await refreshAuthToken();
    }
  }

  Future<void> refreshAuthToken() async {
    final currentToken = state.token;
    if (currentToken?.refreshToken == null) {
      state = state.copyWith(
        error: 'No refresh token available',
        isAuthenticated: false,
      );
      return;
    }

    state = state.copyWith(isLoading: true, error: null);

    final refreshToken = ref.read(refreshTokenProvider);
    final result = await refreshToken(
      RefreshTokenParams(refreshToken: currentToken!.refreshToken!),
    );

    result.fold(
      (failure) => state = state.copyWith(
        isLoading: false,
        error: failure.message,
        isAuthenticated: false,
      ),
      (token) => state = state.copyWith(
        isLoading: false,
        token: token,
        isAuthenticated: true,
        error: null,
      ),
    );
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
        state = state.copyWith(isLoading: false, error: failure.message);
      },
      (success) {
        state = const AuthState();
      },
    );
  }
}
