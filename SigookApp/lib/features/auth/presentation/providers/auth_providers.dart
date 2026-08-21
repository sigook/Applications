import 'package:flutter_appauth/flutter_appauth.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:riverpod/riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/auth_interceptor.dart';

import '../../data/datasources/auth_local_datasource.dart';
import '../../data/datasources/auth_remote_datasource.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/usecases/sign_in.dart';
import '../../domain/usecases/logout.dart';
import '../../domain/usecases/request_password_reset_code.dart';
import '../../domain/usecases/reset_password.dart';
import '../../domain/usecases/resend_confirmation_link.dart';

part 'auth_providers.g.dart';

// 0. Auth Interceptor for automatic token refresh on 401 errors
final authInterceptorProvider = Provider<AuthInterceptor>((ref) {
  return AuthInterceptor(
    ref: ref,
    authRepository: ref.read(authRepositoryProvider),
    localDataSource: ref.read(authLocalDataSourceProvider),
    dio: ref.read(apiClientProvider).dio,
  );
});

// 0b. Authenticated API Client with auth interceptor
/// Use this provider for API calls that require authentication
final authenticatedApiClientProvider = Provider((ref) {
  final authInterceptor = ref.watch(authInterceptorProvider);
  final apiClient = ref.read(apiClientProvider);

  // Add the auth interceptor to the Dio instance
  // This is safe because it only happens once per provider instance
  if (!apiClient.dio.interceptors.any((i) => i is AuthInterceptor)) {
    apiClient.dio.interceptors.insert(0, authInterceptor);
  }

  return apiClient;
});

// 1. OAuth Client
final flutterAppAuthProvider = Provider<FlutterAppAuth>((ref) {
  return FlutterAppAuth();
});

// 2. Datasources
final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>((ref) {
  return AuthRemoteDataSourceImpl(
    dio: ref.read(apiClientProvider).dio,
    anonymousDio: ApiClient().dio,
    networkInfo: ref.read(networkInfoProvider),
    appAuth: ref.read(flutterAppAuthProvider),
  );
});

final authLocalDataSourceProvider = Provider<AuthLocalDataSource>((ref) {
  return AuthLocalDataSourceImpl(
    secureStorage: ref.read(secureStorageProvider),
  );
});

// 3. Repository
final authRepositoryProvider = Provider<AuthRepository>((ref) {
  return AuthRepositoryImpl(
    remote: ref.read(authRemoteDataSourceProvider),
    local: ref.read(authLocalDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

// 4. UseCases
@riverpod
SignIn signIn(Ref ref) {
  return SignIn(ref.read(authRepositoryProvider));
}

@riverpod
Logout logout(Ref ref) {
  return Logout(ref.read(authRepositoryProvider));
}

@riverpod
RequestPasswordResetCode requestPasswordResetCode(Ref ref) {
  return RequestPasswordResetCode(ref.read(authRepositoryProvider));
}

@riverpod
ResetPassword resetPassword(Ref ref) {
  return ResetPassword(ref.read(authRepositoryProvider));
}

@riverpod
ResendConfirmationLink resendConfirmationLink(Ref ref) {
  return ResendConfirmationLink(ref.read(authRepositoryProvider));
}

// 5. authViewModelProvider is auto-generated from @riverpod in auth_viewmodel.dart
