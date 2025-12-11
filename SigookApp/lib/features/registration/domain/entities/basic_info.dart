import 'package:equatable/equatable.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/zip_code.dart';
import 'value_objects/name.dart';
import 'value_objects/phone_number.dart';
import 'gender.dart';
import 'country.dart';
import 'province.dart';
import 'city.dart';
import 'value_objects/profile_photo.dart';

/// Basic information entity
/// Combines personal details, location, and mobile number
class BasicInfo extends Equatable {
  // Personal details
  final ProfilePhoto profilePhoto;
  final Name firstName;
  final Name lastName;
  final DateTime dateOfBirth;
  final Gender gender;

  // Location
  final Country? country;
  final Province? provinceState;
  final City? city;
  final String address;
  final ZipCode zipCode;

  // Contact
  final PhoneNumber mobileNumber;

  // Identification (optional - can be provided via documents)
  final String? identificationType;
  final String? identificationNumber;

  factory BasicInfo.empty() => BasicInfo(
    firstName: Name(''),
    lastName: Name(''),
    dateOfBirth: DateTime(1900),
    gender: Gender(value: ''),
    country: null,
    provinceState: null,
    city: null,
    address: '',
    zipCode: ZipCode.emptyUS,
    mobileNumber: PhoneNumber.empty(),
    profilePhoto: ProfilePhoto.empty(),
  );

  const BasicInfo({
    required this.profilePhoto,
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
    required this.gender,
    this.country,
    this.provinceState,
    this.city,
    required this.address,
    required this.zipCode,
    required this.mobileNumber,
    this.identificationType,
    this.identificationNumber,
  });

  /// Check if the user is at least 18 years old
  bool get isAdult {
    final today = DateTime.now();
    final eighteenYearsAgo = DateTime(today.year - 18, today.month, today.day);
    return dateOfBirth.isBefore(eighteenYearsAgo) ||
        dateOfBirth.isAtSameMomentAs(eighteenYearsAgo);
  }

  /// Validates all fields
  bool get isValid {
    return firstName.isValid &&
        lastName.isValid &&
        isAdult &&
        country != null &&
        provinceState != null &&
        city != null &&
        address.isNotEmpty &&
        address.length >= 5 &&
        zipCode.value.isNotEmpty &&
        mobileNumber.isValid &&
        profilePhoto.hasPhoto;
  }

  /// Validation error messages
  String? get countryError => country == null ? 'Country is required' : null;
  String? get provinceStateError =>
      provinceState == null ? 'Province/State is required' : null;
  String? get cityError => city == null ? 'City is required' : null;
  String? get addressError {
    if (address.isEmpty) return 'Address is required';
    if (address.length < 5) return 'Address must be at least 5 characters';
    return null;
  }

  String? get zipCodeError =>
      zipCode.value.isEmpty ? 'ZIP Code is required' : null;
  String? get mobileNumberError => mobileNumber.errorMessage;
  String? get dateOfBirthError =>
      !isAdult ? 'You must be at least 18 years old' : null;
  String? get profilePhotoError =>
      profilePhoto.path.isEmpty ? 'Profile photo is required' : null;

  /// Creates a copy with updated fields
  BasicInfo copyWith({
    ProfilePhoto? profilePhoto,
    Name? firstName,
    Name? lastName,
    DateTime? dateOfBirth,
    Gender? gender,
    Country? country,
    Province? provinceState,
    City? city,
    String? address,
    ZipCode? zipCode,
    PhoneNumber? mobileNumber,
    String? identificationType,
    String? identificationNumber,
  }) {
    return BasicInfo(
      profilePhoto: profilePhoto ?? this.profilePhoto,
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      dateOfBirth: dateOfBirth ?? this.dateOfBirth,
      gender: gender ?? this.gender,
      country: country ?? this.country,
      provinceState: provinceState ?? this.provinceState,
      city: city ?? this.city,
      address: address ?? this.address,
      zipCode: zipCode ?? this.zipCode,
      mobileNumber: mobileNumber ?? this.mobileNumber,
      identificationType: identificationType ?? this.identificationType,
      identificationNumber: identificationNumber ?? this.identificationNumber,
    );
  }

  @override
  List<Object?> get props => [
    profilePhoto,
    firstName,
    lastName,
    dateOfBirth,
    gender,
    country,
    provinceState,
    city,
    address,
    zipCode,
    mobileNumber,
    identificationType,
    identificationNumber,
  ];
}
