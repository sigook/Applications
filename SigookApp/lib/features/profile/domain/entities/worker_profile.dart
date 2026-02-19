import 'package:equatable/equatable.dart';

class WorkerProfile extends Equatable {
  final String id;
  final int? numberId;
  final String? profilePhotoUrl;
  final String? firstName;
  final String? middleName;
  final String? lastName;
  final String? secondLastName;
  final String? birthDay;
  final String? gender;
  final String? socialInsurance;
  final bool socialInsuranceExpire;
  final String? dueDate;
  final String? socialInsuranceFileName;
  final String? identificationNumber1;
  final String? identificationNumber2;
  final String? identificationType1;
  final String? identificationType2;
  final String? identificationType1FileName;
  final String? identificationType2FileName;
  final bool havePoliceCheckBackground;
  final String? policeCheckBackgroundFileName;
  final String? mobileNumber;
  final String? phone;
  final String? email;
  final String? address;
  final String? city;
  final String? province;
  final String? country;
  final String? postalCode;
  final bool hasVehicle;
  final List<String> availabilities;
  final List<String> availabilityTimes;
  final List<String> availabilityDays;
  final String? liftCapacity;
  final List<String> languages;
  final List<String> skills;
  final bool hasResume;
  final bool approvedToWork;
  final String? punchCardId;
  final String? contactEmergencyName;
  final String? contactEmergencyLastName;
  final String? contactEmergencyPhone;

  const WorkerProfile({
    required this.id,
    this.numberId,
    this.profilePhotoUrl,
    this.firstName,
    this.middleName,
    this.lastName,
    this.secondLastName,
    this.birthDay,
    this.gender,
    this.socialInsurance,
    this.socialInsuranceExpire = false,
    this.dueDate,
    this.socialInsuranceFileName,
    this.identificationNumber1,
    this.identificationNumber2,
    this.identificationType1,
    this.identificationType2,
    this.identificationType1FileName,
    this.identificationType2FileName,
    this.havePoliceCheckBackground = false,
    this.policeCheckBackgroundFileName,
    this.mobileNumber,
    this.phone,
    this.email,
    this.address,
    this.city,
    this.province,
    this.country,
    this.postalCode,
    this.hasVehicle = false,
    this.availabilities = const [],
    this.availabilityTimes = const [],
    this.availabilityDays = const [],
    this.liftCapacity,
    this.languages = const [],
    this.skills = const [],
    this.hasResume = false,
    this.approvedToWork = false,
    this.punchCardId,
    this.contactEmergencyName,
    this.contactEmergencyLastName,
    this.contactEmergencyPhone,
  });

  String get fullName {
    final parts = [firstName, middleName, lastName, secondLastName]
        .where((p) => p != null && p.isNotEmpty)
        .toList();
    return parts.join(' ').trim();
  }

  String get maskedSocialInsurance {
    if (socialInsurance == null || socialInsurance!.isEmpty) return 'N/A';
    if (socialInsurance!.length <= 4) return '****';
    return '****${socialInsurance!.substring(socialInsurance!.length - 4)}';
  }

  String get maskedIdNumber1 {
    if (identificationNumber1 == null || identificationNumber1!.isEmpty) return 'N/A';
    if (identificationNumber1!.length <= 4) return '****';
    return '****${identificationNumber1!.substring(identificationNumber1!.length - 4)}';
  }

  String get maskedIdNumber2 {
    if (identificationNumber2 == null || identificationNumber2!.isEmpty) return 'N/A';
    if (identificationNumber2!.length <= 4) return '****';
    return '****${identificationNumber2!.substring(identificationNumber2!.length - 4)}';
  }

  String get formattedBirthDay {
    if (birthDay == null || birthDay!.isEmpty) return 'N/A';
    try {
      final date = DateTime.parse(birthDay!);
      const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December',
      ];
      return '${months[date.month - 1]} ${date.day}, ${date.year}';
    } catch (_) {
      return birthDay!;
    }
  }

  String get formattedDueDate {
    if (dueDate == null || dueDate!.isEmpty) return 'N/A';
    try {
      final date = DateTime.parse(dueDate!);
      const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December',
      ];
      return '${months[date.month - 1]} ${date.day}, ${date.year}';
    } catch (_) {
      return dueDate!;
    }
  }

  @override
  List<Object?> get props => [
    id, numberId, profilePhotoUrl, firstName, middleName, lastName,
    secondLastName, birthDay, gender, socialInsurance,
    socialInsuranceExpire, dueDate, socialInsuranceFileName,
    identificationNumber1, identificationNumber2,
    identificationType1, identificationType2,
    identificationType1FileName, identificationType2FileName,
    havePoliceCheckBackground, policeCheckBackgroundFileName,
    mobileNumber, phone, email, address, city, province, country,
    postalCode, hasVehicle, availabilities, availabilityTimes,
    availabilityDays, liftCapacity, languages, skills, hasResume,
    approvedToWork, punchCardId, contactEmergencyName,
    contactEmergencyLastName, contactEmergencyPhone,
  ];
}
