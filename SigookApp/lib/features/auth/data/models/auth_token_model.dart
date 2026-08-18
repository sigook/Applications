import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';
import 'user_info_model.dart';

part 'auth_token_model.freezed.dart';
part 'auth_token_model.g.dart';

@freezed
abstract class AuthTokenModel with _$AuthTokenModel {
  const AuthTokenModel._();

  const factory AuthTokenModel({
    String? accessToken,
    String? idToken,
    String? refreshToken,
    DateTime? expirationDateTime,
    String? tokenType,
    List<String>? scopes,
    UserInfoModel? userInfo,
  }) = _AuthTokenModel;

  factory AuthTokenModel.fromJson(Map<String, dynamic> json) =>
      _$AuthTokenModelFromJson(json);

  factory AuthTokenModel.fromResponse(dynamic response) {
    UserInfoModel? userInfo;
    if (response.idToken != null) {
      try {
        final decodedToken = JwtDecoder.decode(response.idToken);
        userInfo = UserInfoModel.fromIdTokenClaims(decodedToken);
      } catch (e) {
        userInfo = null;
      }
    }

    return AuthTokenModel(
      accessToken: response.accessToken,
      idToken: response.idToken,
      refreshToken: response.refreshToken,
      expirationDateTime: response.accessTokenExpirationDateTime,
      tokenType: response.tokenType ?? 'Bearer',
      scopes: response.scopes,
      userInfo: userInfo,
    );
  }

  factory AuthTokenModel.fromLoginResponse(Map<String, dynamic> json) {
    final idToken = json['idToken'] as String?;
    UserInfoModel? userInfo;
    if (idToken != null && idToken.isNotEmpty) {
      try {
        final decodedToken = JwtDecoder.decode(idToken);
        userInfo = UserInfoModel.fromIdTokenClaims(decodedToken);
      } catch (e) {
        userInfo = null;
      }
    }

    DateTime? expiration;
    final expirationRaw = json['expirationDateTime'];
    if (expirationRaw is String) {
      expiration = DateTime.tryParse(expirationRaw);
    }
    final expiresIn = json['expiresIn'];
    if (expiration == null && expiresIn is num) {
      expiration = DateTime.now().add(Duration(seconds: expiresIn.toInt()));
    }

    List<String>? scopes;
    final scopesRaw = json['scopes'] ?? json['scope'];
    if (scopesRaw is List) {
      scopes = scopesRaw.map((e) => e.toString()).toList();
    } else if (scopesRaw is String && scopesRaw.isNotEmpty) {
      scopes = scopesRaw.split(' ');
    }

    return AuthTokenModel(
      accessToken: json['accessToken'] as String?,
      idToken: idToken,
      refreshToken: json['refreshToken'] as String?,
      expirationDateTime: expiration,
      tokenType: json['tokenType'] as String? ?? 'Bearer',
      scopes: scopes,
      userInfo: userInfo,
    );
  }

  bool get isValid =>
      (accessToken != null && accessToken!.isNotEmpty) &&
      (expirationDateTime != null);

  AuthToken toEntity() {
    return AuthToken(
      accessToken: accessToken,
      idToken: idToken,
      refreshToken: refreshToken,
      expirationDateTime: expirationDateTime,
      tokenType: tokenType,
      scopes: scopes,
      userInfo: userInfo?.toEntity(),
    );
  }
}
