import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/registration_form.dart';
import 'personal_info_model.dart';
import 'contact_info_model.dart';
import 'address_info_model.dart';
import 'availability_info_model.dart';
import 'professional_info_model.dart';

part 'registration_form_model.freezed.dart';
part 'registration_form_model.g.dart';

@freezed
class RegistrationFormModel with _$RegistrationFormModel {
  const RegistrationFormModel._();

  const factory RegistrationFormModel({
    PersonalInfoModel? personalInfo,
    ContactInfoModel? contactInfo,
    AddressInfoModel? addressInfo,
    AvailabilityInfoModel? availabilityInfo,
    ProfessionalInfoModel? professionalInfo,
  }) = _RegistrationFormModel;

  /// Convert from domain entity
  factory RegistrationFormModel.fromEntity(RegistrationForm entity) {
    return RegistrationFormModel(
      personalInfo: entity.personalInfo != null
          ? PersonalInfoModel.fromEntity(entity.personalInfo!)
          : null,
      contactInfo: entity.contactInfo != null
          ? ContactInfoModel.fromEntity(entity.contactInfo!)
          : null,
      addressInfo: entity.addressInfo != null
          ? AddressInfoModel.fromEntity(entity.addressInfo!)
          : null,
      availabilityInfo: entity.availabilityInfo != null
          ? AvailabilityInfoModel.fromEntity(entity.availabilityInfo!)
          : null,
      professionalInfo: entity.professionalInfo != null
          ? ProfessionalInfoModel.fromEntity(entity.professionalInfo!)
          : null,
    );
  }

  /// Convert to domain entity
  RegistrationForm toEntity() {
    return RegistrationForm(
      personalInfo: personalInfo?.toEntity(),
      contactInfo: contactInfo?.toEntity(),
      addressInfo: addressInfo?.toEntity(),
      availabilityInfo: availabilityInfo?.toEntity(),
      professionalInfo: professionalInfo?.toEntity(),
    );
  }

  /// Empty form
  factory RegistrationFormModel.empty() {
    return const RegistrationFormModel();
  }

  /// From JSON
  factory RegistrationFormModel.fromJson(Map<String, dynamic> json) =>
      _$RegistrationFormModelFromJson(json);
}
