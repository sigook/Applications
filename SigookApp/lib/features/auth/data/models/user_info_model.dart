import '../../domain/entities/user_info.dart';

class UserInfoModel extends UserInfo {
  const UserInfoModel({
    super.sub,
    super.name,
    super.givenName,
    super.familyName,
    super.email,
    super.emailVerified,
    super.roles,
  });

  factory UserInfoModel.fromJson(Map<String, dynamic> json) {
    List<String>? roles;
    if (json['roles'] != null) {
      roles = (json['roles'] as List).map((e) => e.toString()).toList();
    }

    return UserInfoModel(
      sub: json['sub']?.toString(),
      name: json['name']?.toString(),
      givenName:
          json['given_name']?.toString() ?? json['givenName']?.toString(),
      familyName:
          json['family_name']?.toString() ?? json['familyName']?.toString(),
      email: json['email']?.toString(),
      emailVerified:
          json['email_verified'] as bool? ?? json['emailVerified'] as bool?,
      roles: roles,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'sub': sub,
      'name': name,
      'givenName': givenName,
      'familyName': familyName,
      'email': email,
      'emailVerified': emailVerified,
      'roles': roles,
    };
  }

  factory UserInfoModel.fromIdTokenClaims(Map<String, dynamic> claims) {
    final userInfo = UserInfo.fromIdTokenClaims(claims);
    return UserInfoModel(
      sub: userInfo.sub,
      name: userInfo.name,
      givenName: userInfo.givenName,
      familyName: userInfo.familyName,
      email: userInfo.email,
      emailVerified: userInfo.emailVerified,
      roles: userInfo.roles,
    );
  }
}
