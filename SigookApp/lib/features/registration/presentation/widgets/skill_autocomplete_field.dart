import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/notifiers/catalog_notifiers.dart';
import '../../../registration/domain/entities/skill.dart';

/// Autocomplete text field for skills
/// Allows user to type and select from API suggestions only
class SkillAutocompleteField extends ConsumerStatefulWidget {
  final List<Skill> selectedSkills;
  final ValueChanged<List<Skill>> onChanged;
  final String? errorText;

  const SkillAutocompleteField({
    super.key,
    required this.selectedSkills,
    required this.onChanged,
    this.errorText,
  });

  @override
  ConsumerState<SkillAutocompleteField> createState() =>
      _SkillAutocompleteFieldState();
}

class _SkillAutocompleteFieldState
    extends ConsumerState<SkillAutocompleteField> {
  final TextEditingController _controller = TextEditingController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _addSkill(Skill skill) {
    if (widget.selectedSkills.any((s) => s.skill == skill.skill)) return;

    final updatedList = [...widget.selectedSkills, skill];
    widget.onChanged(updatedList);
    _controller.clear();
  }

  void _removeSkill(String skillName) {
    final updatedList = widget.selectedSkills
        .where((skill) => skill.skill != skillName)
        .toList();
    widget.onChanged(updatedList);
  }

  @override
  Widget build(BuildContext context) {
    final skillsAsync = ref.watch(skillsProvider);

    return skillsAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Skills',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          const Center(child: CircularProgressIndicator()),
        ],
      ),
      error: (error, _) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Skills',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.red.shade50,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.red.shade200),
            ),
            child: Row(
              children: [
                Icon(Icons.error_outline, color: Colors.red.shade700, size: 20),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'Failed to load skills',
                    style: TextStyle(color: Colors.red.shade700),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.read(skillsProvider.notifier).refresh();
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (skills) {

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Skills',
              style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
            ),
            const SizedBox(height: 12),
            Autocomplete<String>(
              optionsBuilder: (TextEditingValue textEditingValue) {
                if (textEditingValue.text.isEmpty) {
                  return const Iterable<String>.empty();
                }
                return skills
                    .map((skill) => skill.value)
                    .where((String option) {
                  return option.toLowerCase().contains(
                    textEditingValue.text.toLowerCase(),
                  );
                });
              },
              onSelected: (String selection) {
                // Create skill entity from selected value
                _addSkill(Skill(skill: selection));
              },
              fieldViewBuilder:
                  (
                    BuildContext context,
                    TextEditingController fieldTextEditingController,
                    FocusNode fieldFocusNode,
                    VoidCallback onFieldSubmitted,
                  ) {
                    // Sync controllers
                    _controller.text = fieldTextEditingController.text;
                    fieldTextEditingController.addListener(() {
                      _controller.text = fieldTextEditingController.text;
                    });

                    return TextField(
                      controller: fieldTextEditingController,
                      focusNode: fieldFocusNode,
                      decoration: InputDecoration(
                        hintText: 'Type to search or add custom skill...',
                        helperText: 'Press Enter to add custom skill or select from suggestions',
                        helperMaxLines: 2,
                        prefixIcon: const Icon(Icons.workspace_premium),
                        suffixIcon: fieldTextEditingController.text.isNotEmpty
                            ? IconButton(
                                icon: const Icon(Icons.clear),
                                onPressed: () {
                                  fieldTextEditingController.clear();
                                  _controller.clear();
                                },
                              )
                            : null,
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        enabledBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: BorderSide(color: Colors.grey.shade300),
                        ),
                        focusedBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: const BorderSide(
                            color: AppTheme.primaryBlue,
                            width: 2,
                          ),
                        ),
                        errorBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: const BorderSide(
                            color: AppTheme.errorRed,
                            width: 2,
                          ),
                        ),
                        filled: true,
                        fillColor: Colors.white,
                      ),
                      onSubmitted: (value) {
                        if (value.trim().isEmpty) return;
                        
                        // Allow adding any skill - from suggestions or custom
                        final trimmedValue = value.trim();
                        final matchingSkill = skills.where(
                          (skill) => skill.value.toLowerCase() == trimmedValue.toLowerCase(),
                        ).firstOrNull;
                        
                        if (matchingSkill != null) {
                          // Use the skill from suggestions (preserves correct casing)
                          _addSkill(Skill(skill: matchingSkill.value));
                        } else {
                          // Allow custom skill entry
                          _addSkill(Skill(skill: trimmedValue));
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('Added custom skill: "$trimmedValue"'),
                              backgroundColor: AppTheme.successGreen,
                              duration: const Duration(seconds: 2),
                            ),
                          );
                        }
                        fieldTextEditingController.clear();
                      },
                    );
                  },
              optionsViewBuilder:
                  (
                    BuildContext context,
                    AutocompleteOnSelected<String> onSelected,
                    Iterable<String> options,
                  ) {
                    return Align(
                      alignment: Alignment.topLeft,
                      child: Material(
                        elevation: 4.0,
                        borderRadius: BorderRadius.circular(12),
                        child: ConstrainedBox(
                          constraints: const BoxConstraints(
                            maxHeight: 300,
                            maxWidth: 400,
                          ),
                          child: ListView.builder(
                            padding: const EdgeInsets.symmetric(vertical: 8),
                            shrinkWrap: true,
                            physics: const AlwaysScrollableScrollPhysics(),
                            itemCount: options.length,
                            itemBuilder: (BuildContext context, int index) {
                              final String option = options.elementAt(index);
                              return InkWell(
                                onTap: () {
                                  onSelected(option);
                                },
                                child: Container(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 16,
                                    vertical: 12,
                                  ),
                                  child: Row(
                                    children: [
                                      const Icon(
                                        Icons.workspace_premium,
                                        size: 20,
                                        color: AppTheme.primaryBlue,
                                      ),
                                      const SizedBox(width: 12),
                                      Expanded(
                                        child: Text(
                                          option,
                                          style: const TextStyle(fontSize: 14),
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                              );
                            },
                          ),
                        ),
                      ),
                    );
                  },
            ),
            if (widget.errorText != null) ...[
              const SizedBox(height: 8),
              Padding(
                padding: const EdgeInsets.only(left: 12),
                child: Text(
                  widget.errorText!,
                  style: const TextStyle(
                    color: AppTheme.errorRed,
                    fontSize: 12,
                  ),
                ),
              ),
            ],
            if (widget.selectedSkills.isNotEmpty) ...[
              const SizedBox(height: 16),
              Wrap(
                spacing: 8,
                runSpacing: 8,
                children: widget.selectedSkills.map((skill) {
                  return Chip(
                    label: Text(skill.skill),
                    deleteIcon: const Icon(Icons.close, size: 18),
                    onDeleted: () => _removeSkill(skill.skill),
                    backgroundColor: AppTheme.successGreen.withValues(
                      alpha: 0.1,
                    ),
                    labelStyle: const TextStyle(
                      color: AppTheme.successGreen,
                      fontWeight: FontWeight.w600,
                    ),
                  );
                }).toList(),
              ),
            ],
          ],
        );
      },
    );
  }
}
