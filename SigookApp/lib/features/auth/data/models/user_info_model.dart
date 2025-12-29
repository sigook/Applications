import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/user_info.dart';

part 'user_info_model.freezed.dart';
part 'user_info_model.g.dart';

@freezed
abstract class UserInfoModel with _$UserInfoModel {
  const UserInfoModel._();

  const factory UserInfoModel({
    String? sub,
    String? name,
    @JsonKey(name: 'given_name') String? givenName,
    @JsonKey(name: 'family_name') String? familyName,
    String? email,
    @JsonKey(name: 'email_verified') bool? emailVerified,
    @JsonKey(name: 'roles') List<String>? roles,
  }) = _UserInfoModel;

  factory UserInfoModel.fromJson(Map<String, dynamic> json) =>
      _$UserInfoModelFromJson(json);

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

  UserInfo toEntity() {
    return UserInfo(
      sub: sub,
      name: name,
      givenName: givenName,
      familyName: familyName,
      email: email,
      emailVerified: emailVerified,
      roles: roles,
    );
  }
}
