import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';

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
  }) = _AuthTokenModel;

  factory AuthTokenModel.fromJson(Map<String, dynamic> json) =>
      _$AuthTokenModelFromJson(json);

  factory AuthTokenModel.fromResponse(dynamic response) {
    return AuthTokenModel(
      accessToken: response.accessToken,
      idToken: response.idToken,
      refreshToken: response.refreshToken,
      expiresIn: response.expiresIn,
      tokenType: response.tokenType ?? 'Bearer',
      scope: response.scope,
    );
  }

  @override
  bool get isValid =>
      (accessToken != null && accessToken!.isNotEmpty) &&
      (expiresIn != null && expiresIn! > 0);
}
