import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/profile_photo.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/zip_code.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/value_objects/name.dart';
import '../../domain/entities/value_objects/phone_number.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/city.dart';

part 'basic_info_model.freezed.dart';
part 'basic_info_model.g.dart';

@freezed
sealed class BasicInfoModel with _$BasicInfoModel {
  const BasicInfoModel._();

  const factory BasicInfoModel({
    required String firstName,
    required String lastName,
    required String dateOfBirth, // ISO 8601 string
    required String genderId,
    required String genderValue,
    Map<String, dynamic>? country,
    Map<String, dynamic>? provinceState,
    Map<String, dynamic>? city,
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
      country: entity.country?.toJson(),
      provinceState: entity.provinceState?.toJson(),
      city: entity.city?.toJson(),
      address: entity.address,
      zipCode: entity.zipCode.value,
      mobileNumber: entity.mobileNumber.e164Format,
      identificationType: entity.identificationType,
      identificationNumber: entity.identificationNumber,
    );
  }

  /// Convert to domain entity
  BasicInfo toEntity() {
    return BasicInfo(
      profilePhoto: ProfilePhoto(path: ''),
      firstName: Name(firstName),
      lastName: Name(lastName),
      dateOfBirth: DateTime.parse(dateOfBirth),
      gender: Gender(id: genderId, value: genderValue),
      country: country != null
          ? Country(
              id: country!['id'] as String?,
              value: country!['value'] as String,
              code: country!['code'] as String?,
            )
          : null,
      provinceState: provinceState != null
          ? Province(
              id: provinceState!['id'] as String?,
              value: provinceState!['value'] as String,
              code: provinceState!['code'] as String?,
              country: country != null
                  ? Country(
                      id: country!['id'] as String?,
                      value: country!['value'] as String,
                      code: country!['code'] as String?,
                    )
                  : Country(id: null, value: '', code: null),
            )
          : null,
      city: city != null
          ? City(
              id: city!['id'] as String?,
              value: city!['value'] as String,
              code: city!['code'] as String?,
              province: provinceState != null
                  ? Province(
                      id: provinceState!['id'] as String?,
                      value: provinceState!['value'] as String,
                      code: provinceState!['code'] as String?,
                      country: country != null
                          ? Country(
                              id: country!['id'] as String?,
                              value: country!['value'] as String,
                              code: country!['code'] as String?,
                            )
                          : Country(id: null, value: '', code: null),
                    )
                  : Province(
                      id: null,
                      value: '',
                      code: null,
                      country: Country(id: null, value: '', code: null),
                    ),
            )
          : null,
      address: address,
      zipCode:
          ZipCode.parse(
            input: zipCode,
            countryCode: country?['code'] as String? ?? 'US',
            provinceCode: provinceState?['code'] as String?,
          ).fold(
            (error) =>
                country?['code'] == 'CA' ? ZipCode.emptyCA : ZipCode.emptyUS,
            (validZip) => validZip,
          ),
      mobileNumber: PhoneNumber(value: mobileNumber),
      identificationType: identificationType,
      identificationNumber: identificationNumber,
    );
  }

  /// From JSON
  factory BasicInfoModel.fromJson(Map<String, dynamic> json) =>
      _$BasicInfoModelFromJson(json);
}
