import 'package:equatable/equatable.dart';
import 'user_info.dart';

class AuthToken extends Equatable {
  final String? accessToken;
  final String? idToken;
  final String? refreshToken;
  final DateTime? expirationDateTime;
  final String? tokenType;
  final List<String>? scopes;
  final UserInfo? userInfo;

  const AuthToken({
    this.accessToken,
    this.idToken,
    this.refreshToken,
    this.expirationDateTime,
    this.tokenType,
    this.scopes,
    this.userInfo,
  });

  factory AuthToken.empty() => const AuthToken();

  bool get isValid =>
      (accessToken != null && accessToken!.isNotEmpty) &&
      (expirationDateTime != null);

  @override
  List<Object?> get props => [
    accessToken,
    idToken,
    refreshToken,
    expirationDateTime,
    tokenType,
    scopes,
    userInfo,
  ];
}
