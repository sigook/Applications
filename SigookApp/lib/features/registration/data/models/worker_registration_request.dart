import 'package:flutter/foundation.dart';
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
import '../../domain/entities/uploaded_file.dart';
import '../../domain/entities/profile_image.dart';

/// Request model for worker profile registration
/// Maps to https://staging.api.sigook.ca/api/WorkerProfile endpoint
class WorkerRegistrationRequest {
  // Personal Info
  final String firstName;
  final String lastName;
  final String birthDay; // ISO 8601 format
  final Gender gender;
  final ProfileImage? profileImage;

  // Identification 1 (required)
  final String identificationNumber1;
  final IdentificationType identificationType1;
  final UploadedFile? identificationType1File;

  // Identification 2 (optional)
  final String? identificationNumber2;
  final IdentificationType? identificationType2;
  final UploadedFile? identificationType2File;

  // Documents
  final UploadedFile? resume;

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
    this.profileImage,
    required this.identificationNumber1,
    required this.identificationType1,
    this.identificationType1File,
    this.identificationNumber2,
    this.identificationType2,
    this.identificationType2File,
    this.resume,
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
    final documentsInfo = form.documentsInfo;

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

    // Extract documents from form
    final identification1 = documentsInfo?.identification1;
    final identification2 = documentsInfo?.identification2;
    final resumeFile = documentsInfo?.resume;

    // Create IdentificationType from document data if available
    final identType1 = identification1 != null
        ? IdentificationType(
            id: identification1.identificationTypeId,
            value: identification1.identificationTypeValue,
          )
        : identificationType;

    final identNumber1 =
        identification1?.identificationNumber ?? identificationNumber;

    // Convert profile photo to profile image
    final profileImage = basicInfo.profilePhoto.hasPhoto
        ? ProfileImage.fromPath(basicInfo.profilePhoto.path)
        : null;

    return WorkerRegistrationRequest(
      firstName: basicInfo.firstName.value,
      lastName: basicInfo.lastName.value,
      birthDay: formattedDate,
      gender: basicInfo.gender,
      profileImage: profileImage,
      identificationNumber1: identNumber1,
      identificationType1: identType1,
      identificationType1File: identification1?.file,
      identificationNumber2: identification2?.identificationNumber,
      identificationType2: identification2 != null
          ? IdentificationType(
              id: identification2.identificationTypeId,
              value: identification2.identificationTypeValue,
            )
          : null,
      identificationType2File: identification2?.file,
      resume: resumeFile,
      mobileNumber: basicInfo.mobileNumber.nationalFormat,
      phone: null,
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
    debugPrint('═══ DEBUG: Entity Values ═══');
    debugPrint('Gender: id=${gender.id}, value=${gender.value}');
    debugPrint('Languages count: ${languages.length}');
    if (languages.isNotEmpty) {
      debugPrint(
        'First language: id=${languages.first.id}, value=${languages.first.value}',
      );
    }
    debugPrint('Skills count: ${skills.length}');
    if (skills.isNotEmpty) {
      debugPrint('First skill: skill=${skills.first.skill}');
    }
    debugPrint('═══════════════════════════');

    return {
      'firstName': firstName,
      'lastName': lastName,
      'birthDay': birthDay,
      'gender': {'id': gender.id}, // Only id per API spec
      if (profileImage != null) 'profileImage': profileImage!.toJson(),
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
      // File uploads
      'identificationType1File': identificationType1File?.toJson(),
      'identificationType2File': identificationType2File?.toJson(),
      'identificationType2': identificationType2?.toJson(),
      if (identificationNumber2 != null)
        'identificationNumber2': identificationNumber2,
      'resume': resume?.toJson(),
      // Empty arrays for now
      'licenses': [],
      'certificates': [],
      'otherDocuments': [],
    };
  }
}
