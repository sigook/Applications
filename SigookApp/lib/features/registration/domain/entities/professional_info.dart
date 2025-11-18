import 'package:equatable/equatable.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/language.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/skill.dart';

/// Professional information entity
/// Represents the professional section of the registration form
class ProfessionalInfo extends Equatable {
  final List<Language> languages;
  final List<Skill> skills;

  const ProfessionalInfo({required this.languages, required this.skills});

  /// Validates all fields
  bool get isValid {
    return languages.isNotEmpty && skills.isNotEmpty;
  }

  /// Validation error messages
  String? get languagesError =>
      languages.isEmpty ? 'Please add at least one language' : null;
  String? get skillsError =>
      skills.isEmpty ? 'Please add at least one skill' : null;

  /// Creates a copy with updated fields
  ProfessionalInfo copyWith({List<Language>? languages, List<Skill>? skills}) {
    return ProfessionalInfo(
      languages: languages ?? this.languages,
      skills: skills ?? this.skills,
    );
  }

  @override
  List<Object?> get props => [languages, skills];
}
