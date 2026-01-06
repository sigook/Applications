import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:flutter_appauth/flutter_appauth.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/network_info.dart';
import '../models/auth_token_model.dart';

abstract class AuthRemoteDataSource {
  Future<AuthTokenModel> signIn();
  Future<void> logout(String idToken);
  Future<AuthTokenModel> refreshToken(String currentRefreshToken);
  Future<bool> validateToken(String accessToken);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio dio;
  final NetworkInfo networkInfo;
  final FlutterAppAuth appAuth;

  AuthRemoteDataSourceImpl({
    required this.dio,
    required this.networkInfo,
    required this.appAuth,
  });

  @override
  Future<AuthTokenModel> signIn() async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final AuthorizationTokenRequest request = AuthorizationTokenRequest(
        EnvironmentConfig.clientId,
        EnvironmentConfig.redirectUri,
        issuer: EnvironmentConfig.authority,
        scopes: EnvironmentConfig.scopes,
      );

      final AuthorizationTokenResponse result = await appAuth
          .authorizeAndExchangeCode(request);

      return AuthTokenModel.fromResponse(result);
    } on PlatformException catch (e) {
      debugPrint('⚠️ PlatformException during sign-in: ${e.code}');
      debugPrint('   Details: ${e.details}');

      // Handle user cancellation (webview closed)
      if (e.code == 'authorize_and_exchange_code_failed' ||
          e.code == 'CANCELED' ||
          e.message?.toLowerCase().contains('user cancel') == true) {
        final details = e.details is Map
            ? Map<String, dynamic>.from(e.details as Map)
            : null;
        final userCancelled = details?['user_did_cancel'] == true;

        debugPrint('   User cancelled: $userCancelled');
        debugPrint('   Error code: ${e.code}');

        if (userCancelled || e.code == 'CANCELED') {
          debugPrint(
            '✅ User cancelled sign-in (closed webview) - treating as user action',
          );
          throw ServerException(message: 'User cancelled authentication');
        }
      }

      throw ServerException(message: 'Authentication failed: ${e.message}');
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Authentication error: ${e.toString()}');
    }
  }

  @override
  Future<void> logout(String idToken) async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      final request = EndSessionRequest(
        idTokenHint: idToken,
        postLogoutRedirectUrl: EnvironmentConfig.postLogoutRedirectUri,
        issuer: EnvironmentConfig.authority,
      );

      debugPrint('🔐 Starting logout session...');
      await appAuth.endSession(request);
      debugPrint('✅ Logout session completed successfully');
    } on PlatformException catch (e) {
      debugPrint('⚠️ PlatformException during logout: ${e.code}');
      debugPrint('   Details: ${e.details}');

      // Handle user cancellation (webview closed)
      if (e.code == 'end_session_failed' ||
          e.code == 'CANCELED' ||
          e.message?.toLowerCase().contains('user cancel') == true) {
        final details = e.details is Map
            ? Map<String, dynamic>.from(e.details as Map)
            : null;
        final userCancelled = details?['user_did_cancel'] == true;

        debugPrint('   User cancelled: $userCancelled');
        debugPrint('   Error code: ${e.code}');

        if (userCancelled || e.code == 'CANCELED') {
          debugPrint(
            '✅ User cancelled logout (closed webview) - treating as successful (tokens will be cleared)',
          );
          return;
        }
      }

      debugPrint('❌ Rethrowing as ServerException');
      throw ServerException(message: 'Logout failed: ${e.message}');
    } catch (e) {
      debugPrint('❌ Unexpected error during logout: $e');
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Logout failed: ${e.toString()}');
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
