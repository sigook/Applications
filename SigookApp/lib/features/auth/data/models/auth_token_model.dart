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
