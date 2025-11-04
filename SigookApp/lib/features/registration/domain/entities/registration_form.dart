import 'package:equatable/equatable.dart';
import 'personal_info.dart';
import 'contact_info.dart';
import 'address_info.dart';
import 'availability_info.dart';
import 'professional_info.dart';

/// Complete registration form entity
/// Aggregates all form sections
class RegistrationForm extends Equatable {
  final PersonalInfo? personalInfo;
  final ContactInfo? contactInfo;
  final AddressInfo? addressInfo;
  final AvailabilityInfo? availabilityInfo;
  final ProfessionalInfo? professionalInfo;

  const RegistrationForm({
    this.personalInfo,
    this.contactInfo,
    this.addressInfo,
    this.availabilityInfo,
    this.professionalInfo,
  });

  /// Empty form constructor
  factory RegistrationForm.empty() {
    return const RegistrationForm();
  }

  /// Validates if all sections are complete and valid
  bool get isComplete {
    return personalInfo != null &&
        contactInfo != null &&
        addressInfo != null &&
        availabilityInfo != null &&
        professionalInfo != null &&
        personalInfo!.isValid &&
        contactInfo!.isValid &&
        addressInfo!.isValid &&
        availabilityInfo!.isValid &&
        professionalInfo!.isValid;
  }

  /// Checks which sections are completed
  bool get isPersonalInfoComplete => personalInfo?.isValid ?? false;
  bool get isContactInfoComplete => contactInfo?.isValid ?? false;
  bool get isAddressInfoComplete => addressInfo?.isValid ?? false;
  bool get isAvailabilityInfoComplete => availabilityInfo?.isValid ?? false;
  bool get isProfessionalInfoComplete => professionalInfo?.isValid ?? false;

  /// Calculates completion percentage
  double get completionPercentage {
    int completed = 0;
    if (isPersonalInfoComplete) completed++;
    if (isContactInfoComplete) completed++;
    if (isAddressInfoComplete) completed++;
    if (isAvailabilityInfoComplete) completed++;
    if (isProfessionalInfoComplete) completed++;
    return completed / 5;
  }

  /// Creates a copy with updated fields
  RegistrationForm copyWith({
    PersonalInfo? personalInfo,
    ContactInfo? contactInfo,
    AddressInfo? addressInfo,
    AvailabilityInfo? availabilityInfo,
    ProfessionalInfo? professionalInfo,
  }) {
    return RegistrationForm(
      personalInfo: personalInfo ?? this.personalInfo,
      contactInfo: contactInfo ?? this.contactInfo,
      addressInfo: addressInfo ?? this.addressInfo,
      availabilityInfo: availabilityInfo ?? this.availabilityInfo,
      professionalInfo: professionalInfo ?? this.professionalInfo,
    );
  }

  @override
  List<Object?> get props => [
    personalInfo,
    contactInfo,
    addressInfo,
    availabilityInfo,
    professionalInfo,
  ];
}
