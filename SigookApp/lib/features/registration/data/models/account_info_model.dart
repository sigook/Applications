import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/account_info.dart';
import '../../domain/entities/value_objects/email.dart';
import '../../domain/entities/value_objects/password.dart';

part 'account_info_model.freezed.dart';
part 'account_info_model.g.dart';

@freezed
sealed class AccountInfoModel with _$AccountInfoModel {
  const AccountInfoModel._();

  const factory AccountInfoModel({
    required String email,
    required String password,
    required String confirmPassword,
    required bool termsAccepted,
  }) = _AccountInfoModel;

  /// Convert from domain entity
  factory AccountInfoModel.fromEntity(AccountInfo entity) {
    return AccountInfoModel(
      email: entity.email.value,
      password: entity.password.value,
      confirmPassword: entity.confirmPassword,
      termsAccepted: entity.termsAccepted,
    );
  }

  /// Convert to domain entity
  AccountInfo toEntity() {
    return AccountInfo(
      email: Email(email),
      password: Password(password),
      confirmPassword: confirmPassword,
      termsAccepted: termsAccepted,
    );
  }

  /// From JSON
  factory AccountInfoModel.fromJson(Map<String, dynamic> json) =>
      _$AccountInfoModelFromJson(json);
}
