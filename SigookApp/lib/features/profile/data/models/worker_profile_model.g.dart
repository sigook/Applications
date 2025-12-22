// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'worker_profile_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

WorkerProfileModel _$WorkerProfileModelFromJson(Map<String, dynamic> json) =>
    WorkerProfileModel(
      id: json['id'] as String,
      firstName: json['firstName'] as String?,
      lastName: json['lastName'] as String?,
      email: json['email'] as String?,
      profilePhoto: json['profilePhoto'] as String?,
      phoneNumber: json['phoneNumber'] as String?,
      address: json['address'] as String?,
      city: json['city'] as String?,
      state: json['state'] as String?,
      zipCode: json['zipCode'] as String?,
      dateOfBirth: json['dateOfBirth'] == null
          ? null
          : DateTime.parse(json['dateOfBirth'] as String),
    );

Map<String, dynamic> _$WorkerProfileModelToJson(WorkerProfileModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'firstName': instance.firstName,
      'lastName': instance.lastName,
      'email': instance.email,
      'profilePhoto': instance.profilePhoto,
      'phoneNumber': instance.phoneNumber,
      'address': instance.address,
      'city': instance.city,
      'state': instance.state,
      'zipCode': instance.zipCode,
      'dateOfBirth': instance.dateOfBirth?.toIso8601String(),
    };
