import 'package:equatable/equatable.dart';

class AuthToken extends Equatable {
  final String? accessToken;
  final String? idToken;
  final String? refreshToken;
  final int? expiresIn;

  const AuthToken({
    this.accessToken,
    this.idToken,
    this.refreshToken,
    this.expiresIn,
  });

  factory AuthToken.empty() => const AuthToken();

  bool get isValid =>
      (accessToken != null && accessToken!.isNotEmpty) &&
      (expiresIn != null && expiresIn! > 0);

  @override
  List<Object?> get props => [accessToken, idToken, refreshToken, expiresIn];
}
