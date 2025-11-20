// lib/features/auth/presentation/providers/auth_providers.dart

import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:riverpod/riverpod.dart';
import 'package:sigook_app_flutter/core/providers/core_providers.dart';
import 'package:sigook_app_flutter/features/catalog/presentation/providers/catalog_providers.dart';

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

// 1. Datasources
final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>((ref) {
  return AuthRemoteDataSource(
    dio: ref.read(apiClientProvider).dio,
    networkInfo: ref.read(networkInfoProvider),
  );
});

final authLocalDataSourceProvider = Provider<AuthLocalDataSource>((ref) {
  return AuthLocalDataSourceImpl(
    sharedPreferences: ref.read(sharedPreferencesProvider),
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
