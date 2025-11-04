// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'registration_form_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_$RegistrationFormModelImpl _$$RegistrationFormModelImplFromJson(
  Map<String, dynamic> json,
) => _$RegistrationFormModelImpl(
  personalInfo: json['personalInfo'] == null
      ? null
      : PersonalInfoModel.fromJson(
          json['personalInfo'] as Map<String, dynamic>,
        ),
  contactInfo: json['contactInfo'] == null
      ? null
      : ContactInfoModel.fromJson(json['contactInfo'] as Map<String, dynamic>),
  addressInfo: json['addressInfo'] == null
      ? null
      : AddressInfoModel.fromJson(json['addressInfo'] as Map<String, dynamic>),
  availabilityInfo: json['availabilityInfo'] == null
      ? null
      : AvailabilityInfoModel.fromJson(
          json['availabilityInfo'] as Map<String, dynamic>,
        ),
  professionalInfo: json['professionalInfo'] == null
      ? null
      : ProfessionalInfoModel.fromJson(
          json['professionalInfo'] as Map<String, dynamic>,
        ),
);

Map<String, dynamic> _$$RegistrationFormModelImplToJson(
  _$RegistrationFormModelImpl instance,
) => <String, dynamic>{
  'personalInfo': instance.personalInfo,
  'contactInfo': instance.contactInfo,
  'addressInfo': instance.addressInfo,
  'availabilityInfo': instance.availabilityInfo,
  'professionalInfo': instance.professionalInfo,
};
