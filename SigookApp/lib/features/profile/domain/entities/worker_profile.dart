import 'package:equatable/equatable.dart';

class WorkerLicense extends Equatable {
  final String? fileUrl;
  final String? fileName;
  final String? description;
  final String? number;
  final String? issued;
  final String? expires;

  const WorkerLicense({
    this.fileUrl,
    this.fileName,
    this.description,
    this.number,
    this.issued,
    this.expires,
  });

  String get formattedIssued => _formatDate(issued);
  String get formattedExpires => _formatDate(expires);

  bool get isExpired {
    if (expires == null || expires!.isEmpty) return false;
    try {
      return DateTime.parse(expires!).isBefore(DateTime.now());
    } catch (_) {
      return false;
    }
  }

  static String _formatDate(String? dateStr) {
    if (dateStr == null || dateStr.isEmpty) return 'N/A';
    try {
      final date = DateTime.parse(dateStr);
      const months = [
        'January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December',
      ];
      return '${months[date.month - 1]} ${date.day}, ${date.year}';
    } catch (_) {
      return dateStr;
    }
  }

  @override
  List<Object?> get props => [fileUrl, fileName, description, number, issued, expires];
}

class WorkerCertificate extends Equatable {
  final String? id;
  final String? fileUrl;
  final String? fileName;
  final String? description;

  const WorkerCertificate({
    this.id,
    this.fileUrl,
    this.fileName,
    this.description,
  });

  @override
  List<Object?> get props => [id, fileUrl, fileName, description];
}

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
  final String? socialInsuranceFileUrl;
  final String? identificationNumber1;
  final String? identificationNumber2;
  final String? identificationType1;
  final String? identificationType2;
  final String? identificationType1FileName;
  final String? identificationType1FileUrl;
  final String? identificationType2FileName;
  final String? identificationType2FileUrl;
  final bool havePoliceCheckBackground;
  final String? policeCheckBackgroundFileName;
  final String? mobileNumber;
  final String? phone;
  final String? email;
  final String? address;
  final String? city;
  final String? cityId;
  final String? province;
  final String? country;
  final String? postalCode;
  final bool hasVehicle;
  final List<String> availabilities;
  final List<String> availabilityIds;
  final List<String> availabilityTimes;
  final List<String> availabilityTimeIds;
  final List<String> availabilityDays;
  final List<String> availabilityDayIds;
  final String? liftCapacity;
  final String? liftId;
  final List<String> languages;
  final List<String> languageIds;
  final List<String> skills;
  final List<String> skillIds;
  final bool hasResume;
  final String? resumeFileName;
  final String? resumeFileUrl;
  final List<WorkerLicense> licenses;
  final List<WorkerCertificate> certificates;
  final bool approvedToWork;
  final String? punchCardId;
  final bool haveAnyHealthProblem;
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
    this.socialInsuranceFileUrl,
    this.identificationNumber1,
    this.identificationNumber2,
    this.identificationType1,
    this.identificationType2,
    this.identificationType1FileName,
    this.identificationType1FileUrl,
    this.identificationType2FileName,
    this.identificationType2FileUrl,
    this.havePoliceCheckBackground = false,
    this.policeCheckBackgroundFileName,
    this.mobileNumber,
    this.phone,
    this.email,
    this.address,
    this.city,
    this.cityId,
    this.province,
    this.country,
    this.postalCode,
    this.hasVehicle = false,
    this.availabilities = const [],
    this.availabilityIds = const [],
    this.availabilityTimes = const [],
    this.availabilityTimeIds = const [],
    this.availabilityDays = const [],
    this.availabilityDayIds = const [],
    this.liftCapacity,
    this.liftId,
    this.languages = const [],
    this.languageIds = const [],
    this.skills = const [],
    this.skillIds = const [],
    this.hasResume = false,
    this.resumeFileName,
    this.resumeFileUrl,
    this.licenses = const [],
    this.certificates = const [],
    this.approvedToWork = false,
    this.punchCardId,
    this.haveAnyHealthProblem = false,
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
    socialInsuranceExpire, dueDate, socialInsuranceFileName, socialInsuranceFileUrl,
    identificationNumber1, identificationNumber2,
    identificationType1, identificationType2,
    identificationType1FileName, identificationType1FileUrl,
    identificationType2FileName, identificationType2FileUrl,
    havePoliceCheckBackground, policeCheckBackgroundFileName,
    mobileNumber, phone, email, address, city, cityId, province, country,
    postalCode, hasVehicle,
    availabilities, availabilityIds,
    availabilityTimes, availabilityTimeIds,
    availabilityDays, availabilityDayIds,
    liftCapacity, liftId,
    languages, languageIds,
    skills, skillIds,
    hasResume, resumeFileName, resumeFileUrl,
    licenses, certificates,
    approvedToWork, punchCardId, haveAnyHealthProblem,
    contactEmergencyName, contactEmergencyLastName, contactEmergencyPhone,
  ];
}
