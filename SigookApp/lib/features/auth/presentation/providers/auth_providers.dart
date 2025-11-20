// lib/features/auth/presentation/providers/auth_providers.dart

import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:riverpod/riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../../core/network/auth_interceptor.dart';

import '../../data/datasources/auth_local_datasource.dart';
import '../../data/datasources/auth_remote_datasource.dart';
import '../../data/repositories/auth_repository_impl.dart';
import '../../domain/entities/auth_token.dart';
import '../../domain/repositories/auth_repository.dart';
import '../../domain/usecases/sign_in.dart';
import '../../domain/usecases/refresh_token.dart';
import '../../domain/usecases/logout.dart';
import '../viewmodels/auth_viewmodel.dart';

part 'auth_providers.g.dart';

// 0. Auth Interceptor for automatic token refresh on 401 errors
final authInterceptorProvider = Provider<AuthInterceptor>((ref) {
  return AuthInterceptor(
    ref: ref,
    authRepository: ref.read(authRepositoryProvider),
    localDataSource: ref.read(authLocalDataSourceProvider),
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

// 1. Datasources
final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>((ref) {
  return AuthRemoteDataSource(
    dio: ref.read(apiClientProvider).dio,
    networkInfo: ref.read(networkInfoProvider),
  );
});

final authLocalDataSourceProvider = Provider<AuthLocalDataSource>((ref) {
  return AuthLocalDataSourceImpl(
    secureStorage: ref.read(secureStorageProvider),
  );
});

// 2. Repository
final authRepositoryProvider = Provider<AuthRepository>((ref) {
  return AuthRepositoryImpl(
    remote: ref.read(authRemoteDataSourceProvider),
    local: ref.read(authLocalDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

// 3. UseCases
@riverpod
SignIn signIn(Ref ref) {
  return SignIn(ref.read(authRepositoryProvider));
}

@riverpod
RefreshToken refreshToken(Ref ref) {
  return RefreshToken(ref.read(authRepositoryProvider));
}

@riverpod
Logout logout(Ref ref) {
  return Logout(ref.read(authRepositoryProvider));
}

// 4. authViewModelProvider is auto-generated from @riverpod in auth_viewmodel.dart

// 5. Token actual (para usar en interceptors de Dio, etc.)
@riverpod
AuthToken? currentAuthToken(Ref ref) {
  return ref.watch(authViewModelProvider).token;
}
