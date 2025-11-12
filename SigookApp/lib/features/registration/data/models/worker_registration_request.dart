import '../../domain/entities/registration_form.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/identification_type.dart';
import '../../domain/entities/day_of_week.dart';

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
  
  // Location (simplified for now - can be expanded later)
  final String? address;
  final String? city;
  final String? provinceState;
  final String? country;
  
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

  WorkerRegistrationRequest({
    required this.firstName,
    required this.lastName,
    required this.birthDay,
    required this.gender,
    required this.identificationNumber1,
    required this.identificationType1,
    this.mobileNumber,
    this.phone,
    this.address,
    this.city,
    this.provinceState,
    this.country,
    required this.availabilities,
    required this.availabilityTimes,
    required this.availabilityDays,
    required this.languages,
    required this.skills,
    required this.email,
    required this.password,
    required this.confirmPassword,
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

    // Convert day strings to DayOfWeekEntity objects
    // API expects {id: guid, value: "DayName"}
    final dayIdMap = {
      'Monday': '00000000-0000-0000-0000-000000000001',
      'Tuesday': '00000000-0000-0000-0000-000000000002',
      'Wednesday': '00000000-0000-0000-0000-000000000003',
      'Thursday': '00000000-0000-0000-0000-000000000004',
      'Friday': '00000000-0000-0000-0000-000000000005',
      'Saturday': '00000000-0000-0000-0000-000000000006',
      'Sunday': '00000000-0000-0000-0000-000000000007',
    };
    
    final availabilityDays = preferencesInfo.availableDays.map((dayName) {
      return DayOfWeekEntity(
        id: dayIdMap[dayName] ?? '00000000-0000-0000-0000-000000000000',
        value: dayName,
      );
    }).toList();

    // Convert availability time strings to AvailableTime entities
    // Assuming time slots have associated IDs from catalog
    final availabilityTimes = preferencesInfo.availableTimes.map((timeValue) {
      return AvailableTime(
        id: null, // Will be filled by API or catalog lookup
        value: timeValue,
      );
    }).toList();

    // Convert availability type string to AvailabilityType entity
    final availabilityType = AvailabilityType(
      id: null, // Will be filled by API or catalog lookup
      value: preferencesInfo.availabilityType,
    );

    // NOTE: Currently no identification fields in new structure
    // Using placeholder values - may need to add these fields to BasicInfo or AccountInfo
    final placeholderIdentificationType = IdentificationType(
      id: null,
      value: 'ID', // Placeholder
    );

    return WorkerRegistrationRequest(
      firstName: basicInfo.firstName.value,
      lastName: basicInfo.lastName.value,
      birthDay: formattedDate,
      gender: basicInfo.gender,
      identificationNumber1: 'TEMP', // TODO: Add identification to form
      identificationType1: placeholderIdentificationType,
      mobileNumber: basicInfo.mobileNumber,
      phone: null, // Optional phone field
      address: basicInfo.address,
      city: basicInfo.city,
      provinceState: basicInfo.provinceState,
      country: basicInfo.country,
      availabilities: [availabilityType],
      availabilityTimes: availabilityTimes,
      availabilityDays: availabilityDays,
      languages: preferencesInfo.languages,
      skills: preferencesInfo.skills,
      email: accountInfo.email.value,
      password: accountInfo.password.value,
      confirmPassword: accountInfo.confirmPassword,
    );
  }

  /// Convert to JSON for API request
  Map<String, dynamic> toJson() {
    // Debug: Print entity values before conversion
    print('═══ DEBUG: Entity Values ═══');
    print('Gender: id=${gender.id}, value=${gender.value}');
    print('Languages count: ${languages.length}');
    if (languages.isNotEmpty) {
      print('First language: id=${languages.first.id}, value=${languages.first.value}');
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
      'gender': gender.toJson(),
      'identificationNumber1': identificationNumber1,
      'identificationType1': identificationType1.toJson(),
      if (mobileNumber != null) 'mobileNumber': mobileNumber,
      if (phone != null) 'phone': phone,
      if (address != null) 'address': address,
      if (city != null) 'city': city,
      if (provinceState != null) 'provinceState': provinceState,
      if (country != null) 'country': country,
      'availabilities': availabilities.map((a) => a.toJson()).toList(),
      'availabilityTimes': availabilityTimes.map((a) => a.toJson()).toList(),
      'availabilityDays': availabilityDays.map((d) => d.toJson()).toList(),
      'languages': languages.map((l) => l.toJson()).toList(),
      'skills': skills.map((s) => s.toJson()).toList(),
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
    };
  }
}
