import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/contact_info.dart';
import '../../domain/entities/value_objects/email.dart';
import '../../domain/entities/value_objects/password.dart';

part 'contact_info_model.freezed.dart';
part 'contact_info_model.g.dart';

@freezed
class ContactInfoModel with _$ContactInfoModel {
  const ContactInfoModel._();

  const factory ContactInfoModel({
    required String email,
    required String password,
  }) = _ContactInfoModel;

  /// Convert from domain entity
  factory ContactInfoModel.fromEntity(ContactInfo entity) {
    return ContactInfoModel(
      email: entity.email.value,
      password: entity.password.value,
    );
  }

  /// Convert to domain entity
  ContactInfo toEntity() {
    return ContactInfo(
      email: Email(email),
      password: Password(password),
    );
  }

  /// From JSON
  factory ContactInfoModel.fromJson(Map<String, dynamic> json) =>
      _$ContactInfoModelFromJson(json);
}
