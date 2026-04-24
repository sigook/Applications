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
  Future<String> getUserRole(String accessToken);
  Future<void> deactivateAccount(String accessToken);
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
        // Always require credential entry. flutter_appauth uses Chrome Custom
        // Tabs (Chrome cookie store) while LogoutWebviewPage uses the Android
        // WebView (a separate cookie store), so Chrome may still hold a valid
        // SSO session after logout. This prompt forces re-authentication.
        promptValues: ['login'],
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
