import 'package:equatable/equatable.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/zip_code.dart';
import 'value_objects/name.dart';
import 'value_objects/phone_number.dart';
import 'gender.dart';
import 'country.dart';
import 'province.dart';
import 'city.dart';
import 'value_objects/profile_photo.dart';

class BasicInfo extends Equatable {
  final ProfilePhoto profilePhoto;
  final Name firstName;
  final Name lastName;
  final DateTime dateOfBirth;
  final Gender gender;

  final Country? country;
  final Province? provinceState;
  final City? city;
  final String address;
  final ZipCode zipCode;

  final PhoneNumber mobileNumber;

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

  bool get isAdult {
    final today = DateTime.now();
    final eighteenYearsAgo = DateTime(today.year - 18, today.month, today.day);
    return dateOfBirth.isBefore(eighteenYearsAgo) ||
        dateOfBirth.isAtSameMomentAs(eighteenYearsAgo);
  }

  bool get hasGender => gender.id?.isNotEmpty ?? false;

  bool get isValid {
    return firstName.isValid &&
        lastName.isValid &&
        isAdult &&
        hasGender &&
        country != null &&
        provinceState != null &&
        city != null &&
        addressError == null &&
        zipCode.value.isNotEmpty &&
        mobileNumber.isValid;
  }

  String? get genderError => hasGender ? null : 'Gender is required';
  String? get countryError => country == null ? 'Country is required' : null;
  String? get provinceStateError =>
      provinceState == null ? 'Province/State is required' : null;
  String? get cityError => city == null ? 'City is required' : null;
  static const int addressMinLength = 5;
  static const int addressMaxLength = 100;
  static final RegExp _addressAllowed = RegExp(r'^[-.#, a-zA-Z0-9]+$');

  String? get addressError => validateAddress(address);

  static String? validateAddress(String address) {
    if (address.isEmpty) return 'Address is required';
    if (address.length < addressMinLength) {
      return 'Address must be at least $addressMinLength characters';
    }
    if (address.length > addressMaxLength) {
      return 'Address must be at most $addressMaxLength characters';
    }
    if (!_addressAllowed.hasMatch(address)) {
      return 'Address can only contain letters, numbers, spaces and - . # ,';
    }
    return null;
  }

  String? get zipCodeError =>
      zipCode.value.isEmpty ? 'Postal/ZIP code is required' : null;
  String? get mobileNumberError => mobileNumber.errorMessage;
  String? get dateOfBirthError =>
      !isAdult ? 'You must be at least 18 years old' : null;
  String? get profilePhotoError =>
      profilePhoto.path.isEmpty ? 'Profile photo is required' : null;

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

  Map<String, dynamic> toJson() {
    return {
      'profilePhoto': profilePhoto.toJson(),
      'firstName': firstName.toString(),
      'lastName': lastName.toString(),
      'dateOfBirth': dateOfBirth.toIso8601String(),
      'gender': gender.toJson(),
      if (country != null) 'country': country!.toJson(),
      if (provinceState != null) 'provinceState': provinceState!.toJson(),
      if (city != null) 'city': city!.toJson(),
      'address': address,
      'zipCode': zipCode.value,
      'mobileNumber': mobileNumber.nationalFormat,
      if (identificationType != null) 'identificationType': identificationType,
      if (identificationNumber != null)
        'identificationNumber': identificationNumber,
    };
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
