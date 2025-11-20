// lib/features/auth/presentation/viewmodels/auth_viewmodel.dart

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
  @override
  AuthState build() {
    return const AuthState();
  }

  Future<void> signIn() async {
    state = state.copyWith(isLoading: true, error: null);

    final signIn = ref.read(signInProvider);
    final result = await signIn(NoParams());

    result.fold(
      (failure) =>
          state = state.copyWith(isLoading: false, error: failure.message),
      (token) => state = state.copyWith(
        isLoading: false,
        token: token,
        isAuthenticated: true,
        error: null,
      ),
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
    state = state.copyWith(isLoading: true);

    final logout = ref.read(logoutProvider);
    await logout(NoParams());

    state = const AuthState();
  }
}
