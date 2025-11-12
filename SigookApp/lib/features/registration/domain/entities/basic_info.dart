import 'package:equatable/equatable.dart';
import 'value_objects/name.dart';
import 'gender.dart';

/// Basic information entity
/// Combines personal details, location, and mobile number
class BasicInfo extends Equatable {
  // Personal details
  final Name firstName;
  final Name lastName;
  final DateTime dateOfBirth;
  final Gender gender;
  
  // Location
  final String country;
  final String provinceState;
  final String city;
  final String address;
  final String zipCode;
  
  // Contact
  final String mobileNumber;
  
  // Identification (optional - can be provided via documents)
  final String? identificationType;
  final String? identificationNumber;

  const BasicInfo({
    required this.firstName,
    required this.lastName,
    required this.dateOfBirth,
    required this.gender,
    required this.country,
    required this.provinceState,
    required this.city,
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
        country.isNotEmpty &&
        provinceState.isNotEmpty &&
        city.isNotEmpty &&
        address.isNotEmpty &&
        address.length >= 5 &&
        zipCode.isNotEmpty &&
        mobileNumber.isNotEmpty &&
        mobileNumber.length >= 10;
  }

  /// Validation error messages
  String? get countryError => country.isEmpty ? 'Country is required' : null;
  String? get provinceStateError =>
      provinceState.isEmpty ? 'Province/State is required' : null;
  String? get cityError => city.isEmpty ? 'City is required' : null;
  String? get addressError {
    if (address.isEmpty) return 'Address is required';
    if (address.length < 5) return 'Address must be at least 5 characters';
    return null;
  }
  String? get zipCodeError => zipCode.isEmpty ? 'ZIP Code is required' : null;
  String? get mobileNumberError {
    if (mobileNumber.isEmpty) return 'Mobile number is required';
    if (mobileNumber.length < 10) return 'Mobile number must be at least 10 digits';
    return null;
  }
  String? get dateOfBirthError =>
      !isAdult ? 'You must be at least 18 years old' : null;

  /// Creates a copy with updated fields
  BasicInfo copyWith({
    Name? firstName,
    Name? lastName,
    DateTime? dateOfBirth,
    Gender? gender,
    String? country,
    String? provinceState,
    String? city,
    String? address,
    String? zipCode,
    String? mobileNumber,
    String? identificationType,
    String? identificationNumber,
  }) {
    return BasicInfo(
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
