import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../../features/auth/data/datasources/auth_local_datasource.dart';

/// Dio interceptor that automatically adds auth tokens and refreshes them on 401
class AuthInterceptor extends QueuedInterceptorsWrapper {
  final Ref ref;
  final AuthRepository authRepository;
  final AuthLocalDataSource localDataSource;
  final Dio dio;

  AuthInterceptor({
    required this.ref,
    required this.authRepository,
    required this.localDataSource,
    required this.dio,
  });

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Get the cached token
    final cachedToken = await localDataSource.getCachedToken();

    // Add Authorization header if token exists
    if (cachedToken?.accessToken != null) {
      options.headers['Authorization'] = 'Bearer ${cachedToken!.accessToken}';
      options.headers['Accept'] = 'application/json';
    }

    return handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    // Only handle 401 Unauthorized errors
    if (err.response?.statusCode == 401) {
      try {
        // Get the current refresh token
        final cachedToken = await localDataSource.getCachedToken();

        if (cachedToken?.refreshToken == null) {
          // No refresh token available, can't recover
          return handler.next(err);
        }

        // Attempt to refresh the token
        final result = await authRepository.refreshToken(
          cachedToken!.refreshToken!,
        );

        // Check if refresh was successful
        await result.fold(
          (failure) async {
            // Refresh failed, can't recover
            return handler.next(err);
          },
          (newToken) async {
            // Token refreshed successfully
            // Retry the original request with new token
            final requestOptions = err.requestOptions;
            requestOptions.headers['Authorization'] =
                'Bearer ${newToken.accessToken}';

            try {
              final response = await dio.fetch(requestOptions);
              return handler.resolve(response);
            } catch (e) {
              return handler.next(err);
            }
          },
        );
      } catch (e) {
        // If anything goes wrong during refresh, pass the original error
        return handler.next(err);
      }
    } else {
      // Not a 401 error, pass it through
      return handler.next(err);
    }
  }
}
