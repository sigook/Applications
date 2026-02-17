// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'worker_profile_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ProfileImageModel _$ProfileImageModelFromJson(Map<String, dynamic> json) =>
    _ProfileImageModel(
      id: json['id'] as String?,
      pathFile: json['pathFile'] as String?,
      fileName: json['fileName'] as String?,
      description: json['description'] as String?,
      canDownload: json['canDownload'] as bool? ?? false,
    );

Map<String, dynamic> _$ProfileImageModelToJson(_ProfileImageModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'pathFile': instance.pathFile,
      'fileName': instance.fileName,
      'description': instance.description,
      'canDownload': instance.canDownload,
    };

_CatalogItemModel _$CatalogItemModelFromJson(Map<String, dynamic> json) =>
    _CatalogItemModel(
      id: json['id'] as String?,
      value: json['value'] as String?,
    );

Map<String, dynamic> _$CatalogItemModelToJson(_CatalogItemModel instance) =>
    <String, dynamic>{'id': instance.id, 'value': instance.value};

_SkillItemModel _$SkillItemModelFromJson(Map<String, dynamic> json) =>
    _SkillItemModel(id: json['id'] as String?, skill: json['skill'] as String?);

Map<String, dynamic> _$SkillItemModelToJson(_SkillItemModel instance) =>
    <String, dynamic>{'id': instance.id, 'skill': instance.skill};

_CountryModel _$CountryModelFromJson(Map<String, dynamic> json) =>
    _CountryModel(
      id: json['id'] as String?,
      value: json['value'] as String?,
      code: json['code'] as String?,
    );

Map<String, dynamic> _$CountryModelToJson(_CountryModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'value': instance.value,
      'code': instance.code,
    };

_ProvinceModel _$ProvinceModelFromJson(Map<String, dynamic> json) =>
    _ProvinceModel(
      id: json['id'] as String?,
      value: json['value'] as String?,
      code: json['code'] as String?,
      country: json['country'] == null
          ? null
          : CountryModel.fromJson(json['country'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$ProvinceModelToJson(_ProvinceModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'value': instance.value,
      'code': instance.code,
      'country': instance.country,
    };

_CityModel _$CityModelFromJson(Map<String, dynamic> json) => _CityModel(
  id: json['id'] as String?,
  value: json['value'] as String?,
  code: json['code'] as String?,
  province: json['province'] == null
      ? null
      : ProvinceModel.fromJson(json['province'] as Map<String, dynamic>),
);

Map<String, dynamic> _$CityModelToJson(_CityModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'value': instance.value,
      'code': instance.code,
      'province': instance.province,
    };

_LocationModel _$LocationModelFromJson(Map<String, dynamic> json) =>
    _LocationModel(
      id: json['id'] as String?,
      address: json['address'] as String?,
      city: json['city'] == null
          ? null
          : CityModel.fromJson(json['city'] as Map<String, dynamic>),
      postalCode: json['postalCode'] as String?,
      entrance: json['entrance'] as String?,
      mainIntersection: json['mainIntersection'] as String?,
      isBilling: json['isBilling'] as bool? ?? false,
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
      formattedAddress: json['formattedAddress'] as String?,
      isUSA: json['isUSA'] as bool? ?? false,
    );

Map<String, dynamic> _$LocationModelToJson(_LocationModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'address': instance.address,
      'city': instance.city,
      'postalCode': instance.postalCode,
      'entrance': instance.entrance,
      'mainIntersection': instance.mainIntersection,
      'isBilling': instance.isBilling,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'formattedAddress': instance.formattedAddress,
      'isUSA': instance.isUSA,
    };

_WorkerProfileListItemModel _$WorkerProfileListItemModelFromJson(
  Map<String, dynamic> json,
) => _WorkerProfileListItemModel(
  id: json['id'] as String,
  agencyFullName: json['agencyFullName'] as String?,
  agencyLogo: json['agencyLogo'] as String?,
);

Map<String, dynamic> _$WorkerProfileListItemModelToJson(
  _WorkerProfileListItemModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'agencyFullName': instance.agencyFullName,
  'agencyLogo': instance.agencyLogo,
};

_WorkerProfileModel _$WorkerProfileModelFromJson(Map<String, dynamic> json) =>
    _WorkerProfileModel(
      id: json['id'] as String,
      numberId: (json['numberId'] as num?)?.toInt(),
      profileImage: json['profileImage'] == null
          ? null
          : ProfileImageModel.fromJson(
              json['profileImage'] as Map<String, dynamic>,
            ),
      firstName: json['firstName'] as String?,
      middleName: json['middleName'] as String?,
      lastName: json['lastName'] as String?,
      secondLastName: json['secondLastName'] as String?,
      birthDay: json['birthDay'] as String?,
      gender: json['gender'] == null
          ? null
          : CatalogItemModel.fromJson(json['gender'] as Map<String, dynamic>),
      socialInsurance: json['socialInsurance'] as String?,
      socialInsuranceExpire: json['socialInsuranceExpire'] as bool? ?? false,
      dueDate: json['dueDate'] as String?,
      socialInsuranceFile: json['socialInsuranceFile'] == null
          ? null
          : ProfileImageModel.fromJson(
              json['socialInsuranceFile'] as Map<String, dynamic>,
            ),
      identificationNumber1: json['identificationNumber1'] as String?,
      identificationNumber2: json['identificationNumber2'] as String?,
      havePoliceCheckBackground:
          json['havePoliceCheckBackground'] as bool? ?? false,
      identificationType1File: json['identificationType1File'] == null
          ? null
          : ProfileImageModel.fromJson(
              json['identificationType1File'] as Map<String, dynamic>,
            ),
      identificationType2File: json['identificationType2File'] == null
          ? null
          : ProfileImageModel.fromJson(
              json['identificationType2File'] as Map<String, dynamic>,
            ),
      identificationType1: json['identificationType1'] == null
          ? null
          : CatalogItemModel.fromJson(
              json['identificationType1'] as Map<String, dynamic>,
            ),
      identificationType2: json['identificationType2'] == null
          ? null
          : CatalogItemModel.fromJson(
              json['identificationType2'] as Map<String, dynamic>,
            ),
      policeCheckBackGround: json['policeCheckBackGround'] == null
          ? null
          : ProfileImageModel.fromJson(
              json['policeCheckBackGround'] as Map<String, dynamic>,
            ),
      mobileNumber: json['mobileNumber'] as String?,
      phone: json['phone'] as String?,
      phoneExt: json['phoneExt'] as String?,
      location: json['location'] == null
          ? null
          : LocationModel.fromJson(json['location'] as Map<String, dynamic>),
      hasVehicle: json['hasVehicle'] as bool? ?? false,
      licenses:
          (json['licenses'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      certificates:
          (json['certificates'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      otherDocuments:
          (json['otherDocuments'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      availabilities:
          (json['availabilities'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      availabilityTimes:
          (json['availabilityTimes'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      availabilityDays:
          (json['availabilityDays'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      locationPreferences:
          (json['locationPreferences'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      lift: json['lift'] == null
          ? null
          : CatalogItemModel.fromJson(json['lift'] as Map<String, dynamic>),
      languages:
          (json['languages'] as List<dynamic>?)
              ?.map((e) => CatalogItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      skills:
          (json['skills'] as List<dynamic>?)
              ?.map((e) => SkillItemModel.fromJson(e as Map<String, dynamic>))
              .toList() ??
          const [],
      resume: json['resume'] == null
          ? null
          : ProfileImageModel.fromJson(json['resume'] as Map<String, dynamic>),
      haveAnyHealthProblem: json['haveAnyHealthProblem'] as bool? ?? false,
      healthProblem: json['healthProblem'] as String?,
      otherHealthProblem: json['otherHealthProblem'] as String?,
      contactEmergencyName: json['contactEmergencyName'] as String?,
      contactEmergencyLastName: json['contactEmergencyLastName'] as String?,
      contactEmergencyPhone: json['contactEmergencyPhone'] as String?,
      email: json['email'] as String?,
      approvedToWork: json['approvedToWork'] as bool? ?? false,
      workerId: json['workerId'] as String?,
      isSubcontractor: json['isSubcontractor'] as bool? ?? false,
      isContractor: json['isContractor'] as bool? ?? false,
      dnu: json['dnu'] as bool? ?? false,
      punchCardId: json['punchCardId'] as String?,
    );

Map<String, dynamic> _$WorkerProfileModelToJson(_WorkerProfileModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'numberId': instance.numberId,
      'profileImage': instance.profileImage,
      'firstName': instance.firstName,
      'middleName': instance.middleName,
      'lastName': instance.lastName,
      'secondLastName': instance.secondLastName,
      'birthDay': instance.birthDay,
      'gender': instance.gender,
      'socialInsurance': instance.socialInsurance,
      'socialInsuranceExpire': instance.socialInsuranceExpire,
      'dueDate': instance.dueDate,
      'socialInsuranceFile': instance.socialInsuranceFile,
      'identificationNumber1': instance.identificationNumber1,
      'identificationNumber2': instance.identificationNumber2,
      'havePoliceCheckBackground': instance.havePoliceCheckBackground,
      'identificationType1File': instance.identificationType1File,
      'identificationType2File': instance.identificationType2File,
      'identificationType1': instance.identificationType1,
      'identificationType2': instance.identificationType2,
      'policeCheckBackGround': instance.policeCheckBackGround,
      'mobileNumber': instance.mobileNumber,
      'phone': instance.phone,
      'phoneExt': instance.phoneExt,
      'location': instance.location,
      'hasVehicle': instance.hasVehicle,
      'licenses': instance.licenses,
      'certificates': instance.certificates,
      'otherDocuments': instance.otherDocuments,
      'availabilities': instance.availabilities,
      'availabilityTimes': instance.availabilityTimes,
      'availabilityDays': instance.availabilityDays,
      'locationPreferences': instance.locationPreferences,
      'lift': instance.lift,
      'languages': instance.languages,
      'skills': instance.skills,
      'resume': instance.resume,
      'haveAnyHealthProblem': instance.haveAnyHealthProblem,
      'healthProblem': instance.healthProblem,
      'otherHealthProblem': instance.otherHealthProblem,
      'contactEmergencyName': instance.contactEmergencyName,
      'contactEmergencyLastName': instance.contactEmergencyLastName,
      'contactEmergencyPhone': instance.contactEmergencyPhone,
      'email': instance.email,
      'approvedToWork': instance.approvedToWork,
      'workerId': instance.workerId,
      'isSubcontractor': instance.isSubcontractor,
      'isContractor': instance.isContractor,
      'dnu': instance.dnu,
      'punchCardId': instance.punchCardId,
    };
