import 'package:freezed_annotation/freezed_annotation.dart';
import '../../../../core/constants/enums.dart';
import '../../domain/entities/personal_info.dart';
import '../../domain/entities/value_objects/name.dart';

part 'personal_info_model.freezed.dart';
part 'personal_info_model.g.dart';

@freezed
class PersonalInfoModel with _$PersonalInfoModel {
  const PersonalInfoModel._();

  const factory PersonalInfoModel({
    required String firstName,
    required String lastName,
    required DateTime dateOfBirth,
    required Gender gender,
  }) = _PersonalInfoModel;

  /// Convert from domain entity
  factory PersonalInfoModel.fromEntity(PersonalInfo entity) {
    return PersonalInfoModel(
      firstName: entity.firstName.value,
      lastName: entity.lastName.value,
      dateOfBirth: entity.dateOfBirth,
      gender: entity.gender,
    );
  }

  /// Convert to domain entity
  PersonalInfo toEntity() {
    return PersonalInfo(
      firstName: Name(firstName),
      lastName: Name(lastName),
      dateOfBirth: dateOfBirth,
      gender: gender,
    );
  }

  /// From JSON
  factory PersonalInfoModel.fromJson(Map<String, dynamic> json) =>
      _$PersonalInfoModelFromJson(json);
}
