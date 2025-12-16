import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/identification_type.dart';
import '../../domain/entities/day_of_week.dart';
import '../../domain/entities/location.dart';
import '../../domain/entities/lifting_capacity.dart';

class WorkerProfileData {
  final String firstName;
  final String lastName;
  final String birthDay;
  final Gender gender;
  final String identificationNumber1;
  final IdentificationType identificationType1;
  final String? identificationNumber2;
  final IdentificationType? identificationType2;
  final String? mobileNumber;
  final String? phone;
  final Location location;
  final LiftingCapacity? lift;
  final List<AvailabilityType> availabilities;
  final List<AvailableTime> availabilityTimes;
  final List<DayOfWeekEntity> availabilityDays;
  final List<Language> languages;
  final List<Skill> skills;
  final String email;
  final String password;
  final String confirmPassword;
  final bool agreeTermsAndConditions;

  WorkerProfileData({
    required this.firstName,
    required this.lastName,
    required this.birthDay,
    required this.gender,
    required this.identificationNumber1,
    required this.identificationType1,
    this.identificationNumber2,
    this.identificationType2,
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

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'birthDay': birthDay,
      'gender': {'id': gender.id},
      'identificationNumber1': identificationNumber1,
      'identificationType1': identificationType1.toJson(),
      if (identificationNumber2 != null)
        'identificationNumber2': identificationNumber2,
      if (identificationType2 != null)
        'identificationType2': identificationType2!.toJson(),
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
      'licenses': [],
      'certificates': [],
      'otherDocuments': [],
    };
  }
}
