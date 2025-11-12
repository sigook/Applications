import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/value_objects/name.dart';

part 'basic_info_model.freezed.dart';
part 'basic_info_model.g.dart';

@freezed
class BasicInfoModel with _$BasicInfoModel {
  const BasicInfoModel._();

  const factory BasicInfoModel({
    required String firstName,
    required String lastName,
    required String dateOfBirth, // ISO 8601 string
    required String genderId,
    required String genderValue,
    required String country,
    required String provinceState,
    required String city,
    required String address,
    required String zipCode,
    required String mobileNumber,
    String? identificationType,
    String? identificationNumber,
  }) = _BasicInfoModel;

  /// Convert from domain entity
  factory BasicInfoModel.fromEntity(BasicInfo entity) {
    return BasicInfoModel(
      firstName: entity.firstName.value,
      lastName: entity.lastName.value,
      dateOfBirth: entity.dateOfBirth.toIso8601String(),
      genderId: entity.gender.id ?? '',
      genderValue: entity.gender.value,
      country: entity.country,
      provinceState: entity.provinceState,
      city: entity.city,
      address: entity.address,
      zipCode: entity.zipCode,
      mobileNumber: entity.mobileNumber,
      identificationType: entity.identificationType,
      identificationNumber: entity.identificationNumber,
    );
  }

  /// Convert to domain entity
  BasicInfo toEntity() {
    return BasicInfo(
      firstName: Name(firstName),
      lastName: Name(lastName),
      dateOfBirth: DateTime.parse(dateOfBirth),
      gender: Gender(id: genderId, value: genderValue),
      country: country,
      provinceState: provinceState,
      city: city,
      address: address,
      zipCode: zipCode,
      mobileNumber: mobileNumber,
      identificationType: identificationType,
      identificationNumber: identificationNumber,
    );
  }

  /// From JSON
  factory BasicInfoModel.fromJson(Map<String, dynamic> json) =>
      _$BasicInfoModelFromJson(json);
}
