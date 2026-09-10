import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import '../../features/auth/data/datasources/auth_local_datasource.dart';
import '../../features/auth/domain/entities/auth_token.dart';
import '../../features/auth/domain/repositories/auth_repository.dart';
import '../constants/error_messages.dart';
import '../error/failures.dart';

class AuthInterceptor extends QueuedInterceptorsWrapper {
  static const String sessionExpiredExtraKey = 'sigook.sessionExpired';

  final AuthRepository authRepository;
  final AuthLocalDataSource localDataSource;
  final Dio retryDio;
  final VoidCallback onSessionExpired;
  final Duration expiryLeeway;

  String? _rejectedRefreshToken;

  AuthInterceptor({
    required this.authRepository,
    required this.localDataSource,
    required this.retryDio,
    required this.onSessionExpired,
    this.expiryLeeway = AuthToken.defaultExpiryLeeway,
  });

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    var handled = false;

    void proceed([String? accessToken]) {
      handled = true;
      if (accessToken != null) _stampBearer(options, accessToken);
      handler.next(options);
    }

    void rejectExpired() {
      handled = true;
      _rejectSessionExpired(options, handler);
    }

    try {
      final cachedToken = await localDataSource.getCachedToken();
      final accessToken = cachedToken?.accessToken;
      if (cachedToken == null || accessToken == null || accessToken.isEmpty) {
        return proceed();
      }

      final refreshToken = cachedToken.refreshToken;
      if (!cachedToken.isExpired(leeway: expiryLeeway) ||
          refreshToken == null ||
          refreshToken.isEmpty) {
        return proceed(accessToken);
      }

      if (refreshToken == _rejectedRefreshToken) return rejectExpired();

      final result = await authRepository.refreshToken(refreshToken);
      result.fold(
        (failure) {
          if (failure.isDefinitiveAuthFailure) {
            _markSessionExpired(refreshToken);
            rejectExpired();
          } else {
            proceed(accessToken);
          }
        },
        (refreshedToken) => proceed(refreshedToken.accessToken ?? accessToken),
      );
    } catch (e) {
      debugPrint('🔐 [AUTH_INTERCEPTOR] onRequest failed: $e');
      if (!handled) handler.next(options);
    }
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    var handled = false;

    void passthrough() {
      handled = true;
      handler.next(err);
    }

    try {
      if (err.response?.statusCode != 401 ||
          err.requestOptions.extra[sessionExpiredExtraKey] == true) {
        return passthrough();
      }

      final cachedToken = await localDataSource.getCachedToken();
      final refreshToken = cachedToken?.refreshToken;
      if (cachedToken == null || refreshToken == null || refreshToken.isEmpty) {
        return passthrough();
      }

      final cachedAccessToken = cachedToken.accessToken;
      final sentAuthorization = err.requestOptions.headers['Authorization'];
      if (cachedAccessToken != null &&
          cachedAccessToken.isNotEmpty &&
          sentAuthorization != 'Bearer $cachedAccessToken') {
        handled = true;
        return _retry(err, cachedAccessToken, handler);
      }

      if (refreshToken == _rejectedRefreshToken) return passthrough();

      final result = await authRepository.refreshToken(refreshToken);
      handled = true;
      await result.fold(
        (failure) async {
          if (failure.isDefinitiveAuthFailure) {
            _markSessionExpired(refreshToken);
          }
          handler.next(err);
        },
        (refreshedToken) async {
          final accessToken = refreshedToken.accessToken;
          if (accessToken == null || accessToken.isEmpty) {
            handler.next(err);
            return;
          }
          await _retry(err, accessToken, handler);
        },
      );
    } catch (e) {
      debugPrint('🔐 [AUTH_INTERCEPTOR] onError failed: $e');
      if (!handled) handler.next(err);
    }
  }

  Future<void> _retry(
    DioException err,
    String accessToken,
    ErrorInterceptorHandler handler,
  ) async {
    final options = err.requestOptions;
    _stampBearer(options, accessToken);
    final body = options.data;
    if (body is FormData) options.data = body.clone();

    try {
      final response = await retryDio.fetch<dynamic>(options);
      handler.resolve(response);
    } on DioException catch (e) {
      handler.next(e);
    } catch (_) {
      handler.next(err);
    }
  }

  void _rejectSessionExpired(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) {
    options.extra = {...options.extra, sessionExpiredExtraKey: true};
    handler.reject(
      DioException(
        requestOptions: options,
        type: DioExceptionType.badResponse,
        response: Response<dynamic>(requestOptions: options, statusCode: 401),
        message: ErrorMessages.tokenExpired,
      ),
      true,
    );
  }

  void _markSessionExpired(String refreshToken) {
    _rejectedRefreshToken = refreshToken;
    try {
      onSessionExpired();
    } catch (e) {
      debugPrint('🔐 [AUTH_INTERCEPTOR] onSessionExpired failed: $e');
    }
  }

  void _stampBearer(RequestOptions options, String accessToken) {
    options.headers['Authorization'] = 'Bearer $accessToken';
    options.headers['Accept'] = 'application/json';
  }
}
