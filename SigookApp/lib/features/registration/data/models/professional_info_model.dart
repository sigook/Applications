import 'package:freezed_annotation/freezed_annotation.dart';
import '../../domain/entities/professional_info.dart';

part 'professional_info_model.freezed.dart';
part 'professional_info_model.g.dart';

@freezed
class ProfessionalInfoModel with _$ProfessionalInfoModel {
  const ProfessionalInfoModel._();

  const factory ProfessionalInfoModel({
    required List<String> languages,
    required List<String> skills,
  }) = _ProfessionalInfoModel;

  /// Convert from domain entity
  factory ProfessionalInfoModel.fromEntity(ProfessionalInfo entity) {
    return ProfessionalInfoModel(
      languages: entity.languages,
      skills: entity.skills,
    );
  }

  /// Convert to domain entity
  ProfessionalInfo toEntity() {
    return ProfessionalInfo(
      languages: languages,
      skills: skills,
    );
  }

  /// From JSON
  factory ProfessionalInfoModel.fromJson(Map<String, dynamic> json) =>
      _$ProfessionalInfoModelFromJson(json);
}
