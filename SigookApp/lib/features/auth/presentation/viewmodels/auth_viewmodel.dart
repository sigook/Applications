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
    _loadCachedToken();
    return const AuthState();
  }

  bool get isInitialized => _isInitialized;

  Future<void> _loadCachedToken() async {
    try {
      final localDataSource = ref.read(authLocalDataSourceProvider);
      final cachedTokenModel = await localDataSource.getCachedToken();

      if (!ref.mounted) return;

      if (cachedTokenModel != null) {
        final cachedToken = cachedTokenModel.toEntity();

        // Check if refresh token is expired (typically 30 days)
        // Assuming refresh tokens are valid for 30 days from access token expiry
        if (cachedToken.expirationDateTime != null) {
          final refreshTokenExpiry = cachedToken.expirationDateTime!.add(
            const Duration(days: 30),
          );
          if (DateTime.now().isAfter(refreshTokenExpiry)) {
            debugPrint('Refresh token expired, clearing auth state');
            state = const AuthState();
            await localDataSource.clearToken();
            _isInitialized = true;
            return;
          }
        }

        // Check if token is expired locally
        final expirationDateTime = cachedToken.expirationDateTime;
        final isExpired =
            expirationDateTime != null &&
            DateTime.now().isAfter(expirationDateTime);

        if (isExpired) {
          debugPrint('Token is expired, attempting refresh');
          // Try to refresh if we have a refresh token
          if (cachedToken.refreshToken != null) {
            state = state.copyWith(token: cachedToken, isAuthenticated: false);
            _refreshTokenSilent();
          } else {
            // No refresh token, clear auth state
            state = const AuthState();
          }
        } else {
          // Token is not expired, consider it valid
          // It will be validated naturally when making API calls
          state = state.copyWith(token: cachedToken, isAuthenticated: true);
          debugPrint('Token loaded from cache and considered valid');

          // Proactive refresh if expiring soon (within 5 minutes)
          final isExpiringSoon =
              expirationDateTime != null &&
              DateTime.now().isAfter(
                expirationDateTime.subtract(const Duration(minutes: 5)),
              );

          if (isExpiringSoon && cachedToken.refreshToken != null) {
            debugPrint('Token expiring soon, refreshing proactively');
            _refreshTokenSilent();
          }
        }
      }
    } catch (e) {
      debugPrint('Failed to load cached token: $e');
    } finally {
      _isInitialized = true;
    }
  }

  Future<void> _refreshTokenSilent() async {
    final currentToken = state.token;
    if (currentToken?.refreshToken == null) {
      state = state.copyWith(isAuthenticated: false, token: null);
      return;
    }

    // Exponential backoff retry for token refresh
    int retryCount = 0;
    const maxRetries = 3;
    const baseDelay = Duration(milliseconds: 500);

    while (retryCount < maxRetries) {
      final refreshToken = ref.read(refreshTokenProvider);
      final result = await refreshToken(
        RefreshTokenParams(refreshToken: currentToken!.refreshToken!),
      );

      if (!ref.mounted) return;

      final shouldRetry = result.fold(
        (failure) {
          debugPrint(
            'Token refresh attempt ${retryCount + 1} failed: ${failure.message}',
          );

          // Don't retry on auth failures (401, 403)
          if (failure.message.contains('401') ||
              failure.message.contains('403') ||
              failure.message.contains('unauthorized')) {
            state = state.copyWith(isAuthenticated: false, token: null);
            return false;
          }

          return true; // Retry for network or server errors
        },
        (token) {
          state = state.copyWith(token: token, isAuthenticated: true);
          return false; // Success, no retry needed
        },
      );

      if (!shouldRetry) break;

      retryCount++;
      if (retryCount < maxRetries) {
        final delay =
            baseDelay * (1 << retryCount); // Exponential backoff: 1s, 2s, 4s
        debugPrint('Retrying token refresh in ${delay.inSeconds}s...');
        await Future.delayed(delay);
      } else {
        debugPrint('Token refresh failed after $maxRetries attempts');
        state = state.copyWith(isAuthenticated: false, token: null);
      }
    }
  }

  Future<void> signIn() async {
    state = state.copyWith(isLoading: true, error: null);

    final signIn = ref.read(signInProvider);
    final result = await signIn(NoParams());

    result.fold(
      (failure) {
        if (failure.message.contains('User cancelled')) {
          state = state.copyWith(isLoading: false, error: null);
        } else {
          state = state.copyWith(isLoading: false, error: failure.message);
        }
      },
      (token) {
        state = state.copyWith(
          isLoading: false,
          token: token,
          isAuthenticated: true,
          error: null,
        );
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
