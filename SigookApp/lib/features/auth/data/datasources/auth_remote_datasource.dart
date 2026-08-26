import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_appauth/flutter_appauth.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/constants/auth_error_codes.dart';
import '../../../../core/constants/error_messages.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/dio_error_interceptor.dart';
import '../../../../core/network/network_info.dart';
import '../models/auth_token_model.dart';

abstract class AuthRemoteDataSource {
  Future<AuthTokenModel> signIn({required String email, required String password});
  Future<void> revokeRefreshToken(String refreshToken);
  Future<AuthTokenModel> refreshToken(String currentRefreshToken);
  Future<String> getUserRole(String accessToken);
  Future<void> deactivateAccount(String accessToken);
  Future<void> requestPasswordResetCode(String email);
  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  });
  Future<void> resendConfirmationLink(String email);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio dio;
  final Dio anonymousDio;
  final NetworkInfo networkInfo;
  final FlutterAppAuth appAuth;

  AuthRemoteDataSourceImpl({
    required this.dio,
    required this.anonymousDio,
    required this.networkInfo,
    required this.appAuth,
  });

  @override
  Future<AuthTokenModel> signIn({
    required String email,
    required String password,
  }) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final response = await anonymousDio.post(
        _authorityUrl('/connect/token'),
        data: {
          'grant_type': 'password',
          'client_id': EnvironmentConfig.clientId,
          'scope': EnvironmentConfig.scopes.join(' '),
          'username': email,
          'password': password,
        },
        options: Options(contentType: Headers.formUrlEncodedContentType),
      );

      return AuthTokenModel.fromTokenResponse(
        Map<String, dynamic>.from(response.data as Map),
      );
    } on DioException catch (e) {
      final statusCode = e.response?.statusCode;
      if (statusCode == 400 || statusCode == 401) {
        final data = e.response?.data;
        final code = data is Map ? data['error_description']?.toString() : null;
        throw ServerException(
          message: ErrorMessages.signInErrorFromCode(code),
          statusCode: statusCode,
          code: code,
        );
      }
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Authentication error: ${e.toString()}');
    }
  }

  String _roleFromAccessToken(String accessToken) {
    if (accessToken.isEmpty) return '';
    try {
      return _extractRole(JwtDecoder.decode(accessToken)['role']);
    } catch (_) {
      return '';
    }
  }

  String _extractRole(dynamic roleClaim) {
    if (roleClaim is String) return roleClaim;
    if (roleClaim is List) {
      final roles = roleClaim.map((r) => r.toString()).toList();
      return roles.firstWhere(
        (r) => r.toLowerCase() == 'worker',
        orElse: () => roles.isEmpty ? '' : roles.first,
      );
    }
    return '';
  }

  String _authorityUrl(String path) {
    final authority = EnvironmentConfig.authority;
    final base = authority.endsWith('/')
        ? authority.substring(0, authority.length - 1)
        : authority;
    return '$base$path';
  }

  @override
  Future<void> requestPasswordResetCode(String email) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      await anonymousDio.post(
        _authorityUrl('/Password/forgot'),
        data: {'email': email},
      );
    } on DioException catch (e) {
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(
        message: 'Password reset request error: ${e.toString()}',
      );
    }
  }

  @override
  Future<void> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  }) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      await anonymousDio.post(
        _authorityUrl('/Password/reset'),
        data: {'email': email, 'code': code, 'newPassword': newPassword},
      );
    } on DioException catch (e) {
      if (e.response?.statusCode == 400) {
        final data = e.response?.data;
        String? errorCode;
        var messages = const <String>[];
        if (data is Map) {
          errorCode = data['error']?.toString();
          final rawMessages = data['messages'];
          if (rawMessages is List) {
            messages = rawMessages.map((m) => m.toString()).toList();
          }
        }
        var message = ErrorMessages.resetErrorFromCode(errorCode);
        if (errorCode == AuthErrorCodes.passwordPolicy && messages.isNotEmpty) {
          message = '$message\n${messages.join('\n')}';
        }
        throw ServerException(
          message: message,
          statusCode: 400,
          code: errorCode,
        );
      }
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Password reset error: ${e.toString()}');
    }
  }

  @override
  Future<void> resendConfirmationLink(String email) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      await anonymousDio.post(
        _authorityUrl('/Account/ResendConfirmationLink'),
        queryParameters: {'userName': email},
      );
    } on DioException catch (e) {
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(
        message: 'Resend confirmation error: ${e.toString()}',
      );
    }
  }

  @override
  Future<void> revokeRefreshToken(String refreshToken) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      await anonymousDio.post(
        _authorityUrl('/connect/revocation'),
        data: {
          'token': refreshToken,
          'token_type_hint': 'refresh_token',
          'client_id': EnvironmentConfig.clientId,
        },
        options: Options(contentType: Headers.formUrlEncodedContentType),
      );
    } on DioException catch (e) {
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Token revocation error: ${e.toString()}');
    }
  }

  @override
  Future<AuthTokenModel> refreshToken(String currentRefreshToken) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final TokenRequest request = TokenRequest(
        EnvironmentConfig.clientId,
        EnvironmentConfig.redirectUri,
        issuer: EnvironmentConfig.authority,
        refreshToken: currentRefreshToken,
      );

      final TokenResponse result = await appAuth.token(request);

      return AuthTokenModel.fromResponse(result);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Token refresh error: ${e.toString()}');
    }
  }

  @override
  Future<String> getUserRole(String accessToken) async {
    final tokenRole = _roleFromAccessToken(accessToken);
    if (tokenRole.isNotEmpty) {
      debugPrint('🔐 [AUTH] User role from access token: $tokenRole');
      return tokenRole;
    }

    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final userInfoUrl = _authorityUrl('/connect/userinfo');
      debugPrint('🔐 [AUTH] Fetching user role from $userInfoUrl');

      final response = await dio.get(
        userInfoUrl,
        options: Options(
          headers: {'Authorization': 'Bearer $accessToken'},
        ),
      );

      final data = response.data as Map<String, dynamic>;
      final role = _extractRole(data['role']);
      debugPrint('🔐 [AUTH] User role: $role');
      return role;
    } on DioException catch (e) {
      throw ServerException(
        message: 'Failed to fetch user info: ${e.message}',
        statusCode: e.response?.statusCode,
      );
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to fetch user info: $e');
    }
  }

  @override
  Future<void> deactivateAccount(String accessToken) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }
    try {
      await dio.patch(
        '/identity',
        options: Options(headers: {'Authorization': 'Bearer $accessToken'}),
      );
    } on DioException catch (e) {
      throw ServerException(
        message: 'Failed to deactivate account: ${e.message}',
        statusCode: e.response?.statusCode,
      );
    }
  }
}
