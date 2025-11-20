import 'package:equatable/equatable.dart';

/// User information extracted from ID token
class UserInfo extends Equatable {
  final String? sub; // Subject (user ID)
  final String? name; // Full name
  final String? givenName; // First name
  final String? familyName; // Last name
  final String? email;
  final bool? emailVerified;
  final List<String>? roles; // User roles from token claims

  const UserInfo({
    this.sub,
    this.name,
    this.givenName,
    this.familyName,
    this.email,
    this.emailVerified,
    this.roles,
  });

  factory UserInfo.empty() => const UserInfo();

  factory UserInfo.fromIdTokenClaims(Map<String, dynamic> claims) {
    // Extract roles from claims - could be in different claim names
    List<String>? roles;
    if (claims.containsKey('role')) {
      final roleValue = claims['role'];
      if (roleValue is List) {
        roles = roleValue.map((e) => e.toString()).toList();
      } else if (roleValue is String) {
        roles = [roleValue];
      }
    } else if (claims.containsKey('roles')) {
      final rolesValue = claims['roles'];
      if (rolesValue is List) {
        roles = rolesValue.map((e) => e.toString()).toList();
      }
    }

    return UserInfo(
      sub: claims['sub']?.toString(),
      name: claims['name']?.toString(),
      givenName: claims['given_name']?.toString(),
      familyName: claims['family_name']?.toString(),
      email: claims['email']?.toString(),
      emailVerified: claims['email_verified'] as bool?,
      roles: roles,
    );
  }

  bool get hasRole => roles != null && roles!.isNotEmpty;

  String? get displayName => name ?? givenName ?? email ?? sub;

  @override
  List<Object?> get props => [
    sub,
    name,
    givenName,
    familyName,
    email,
    emailVerified,
    roles,
  ];
}
