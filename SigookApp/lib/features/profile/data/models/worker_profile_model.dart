import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/worker_profile.dart';

part 'worker_profile_model.freezed.dart';
part 'worker_profile_model.g.dart';

// --- Shared nested models ---

@freezed
abstract class ProfileImageModel with _$ProfileImageModel {
  const factory ProfileImageModel({
    String? id,
    String? pathFile,
    String? fileName,
    String? description,
    @Default(false) bool canDownload,
  }) = _ProfileImageModel;

  factory ProfileImageModel.fromJson(Map<String, dynamic> json) =>
      _$ProfileImageModelFromJson(json);
}

@freezed
abstract class CatalogItemModel with _$CatalogItemModel {
  const factory CatalogItemModel({
    String? id,
    String? value,
  }) = _CatalogItemModel;

  factory CatalogItemModel.fromJson(Map<String, dynamic> json) =>
      _$CatalogItemModelFromJson(json);
}

@freezed
abstract class SkillItemModel with _$SkillItemModel {
  const factory SkillItemModel({
    String? id,
    String? skill,
  }) = _SkillItemModel;

  factory SkillItemModel.fromJson(Map<String, dynamic> json) =>
      _$SkillItemModelFromJson(json);
}

@freezed
abstract class CountryModel with _$CountryModel {
  const factory CountryModel({
    String? id,
    String? value,
    String? code,
  }) = _CountryModel;

  factory CountryModel.fromJson(Map<String, dynamic> json) =>
      _$CountryModelFromJson(json);
}

@freezed
abstract class ProvinceModel with _$ProvinceModel {
  const factory ProvinceModel({
    String? id,
    String? value,
    String? code,
    CountryModel? country,
  }) = _ProvinceModel;

  factory ProvinceModel.fromJson(Map<String, dynamic> json) =>
      _$ProvinceModelFromJson(json);
}

@freezed
abstract class CityModel with _$CityModel {
  const factory CityModel({
    String? id,
    String? value,
    String? code,
    ProvinceModel? province,
  }) = _CityModel;

  factory CityModel.fromJson(Map<String, dynamic> json) =>
      _$CityModelFromJson(json);
}

@freezed
abstract class LocationModel with _$LocationModel {
  const factory LocationModel({
    String? id,
    String? address,
    CityModel? city,
    String? postalCode,
    String? entrance,
    String? mainIntersection,
    @Default(false) bool isBilling,
    double? latitude,
    double? longitude,
    String? formattedAddress,
    @Default(false) bool isUSA,
  }) = _LocationModel;

  factory LocationModel.fromJson(Map<String, dynamic> json) =>
      _$LocationModelFromJson(json);
}

// --- Worker profile list item (from GET /WorkerProfile) ---

@freezed
abstract class WorkerProfileListItemModel with _$WorkerProfileListItemModel {
  const factory WorkerProfileListItemModel({
    required String id,
    String? agencyFullName,
    String? agencyLogo,
  }) = _WorkerProfileListItemModel;

  factory WorkerProfileListItemModel.fromJson(Map<String, dynamic> json) =>
      _$WorkerProfileListItemModelFromJson(json);
}

// --- Full worker profile (from GET /WorkerProfile/{id}) ---

@freezed
abstract class WorkerProfileModel with _$WorkerProfileModel {
  const WorkerProfileModel._();

  const factory WorkerProfileModel({
    required String id,
    int? numberId,
    ProfileImageModel? profileImage,
    String? firstName,
    String? middleName,
    String? lastName,
    String? secondLastName,
    String? birthDay,
    CatalogItemModel? gender,
    String? socialInsurance,
    @Default(false) bool socialInsuranceExpire,
    String? dueDate,
    ProfileImageModel? socialInsuranceFile,
    String? identificationNumber1,
    String? identificationNumber2,
    @Default(false) bool havePoliceCheckBackground,
    ProfileImageModel? identificationType1File,
    ProfileImageModel? identificationType2File,
    CatalogItemModel? identificationType1,
    CatalogItemModel? identificationType2,
    ProfileImageModel? policeCheckBackGround,
    String? mobileNumber,
    String? phone,
    String? phoneExt,
    LocationModel? location,
    @Default(false) bool hasVehicle,
    @Default([]) List<CatalogItemModel> licenses,
    @Default([]) List<CatalogItemModel> certificates,
    @Default([]) List<CatalogItemModel> otherDocuments,
    @Default([]) List<CatalogItemModel> availabilities,
    @Default([]) List<CatalogItemModel> availabilityTimes,
    @Default([]) List<CatalogItemModel> availabilityDays,
    @Default([]) List<CatalogItemModel> locationPreferences,
    CatalogItemModel? lift,
    @Default([]) List<CatalogItemModel> languages,
    @Default([]) List<SkillItemModel> skills,
    ProfileImageModel? resume,
    @Default(false) bool haveAnyHealthProblem,
    String? healthProblem,
    String? otherHealthProblem,
    String? contactEmergencyName,
    String? contactEmergencyLastName,
    String? contactEmergencyPhone,
    String? email,
    @Default(false) bool approvedToWork,
    String? workerId,
    @Default(false) bool isSubcontractor,
    @Default(false) bool isContractor,
    @Default(false) bool dnu,
    String? punchCardId,
  }) = _WorkerProfileModel;

  factory WorkerProfileModel.fromJson(Map<String, dynamic> json) =>
      _$WorkerProfileModelFromJson(json);

  WorkerProfile toEntity() {
    return WorkerProfile(
      id: id,
      numberId: numberId,
      profilePhotoUrl: profileImage?.pathFile,
      firstName: firstName,
      middleName: middleName,
      lastName: lastName,
      secondLastName: secondLastName,
      birthDay: birthDay,
      gender: gender?.value,
      socialInsurance: socialInsurance,
      socialInsuranceExpire: socialInsuranceExpire,
      dueDate: dueDate,
      socialInsuranceFileName: socialInsuranceFile?.fileName,
      socialInsuranceFileUrl: socialInsuranceFile?.pathFile,
      identificationNumber1: identificationNumber1,
      identificationNumber2: identificationNumber2,
      identificationType1: identificationType1?.value,
      identificationType2: identificationType2?.value,
      identificationType1FileName: identificationType1File?.fileName,
      identificationType1FileUrl: identificationType1File?.pathFile,
      identificationType2FileName: identificationType2File?.fileName,
      identificationType2FileUrl: identificationType2File?.pathFile,
      havePoliceCheckBackground: havePoliceCheckBackground,
      policeCheckBackgroundFileName: policeCheckBackGround?.fileName,
      mobileNumber: mobileNumber,
      phone: phone,
      email: email,
      address: location?.address,
      city: location?.city?.value,
      cityId: location?.city?.id,
      province: location?.city?.province?.value,
      country: location?.city?.province?.country?.value,
      postalCode: location?.postalCode,
      hasVehicle: hasVehicle,
      availabilities: availabilities.map((e) => e.value ?? '').where((v) => v.isNotEmpty).toList(),
      availabilityIds: availabilities.map((e) => e.id ?? '').where((v) => v.isNotEmpty).toList(),
      availabilityTimes: availabilityTimes.map((e) => e.value ?? '').where((v) => v.isNotEmpty).toList(),
      availabilityTimeIds: availabilityTimes.map((e) => e.id ?? '').where((v) => v.isNotEmpty).toList(),
      availabilityDays: availabilityDays.map((e) => e.value ?? '').where((v) => v.isNotEmpty).toList(),
      availabilityDayIds: availabilityDays.map((e) => e.id ?? '').where((v) => v.isNotEmpty).toList(),
      liftCapacity: lift?.value,
      liftId: lift?.id,
      languages: languages.map((e) => e.value ?? '').where((v) => v.isNotEmpty).toList(),
      languageIds: languages.map((e) => e.id ?? '').where((v) => v.isNotEmpty).toList(),
      skills: skills.map((e) => e.skill ?? '').where((v) => v.isNotEmpty).toList(),
      skillIds: skills.map((e) => e.id ?? '').where((v) => v.isNotEmpty).toList(),
      hasResume: resume != null,
      resumeFileName: resume?.fileName,
      resumeFileUrl: resume?.pathFile,
      approvedToWork: approvedToWork,
      punchCardId: punchCardId,
      haveAnyHealthProblem: haveAnyHealthProblem,
      contactEmergencyName: contactEmergencyName,
      contactEmergencyLastName: contactEmergencyLastName,
      contactEmergencyPhone: contactEmergencyPhone,
    );
  }
}