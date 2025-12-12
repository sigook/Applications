// lib/features/auth/presentation/viewmodels/auth_viewmodel.dart

import 'package:flutter/foundation.dart';
import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
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

  /// Load cached token and validate session on initialization
  Future<void> _loadCachedToken() async {
    try {
      final localDataSource = ref.read(authLocalDataSourceProvider);
      final cachedToken = await localDataSource.getCachedToken();

      if (!ref.mounted) return;

      if (cachedToken != null) {
        // Set user as authenticated with cached token first
        state = state.copyWith(token: cachedToken, isAuthenticated: true);

        // Check if token is expired and try to refresh
        final expirationDateTime = cachedToken.expirationDateTime;
        final isExpired =
            expirationDateTime != null &&
            DateTime.now().isAfter(
              expirationDateTime.subtract(const Duration(minutes: 5)),
            );

        if (isExpired && cachedToken.refreshToken != null) {
          // Token expired, try to refresh in background
          debugPrint('🔄 Token expired, attempting refresh...');
          await _refreshTokenSilent();
        }
      }
    } catch (e) {
      // Silent fail - user will need to login manually
      debugPrint('Failed to load cached token: $e');
    } finally {
      _isInitialized = true;
    }
  }

  /// Silently refresh token without showing loading state
  Future<void> _refreshTokenSilent() async {
    final currentToken = state.token;
    if (currentToken?.refreshToken == null) {
      // No refresh token available, but keep user authenticated with current token
      debugPrint('⚠️ No refresh token available, keeping current session');
      return;
    }

    final refreshToken = ref.read(refreshTokenProvider);
    final result = await refreshToken(
      RefreshTokenParams(refreshToken: currentToken!.refreshToken!),
    );

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        debugPrint('❌ Token refresh failed: ${failure.message}');
        // Keep user authenticated with cached token even if refresh fails
        // They will be prompted to re-authenticate when they try to access protected resources
        debugPrint('ℹ️ Keeping user authenticated with cached token');
      },
      (token) {
        debugPrint('✅ Token refreshed successfully');
        state = state.copyWith(token: token, isAuthenticated: true);
      },
    );
  }

  Future<void> signIn() async {
    state = state.copyWith(isLoading: true, error: null);

    final signIn = ref.read(signInProvider);
    final result = await signIn(NoParams());

    result.fold(
      (failure) {
        if (failure.message.contains('User cancelled')) {
          debugPrint('ℹ️ User cancelled authentication - no error shown');
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

    final logout = ref.read(logoutProvider);
    final result = await logout(NoParams());

    if (!ref.mounted) return;

    result.fold(
      (failure) {
        state = state.copyWith(
          isLoading: false,
          error: failure.message,
          // Keep authentication state until logout succeeds
        );
      },
      (success) {
        state = const AuthState();
      },
    );
  }
}
