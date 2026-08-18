import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_appauth/flutter_appauth.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/constants/error_messages.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/dio_error_interceptor.dart';
import '../../../../core/network/network_info.dart';
import '../models/auth_token_model.dart';

abstract class AuthRemoteDataSource {
  Future<AuthTokenModel> signIn({required String email, required String password});
  Future<void> logout(String idToken);
  Future<AuthTokenModel> refreshToken(String currentRefreshToken);
  Future<bool> validateToken(String accessToken);
  Future<String> getUserRole(String accessToken);
  Future<void> deactivateAccount(String accessToken);
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
      // Placeholder endpoint: the backend does not expose it yet. Assumed
      // contract — 200 { accessToken, idToken, refreshToken, expiresIn,
      // tokenType, scopes }; 400/401 = invalid credentials.
      final response = await anonymousDio.post(
        '/Account/Login',
        data: {'email': email, 'password': password},
      );

      return AuthTokenModel.fromLoginResponse(
        Map<String, dynamic>.from(response.data as Map),
      );
    } on DioException catch (e) {
      final statusCode = e.response?.statusCode;
      if (statusCode == 400 || statusCode == 401) {
        throw ServerException(
          message: ErrorMessages.invalidCredentials,
          statusCode: statusCode,
        );
      }
      handleDioException(e);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Authentication error: ${e.toString()}');
    }
  }

  @override
  Future<void> logout(String idToken) async {
    // Skip browser-based endSession: the IdentityServer's registered
    // PostLogoutRedirectUri (com.sigook:/oauth2logout) does not match the
    // env POST_LOGOUT_REDIRECT_URI, so the server ignores the redirect
    // parameter and lands on the Sigook web home page instead of returning
    // to the app. For a standalone mobile app, clearing local tokens is
    // sufficient — the user will need to re-authenticate on next login.
    debugPrint('✅ [LOGOUT] Local logout — tokens will be cleared by repository');
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
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final userInfoUrl = '${EnvironmentConfig.authority}/connect/userinfo';
      debugPrint('🔐 [AUTH] Fetching user role from $userInfoUrl');

      final response = await dio.get(
        userInfoUrl,
        options: Options(
          headers: {'Authorization': 'Bearer $accessToken'},
        ),
      );

      final data = response.data as Map<String, dynamic>;
      final role = data['role'] as String? ?? '';
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

  @override
  Future<bool> validateToken(String accessToken) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final response = await dio.get(
        '/auth/validate',
        options: Options(headers: {'Authorization': 'Bearer $accessToken'}),
      );

      return response.statusCode == 200;
    } on DioException catch (e) {
      if (e.response?.statusCode == 401 || e.response?.statusCode == 403) {
        return false;
      }
      throw ServerException(message: 'Token validation error: ${e.message}');
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Token validation error: ${e.toString()}');
    }
  }
}
