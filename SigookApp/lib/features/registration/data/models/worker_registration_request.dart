import '../../domain/entities/registration_form.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/identification_type.dart';
import '../../domain/entities/day_of_week.dart';
import '../../../../core/constants/enums.dart' as enums;

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

  /// Create from registration form entity
  factory WorkerRegistrationRequest.fromEntity(RegistrationForm form) {
    if (form.personalInfo == null ||
        form.contactInfo == null ||
        form.addressInfo == null ||
        form.availabilityInfo == null ||
        form.professionalInfo == null) {
      throw ArgumentError('All form sections must be completed');
    }

    final personalInfo = form.personalInfo!;
    final contactInfo = form.contactInfo!;
    final addressInfo = form.addressInfo!;
    final availabilityInfo = form.availabilityInfo!;
    final professionalInfo = form.professionalInfo!;

    // Format date as ISO 8601
    final formattedDate = personalInfo.dateOfBirth.toIso8601String();

    // Convert DayOfWeek enums to DayOfWeekEntity objects
    // Note: API expects {id: guid, value: "DayName"}
    // We need to generate a consistent GUID for each day
    // Using a fixed mapping based on day index
    final dayIdMap = {
      enums.DayOfWeek.monday: '00000000-0000-0000-0000-000000000001',
      enums.DayOfWeek.tuesday: '00000000-0000-0000-0000-000000000002',
      enums.DayOfWeek.wednesday: '00000000-0000-0000-0000-000000000003',
      enums.DayOfWeek.thursday: '00000000-0000-0000-0000-000000000004',
      enums.DayOfWeek.friday: '00000000-0000-0000-0000-000000000005',
      enums.DayOfWeek.saturday: '00000000-0000-0000-0000-000000000006',
      enums.DayOfWeek.sunday: '00000000-0000-0000-0000-000000000007',
    };
    
    final dayNameMap = {
      enums.DayOfWeek.monday: 'Monday',
      enums.DayOfWeek.tuesday: 'Tuesday',
      enums.DayOfWeek.wednesday: 'Wednesday',
      enums.DayOfWeek.thursday: 'Thursday',
      enums.DayOfWeek.friday: 'Friday',
      enums.DayOfWeek.saturday: 'Saturday',
      enums.DayOfWeek.sunday: 'Sunday',
    };
    
    final availabilityDays = availabilityInfo.availableDays.map((day) {
      return DayOfWeekEntity(
        id: dayIdMap[day]!,
        value: dayNameMap[day]!,
      );
    }).toList();

    return WorkerRegistrationRequest(
      firstName: personalInfo.firstName.value,
      lastName: personalInfo.lastName.value,
      birthDay: formattedDate,
      gender: personalInfo.gender,
      identificationNumber1: contactInfo.identification,
      identificationType1: contactInfo.identificationType,
      mobileNumber: null, // Not collected yet
      phone: null, // Not collected yet
      address: addressInfo.address,
      city: addressInfo.city,
      provinceState: addressInfo.provinceState,
      country: addressInfo.country,
      availabilities: [availabilityInfo.availabilityType], // Wrap in list
      availabilityTimes: availabilityInfo.availableTimes,
      availabilityDays: availabilityDays,
      languages: professionalInfo.languages,
      skills: professionalInfo.skills,
      email: contactInfo.email.value,
      password: contactInfo.password.value,
      confirmPassword: contactInfo.password.value, // Same as password
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
