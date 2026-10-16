import 'package:equatable/equatable.dart';
import 'user_info.dart';

class AuthToken extends Equatable {
  static const Duration defaultExpiryLeeway = Duration(seconds: 60);

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

  bool get hasAccessToken => accessToken != null && accessToken!.isNotEmpty;

  bool get hasRefreshToken => refreshToken != null && refreshToken!.isNotEmpty;

  bool isExpired({Duration leeway = Duration.zero}) {
    final expiration = expirationDateTime;
    if (expiration == null) return true;
    return !expiration.isAfter(DateTime.now().add(leeway));
  }

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
