import 'package:dio/dio.dart';
import 'package:flutter_appauth/flutter_appauth.dart';
import '../../../../core/config/environment.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/network_info.dart';
import '../models/auth_token_model.dart';

class AuthRemoteDataSource {
  final Dio dio;
  final NetworkInfo networkInfo;
  final FlutterAppAuth flutterAppAuth = FlutterAppAuth();

  AuthRemoteDataSource({required this.dio, required this.networkInfo});

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

      final AuthorizationTokenResponse result = await flutterAppAuth
          .authorizeAndExchangeCode(request);

      return AuthTokenModel.fromResponse(result);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Authentication error: ${e.toString()}');
    }
  }

  Future<void> logout() async {
    if (!(await networkInfo.isConnected)) {
      throw NetworkException('No internet connection');
    }

    try {
      await dio.post('/logout');
    } on DioException catch (e) {
      throw ServerException(
        message: e.response?.data['message'] ?? 'Logout failed',
        statusCode: e.response?.statusCode,
      );
    }
  }

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

      final TokenResponse result = await flutterAppAuth.token(request);

      return AuthTokenModel.fromResponse(result);
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Token refresh error: ${e.toString()}');
    }
  }
}
