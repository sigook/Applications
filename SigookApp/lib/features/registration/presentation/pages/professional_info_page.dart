import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/professional_info.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../providers/registration_providers.dart';
import '../widgets/language_autocomplete_field.dart';
import '../widgets/skill_autocomplete_field.dart';

class ProfessionalInfoPage extends ConsumerStatefulWidget {
  const ProfessionalInfoPage({super.key});

  @override
  ConsumerState<ProfessionalInfoPage> createState() =>
      _ProfessionalInfoPageState();
}

class _ProfessionalInfoPageState extends ConsumerState<ProfessionalInfoPage> {
  List<Language> _languages = [];
  List<Skill> _skills = [];

  String? _languagesError;
  String? _skillsError;

  @override
  void initState() {
    super.initState();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.professionalInfo != null) {
        _languages = List.from(form.professionalInfo!.languages);
        _skills = List.from(form.professionalInfo!.skills);
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      _languagesError = _languages.isEmpty
          ? 'Please select at least one language'
          : null;
      _skillsError = _skills.isEmpty
          ? 'Please select at least one skill'
          : null;

      if (_languagesError == null && _skillsError == null) {
        final professionalInfo = ProfessionalInfo(
          languages: _languages,
          skills: _skills,
        );
        ref
            .read(registrationViewModelProvider.notifier)
            .updateProfessionalInfo(professionalInfo);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;

    return SingleChildScrollView(
      padding: EdgeInsets.all(isMobile ? 12 : 16),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Card(
        elevation: 0,
        color: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Professional Information',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Please provide your professional details',
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),
              LanguageAutocompleteField(
                selectedLanguages: _languages,
                onChanged: (languages) {
                  setState(() {
                    _languages = languages;
                  });
                  _validateAndSave();
                },
                errorText: _languagesError,
              ),
              const SizedBox(height: 32),
              SkillAutocompleteField(
                selectedSkills: _skills,
                onChanged: (skills) {
                  setState(() {
                    _skills = skills;
                  });
                  _validateAndSave();
                },
                errorText: _skillsError,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
