import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/personal_info.dart';
import '../../domain/entities/value_objects/name.dart';
import '../../domain/entities/gender.dart';

part 'personal_info_model.freezed.dart';
part 'personal_info_model.g.dart';

@freezed
class PersonalInfoModel with _$PersonalInfoModel {
  const PersonalInfoModel._();

  const factory PersonalInfoModel({
    required String firstName,
    required String lastName,
    required DateTime dateOfBirth,
    String? genderId, // UUID from API (may be null)
    required String genderName, // Name from API
  }) = _PersonalInfoModel;

  /// Convert from domain entity
  factory PersonalInfoModel.fromEntity(PersonalInfo entity) {
    return PersonalInfoModel(
      firstName: entity.firstName.value,
      lastName: entity.lastName.value,
      dateOfBirth: entity.dateOfBirth,
      genderId: entity.gender.id, // May be null
      genderName: entity.gender.value,
    );
  }

  /// Convert to domain entity
  PersonalInfo toEntity() {
    return PersonalInfo(
      firstName: Name(firstName),
      lastName: Name(lastName),
      dateOfBirth: dateOfBirth,
      gender: Gender(id: genderId, value: genderName),
    );
  }

  /// From JSON
  factory PersonalInfoModel.fromJson(Map<String, dynamic> json) =>
      _$PersonalInfoModelFromJson(json);
}
