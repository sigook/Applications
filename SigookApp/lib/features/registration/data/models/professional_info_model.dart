import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/professional_info.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';

part 'professional_info_model.freezed.dart';
part 'professional_info_model.g.dart';

@freezed
sealed class ProfessionalInfoModel with _$ProfessionalInfoModel {
  const ProfessionalInfoModel._();

  const factory ProfessionalInfoModel({
    required Map<String, String>
    languages, // {id: value} - only languages with IDs
    required List<String> skills, // List of skill names
  }) = _ProfessionalInfoModel;

  /// Convert from domain entity
  factory ProfessionalInfoModel.fromEntity(ProfessionalInfo entity) {
    return ProfessionalInfoModel(
      // Only include languages that have IDs
      languages: {
        for (var lang in entity.languages.where((l) => l.id != null))
          lang.id!: lang.value,
      },
      skills: entity.skills.map((skill) => skill.skill).toList(),
    );
  }

  /// Convert to domain entity
  ProfessionalInfo toEntity() {
    return ProfessionalInfo(
      languages: languages.entries
          .map((e) => Language(id: e.key, value: e.value))
          .toList(),
      skills: skills.map((skillName) => Skill(skill: skillName)).toList(),
    );
  }

  /// From JSON
  factory ProfessionalInfoModel.fromJson(Map<String, dynamic> json) =>
      _$ProfessionalInfoModelFromJson(json);
}
