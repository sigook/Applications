import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../models/auth_token_model.dart';

abstract class AuthLocalDataSource {
  Future<AuthTokenModel?> getCachedToken();
  Future<void> cacheToken(AuthTokenModel token);
  Future<void> clearToken();
}

class AuthLocalDataSourceImpl implements AuthLocalDataSource {
  final FlutterSecureStorage secureStorage;
  static const String cachedTokenKey = 'CACHED_AUTH_TOKEN';

  AuthLocalDataSourceImpl({required this.secureStorage});

  @override
  Future<AuthTokenModel?> getCachedToken() async {
    final jsonString = await secureStorage.read(key: cachedTokenKey);
    if (jsonString != null) {
      return AuthTokenModel.fromJson(json.decode(jsonString));
    }
    return null;
  }

  @override
  Future<void> cacheToken(AuthTokenModel token) async {
    final jsonString = json.encode(token.toJson());
    await secureStorage.write(key: cachedTokenKey, value: jsonString);
  }

  @override
  Future<void> clearToken() async {
    await secureStorage.delete(key: cachedTokenKey);
  }
}
