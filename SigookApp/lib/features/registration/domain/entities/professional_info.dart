import 'package:equatable/equatable.dart';

/// Professional information entity
/// Represents the professional section of the registration form
class ProfessionalInfo extends Equatable {
  final List<String> languages;
  final List<String> skills;

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
  ProfessionalInfo copyWith({List<String>? languages, List<String>? skills}) {
    return ProfessionalInfo(
      languages: languages ?? this.languages,
      skills: skills ?? this.skills,
    );
  }

  @override
  List<Object?> get props => [languages, skills];
}
