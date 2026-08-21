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

  factory AuthTokenModel.fromTokenResponse(Map<String, dynamic> json) {
    DateTime? expiration;
    final expiresIn = json['expires_in'];
    if (expiresIn is num) {
      expiration = DateTime.now().add(Duration(seconds: expiresIn.toInt()));
    }

    List<String>? scopes;
    final scope = json['scope'];
    if (scope is String && scope.isNotEmpty) {
      scopes = scope.split(' ');
    }

    return AuthTokenModel(
      accessToken: json['access_token'] as String?,
      refreshToken: json['refresh_token'] as String?,
      expirationDateTime: expiration,
      tokenType: json['token_type'] as String? ?? 'Bearer',
      scopes: scopes,
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
