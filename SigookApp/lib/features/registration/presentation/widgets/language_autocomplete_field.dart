import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/notifiers/catalog_notifiers.dart';
import '../../../registration/domain/entities/language.dart';

/// Autocomplete text field for languages
/// Allows user to type and select from API suggestions only
class LanguageAutocompleteField extends ConsumerStatefulWidget {
  final List<Language> selectedLanguages;
  final ValueChanged<List<Language>> onChanged;
  final String? errorText;

  const LanguageAutocompleteField({
    super.key,
    required this.selectedLanguages,
    required this.onChanged,
    this.errorText,
  });

  @override
  ConsumerState<LanguageAutocompleteField> createState() =>
      _LanguageAutocompleteFieldState();
}

class _LanguageAutocompleteFieldState
    extends ConsumerState<LanguageAutocompleteField> {
  final TextEditingController _controller = TextEditingController();

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _addLanguage(Language language) {
    if (widget.selectedLanguages.any((l) => l.id == language.id)) return;

    final updatedList = [...widget.selectedLanguages, language];
    widget.onChanged(updatedList);
    _controller.clear();
  }

  void _removeLanguage(String languageValue) {
    final updatedList = widget.selectedLanguages
        .where((lang) => lang.value != languageValue)
        .toList();
    widget.onChanged(updatedList);
  }

  @override
  Widget build(BuildContext context) {
    final languagesAsync = ref.watch(languagesProvider);

    return languagesAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Languages',
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
            'Languages',
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
                    'Failed to load languages',
                    style: TextStyle(color: Colors.red.shade700),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.read(languagesProvider.notifier).refresh();
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (languages) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Languages',
              style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
            ),
            const SizedBox(height: 12),
            Autocomplete<String>(
              optionsBuilder: (TextEditingValue textEditingValue) {
                if (textEditingValue.text.isEmpty) {
                  return const Iterable<String>.empty();
                }
                return languages
                    .map((lang) => lang.value)
                    .where((String option) {
                  return option.toLowerCase().contains(
                    textEditingValue.text.toLowerCase(),
                  );
                });
              },
              onSelected: (String selection) {
                // Find the language and convert to entity
                final catalogItem = languages.firstWhere(
                  (lang) => lang.value == selection,
                );
                _addLanguage(Language(id: catalogItem.id, value: catalogItem.value));
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
                        hintText: 'Type to search languages...',
                        prefixIcon: const Icon(Icons.language),
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
                        // Only allow if it's in the list
                        final matchingLang = languages.where(
                          (lang) => lang.value == value,
                        ).firstOrNull;
                        
                        if (matchingLang != null) {
                          _addLanguage(Language(id: matchingLang.id, value: matchingLang.value));
                        } else {
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text(
                                'Please select a language from the suggestions',
                              ),
                              backgroundColor: AppTheme.errorRed,
                            ),
                          );
                          fieldTextEditingController.clear();
                        }
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
                                        Icons.language,
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
            if (widget.selectedLanguages.isNotEmpty) ...[
              const SizedBox(height: 16),
              Wrap(
                spacing: 8,
                runSpacing: 8,
                children: widget.selectedLanguages.map((language) {
                  return Chip(
                    label: Text(language.value),
                    deleteIcon: const Icon(Icons.close, size: 18),
                    onDeleted: () => _removeLanguage(language.value),
                    backgroundColor: AppTheme.primaryBlue.withValues(
                      alpha: 0.1,
                    ),
                    labelStyle: const TextStyle(
                      color: AppTheme.primaryBlue,
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
