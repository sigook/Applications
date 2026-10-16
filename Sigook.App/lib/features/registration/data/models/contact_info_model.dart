import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/contact_info.dart';
import '../../domain/entities/value_objects/email.dart';
import '../../domain/entities/value_objects/password.dart';
import '../../domain/entities/identification_type.dart';

part 'contact_info_model.freezed.dart';
part 'contact_info_model.g.dart';

@freezed
sealed class ContactInfoModel with _$ContactInfoModel {
  const ContactInfoModel._();

  const factory ContactInfoModel({
    required String email,
    required String password,
    required String identification,
    String? identificationTypeId, // May be null from API
    required String identificationTypeName,
    required String mobileNumber,
  }) = _ContactInfoModel;

  /// Convert from domain entity
  factory ContactInfoModel.fromEntity(ContactInfo entity) {
    return ContactInfoModel(
      email: entity.email.value,
      password: entity.password.value,
      identification: entity.identification,
      identificationTypeId: entity.identificationType.id, // May be null
      identificationTypeName: entity.identificationType.value,
      mobileNumber: entity.mobileNumber,
    );
  }

  /// Convert to domain entity
  ContactInfo toEntity() {
    return ContactInfo(
      email: Email(email),
      password: Password(password),
      identification: identification,
      identificationType: IdentificationType(
        id: identificationTypeId,
        value: identificationTypeName,
      ),
      mobileNumber: mobileNumber,
    );
  }

  /// From JSON
  factory ContactInfoModel.fromJson(Map<String, dynamic> json) =>
      _$ContactInfoModelFromJson(json);
}
