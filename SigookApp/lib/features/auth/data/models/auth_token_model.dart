import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';
import 'user_info_model.dart';

part 'auth_token_model.freezed.dart';
part 'auth_token_model.g.dart';

@freezed
sealed class AuthTokenModel extends AuthToken with _$AuthTokenModel {
  const AuthTokenModel._();

  const factory AuthTokenModel({
    String? accessToken,
    String? idToken,
    String? refreshToken,
    int? expiresIn,
    String? tokenType,
    String? scope,
    UserInfoModel? userInfo,
  }) = _AuthTokenModel;

  factory AuthTokenModel.fromJson(Map<String, dynamic> json) =>
      _$AuthTokenModelFromJson(json);

  factory AuthTokenModel.fromResponse(dynamic response) {
    // Parse user info from id_token if available
    UserInfoModel? userInfo;
    if (response.idToken != null) {
      try {
        final decodedToken = JwtDecoder.decode(response.idToken);
        userInfo = UserInfoModel.fromIdTokenClaims(decodedToken);
      } catch (e) {
        // If token parsing fails, continue without user info
        userInfo = null;
      }
    }

    return AuthTokenModel(
      accessToken: response.accessToken,
      idToken: response.idToken,
      refreshToken: response.refreshToken,
      expiresIn: response.expiresIn,
      tokenType: response.tokenType ?? 'Bearer',
      scope: response.scope,
      userInfo: userInfo,
    );
  }

  @override
  bool get isValid =>
      (accessToken != null && accessToken!.isNotEmpty) &&
      (expiresIn != null && expiresIn! > 0);
}
