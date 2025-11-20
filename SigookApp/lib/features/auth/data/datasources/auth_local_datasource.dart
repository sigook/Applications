import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';
import '../models/auth_token_model.dart';

abstract class AuthLocalDataSource {
  Future<AuthTokenModel?> getCachedToken();
  Future<void> cacheToken(AuthTokenModel token);
  Future<void> clearToken();
}

class AuthLocalDataSourceImpl implements AuthLocalDataSource {
  final SharedPreferences sharedPreferences;
  static const String cachedTokenKey = 'CACHED_AUTH_TOKEN';

  AuthLocalDataSourceImpl({required this.sharedPreferences});

  @override
  Future<AuthTokenModel?> getCachedToken() async {
    final jsonString = sharedPreferences.getString(cachedTokenKey);
    if (jsonString != null) {
      return AuthTokenModel.fromJson(json.decode(jsonString));
    }
    return null;
  }

  @override
  Future<void> cacheToken(AuthTokenModel token) async {
    final jsonString = json.encode(token.toJson());
    await sharedPreferences.setString(cachedTokenKey, jsonString);
  }

  @override
  Future<void> clearToken() async {
    await sharedPreferences.remove(cachedTokenKey);
  }
}
