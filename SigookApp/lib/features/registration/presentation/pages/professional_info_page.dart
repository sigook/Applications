import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/professional_info.dart';
import '../providers/registration_providers.dart';

class ProfessionalInfoPage extends ConsumerStatefulWidget {
  const ProfessionalInfoPage({super.key});

  @override
  ConsumerState<ProfessionalInfoPage> createState() => _ProfessionalInfoPageState();
}

class _ProfessionalInfoPageState extends ConsumerState<ProfessionalInfoPage> {
  final TextEditingController _languageController = TextEditingController();
  final TextEditingController _skillController = TextEditingController();
  
  List<String> _languages = [];
  List<String> _skills = [];

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
    _languageController.dispose();
    _skillController.dispose();
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      _languagesError = _languages.isEmpty ? 'Please add at least one language' : null;
      _skillsError = _skills.isEmpty ? 'Please add at least one skill' : null;

      if (_languagesError == null && _skillsError == null) {
        final professionalInfo = ProfessionalInfo(
          languages: _languages,
          skills: _skills,
        );
        ref.read(registrationViewModelProvider.notifier).updateProfessionalInfo(professionalInfo);
      }
    });
  }

  void _addLanguage() {
    if (_languageController.text.trim().isNotEmpty) {
      setState(() {
        _languages.add(_languageController.text.trim());
        _languageController.clear();
      });
      _validateAndSave();
    }
  }

  void _addSkill() {
    if (_skillController.text.trim().isNotEmpty) {
      setState(() {
        _skills.add(_skillController.text.trim());
        _skillController.clear();
      });
      _validateAndSave();
    }
  }

  void _removeLanguage(String language) {
    setState(() {
      _languages.remove(language);
    });
    _validateAndSave();
  }

  void _removeSkill(String skill) {
    setState(() {
      _skills.remove(skill);
    });
    _validateAndSave();
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
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.grey.shade600,
                ),
          ),
          const SizedBox(height: 32),
          _buildSection(
            title: 'Languages',
            controller: _languageController,
            items: _languages,
            errorText: _languagesError,
            hint: 'e.g., English, Spanish',
            onAdd: _addLanguage,
            onRemove: _removeLanguage,
          ),
          const SizedBox(height: 32),
          _buildSection(
            title: 'Skills',
            controller: _skillController,
            items: _skills,
            errorText: _skillsError,
            hint: 'e.g., Flutter, UI/UX Design',
            onAdd: _addSkill,
            onRemove: _removeSkill,
          ),
        ],
      ),
        ),
      ),
    );
  }

  Widget _buildSection({
    required String title,
    required TextEditingController controller,
    required List<String> items,
    required String? errorText,
    required String hint,
    required VoidCallback onAdd,
    required void Function(String) onRemove,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          title,
          style: Theme.of(context).textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: TextField(
                controller: controller,
                decoration: InputDecoration(
                  hintText: hint,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  enabledBorder: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                    borderSide: BorderSide(color: Colors.grey.shade300),
                  ),
                  filled: true,
                  fillColor: Colors.white,
                ),
                onSubmitted: (_) => onAdd(),
              ),
            ),
            const SizedBox(width: 12),
            ElevatedButton(
              onPressed: onAdd,
              style: ElevatedButton.styleFrom(
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                padding: const EdgeInsets.all(16),
              ),
              child: const Icon(Icons.add),
            ),
          ],
        ),
        if (errorText != null)
          Padding(
            padding: const EdgeInsets.only(top: 8, left: 12),
            child: Text(
              errorText,
              style: TextStyle(
                color: Theme.of(context).colorScheme.error,
                fontSize: 12,
              ),
            ),
          ),
        const SizedBox(height: 16),
        if (items.isNotEmpty)
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: items.map((item) {
              return Chip(
                label: Text(item),
                onDeleted: () => onRemove(item),
                deleteIcon: const Icon(Icons.close, size: 18),
              );
            }).toList(),
          ),
      ],
    );
  }
}
