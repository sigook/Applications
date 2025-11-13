import '../../domain/entities/registration_form.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/identification_type.dart';
import '../../domain/entities/day_of_week.dart';
import '../../domain/entities/location.dart';
import '../../domain/entities/city.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/lifting_capacity.dart';

/// Request model for worker profile registration
/// Maps to https://staging.api.sigook.ca/api/WorkerProfile endpoint
class WorkerRegistrationRequest {
  // Personal Info
  final String firstName;
  final String lastName;
  final String birthDay; // ISO 8601 format
  final Gender gender;

  // Identification
  final String identificationNumber1;
  final IdentificationType identificationType1;

  // Contact
  final String? mobileNumber;
  final String? phone;

  // Location
  final Location location;

  // Lifting capacity
  final LiftingCapacity? lift;

  // Availability
  final List<AvailabilityType> availabilities;
  final List<AvailableTime> availabilityTimes;
  final List<DayOfWeekEntity> availabilityDays;

  // Professional
  final List<Language> languages;
  final List<Skill> skills;

  // Auth
  final String email;
  final String password;
  final String confirmPassword;
  final bool agreeTermsAndConditions;

  WorkerRegistrationRequest({
    required this.firstName,
    required this.lastName,
    required this.birthDay,
    required this.gender,
    required this.identificationNumber1,
    required this.identificationType1,
    this.mobileNumber,
    this.phone,
    required this.location,
    this.lift,
    required this.availabilities,
    required this.availabilityTimes,
    required this.availabilityDays,
    required this.languages,
    required this.skills,
    required this.email,
    required this.password,
    required this.confirmPassword,
    required this.agreeTermsAndConditions,
  });

  /// Create from registration form entity (NEW 4-section structure)
  factory WorkerRegistrationRequest.fromEntity(RegistrationForm form) {
    if (form.basicInfo == null ||
        form.preferencesInfo == null ||
        form.accountInfo == null) {
      throw ArgumentError('Required form sections must be completed');
    }

    final basicInfo = form.basicInfo!;
    final preferencesInfo = form.preferencesInfo!;
    final accountInfo = form.accountInfo!;

    // Format date as ISO 8601
    final formattedDate = basicInfo.dateOfBirth.toIso8601String();

    // Days, times, and availability type already have IDs from catalog
    final availabilityDays = preferencesInfo.availableDays;
    final availabilityTimes = preferencesInfo.availableTimes;
    final availabilityType = preferencesInfo.availabilityType;

    // Use identification from BasicInfo or documents
    // If not provided in BasicInfo, use placeholder (will be updated via documents upload)
    final identificationType = basicInfo.identificationType != null
        ? IdentificationType(
            id: null, // ID will be resolved by backend from value
            value: basicInfo.identificationType!,
          )
        : IdentificationType(
            id: null,
            value: 'ID', // Placeholder if not provided
          );

    final identificationNumber = basicInfo.identificationNumber ?? 'PENDING';

    // Build Location object using catalog entities with IDs
    final location = basicInfo.city != null
        ? Location(
            city: basicInfo.city!,
            address: basicInfo.address,
            postalCode: basicInfo.zipCode.value,
          )
        : Location(
            // Fallback with empty GUIDs if location entities not selected
            city: City(
              id: '00000000-0000-0000-0000-000000000000',
              value: 'Not specified',
              code: null,
              province: Province(
                id: '00000000-0000-0000-0000-000000000000',
                value: 'Not specified',
                code: null,
                country: Country(
                  id: '00000000-0000-0000-0000-000000000000',
                  value: 'Not specified',
                  code: null,
                ),
              ),
            ),
            address: basicInfo.address,
            postalCode: basicInfo.zipCode.value,
          );

    // Lifting capacity already has ID from catalog
    final lift = preferencesInfo.liftingCapacity;

    return WorkerRegistrationRequest(
      firstName: basicInfo.firstName.value,
      lastName: basicInfo.lastName.value,
      birthDay: formattedDate,
      gender: basicInfo.gender,
      identificationNumber1: identificationNumber,
      identificationType1: identificationType,
      mobileNumber: basicInfo.mobileNumber.e164Format,
      phone: null, // Optional phone field
      location: location,
      lift: lift,
      availabilities: [availabilityType],
      availabilityTimes: availabilityTimes,
      availabilityDays: availabilityDays,
      languages: preferencesInfo.languages,
      skills: preferencesInfo.skills,
      email: accountInfo.email.value,
      password: accountInfo.password.value,
      confirmPassword: accountInfo.confirmPassword,
      agreeTermsAndConditions: accountInfo.termsAccepted,
    );
  }

  /// Convert to JSON for API request
  Map<String, dynamic> toJson() {
    // Debug: Print entity values before conversion
    print('═══ DEBUG: Entity Values ═══');
    print('Gender: id=${gender.id}, value=${gender.value}');
    print('Languages count: ${languages.length}');
    if (languages.isNotEmpty) {
      print(
        'First language: id=${languages.first.id}, value=${languages.first.value}',
      );
    }
    print('Skills count: ${skills.length}');
    if (skills.isNotEmpty) {
      print('First skill: skill=${skills.first.skill}');
    }
    print('═══════════════════════════');

    return {
      'firstName': firstName,
      'lastName': lastName,
      'birthDay': birthDay,
      'gender': {'id': gender.id}, // Only id per API spec
      'identificationNumber1': identificationNumber1,
      'identificationType1': identificationType1.toJson(),
      if (mobileNumber != null) 'mobileNumber': mobileNumber,
      if (phone != null) 'phone': phone,
      'location': location.toJson(),
      if (lift != null) 'lift': lift!.toJson(),
      'availabilities': availabilities.map((a) => a.toJson()).toList(),
      'availabilityTimes': availabilityTimes.map((a) => a.toJson()).toList(),
      'availabilityDays': availabilityDays.map((d) => d.toJson()).toList(),
      'languages': languages.map((l) => l.toJson()).toList(),
      'skills': skills.map((s) => s.toJson()).toList(),
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
      'agreeTermsAndConditions': agreeTermsAndConditions,
      // TODO: Add file uploads when implemented
      // 'identificationType1File': null,
      // 'identificationType2File': null,
      // 'identificationType2': null,
      // 'licenses': [],
      // 'certificates': [],
      // 'resume': null,
      // 'otherDocuments': [],
    };
  }
}
