import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../../features/auth/data/datasources/auth_local_datasource.dart';

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
    final cachedToken = await localDataSource.getCachedToken();

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
    if (err.response?.statusCode == 401) {
      try {
        final cachedToken = await localDataSource.getCachedToken();

        if (cachedToken?.refreshToken == null) {
          return handler.next(err);
        }

        final result = await authRepository.refreshToken(
          cachedToken!.refreshToken!,
        );

        await result.fold(
          (failure) async {
            return handler.next(err);
          },
          (newToken) async {
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
        return handler.next(err);
      }
    } else {
      return handler.next(err);
    }
  }
}
