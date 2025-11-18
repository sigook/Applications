import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';

/// Autocomplete dropdown for country selection
/// Allows user to type and search from API-provided country list
class CountryAutocompleteField extends ConsumerStatefulWidget {
  final String? selectedCountryId;
  final String? selectedCountryName;
  final ValueChanged<CatalogItem?> onChanged;
  final String? errorText;

  const CountryAutocompleteField({
    super.key,
    this.selectedCountryId,
    this.selectedCountryName,
    required this.onChanged,
    this.errorText,
  });

  @override
  ConsumerState<CountryAutocompleteField> createState() =>
      _CountryAutocompleteFieldState();
}

class _CountryAutocompleteFieldState
    extends ConsumerState<CountryAutocompleteField> {
  final TextEditingController _controller = TextEditingController();

  @override
  void initState() {
    super.initState();
    if (widget.selectedCountryName != null) {
      _controller.text = widget.selectedCountryName!;
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final countriesAsync = ref.watch(countriesListProvider);

    return countriesAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Country',
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
            'Country',
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
                    'Failed to load countries',
                    style: TextStyle(color: Colors.red.shade700),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.invalidate(countriesListProvider);
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (countries) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Country',
              style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
            ),
            const SizedBox(height: 12),
            Autocomplete<CatalogItem>(
              initialValue: widget.selectedCountryName != null
                  ? TextEditingValue(text: widget.selectedCountryName!)
                  : null,
              optionsBuilder: (TextEditingValue textEditingValue) {
                if (textEditingValue.text.isEmpty) {
                  return countries; // Show all when empty
                }
                return countries.where((CatalogItem option) {
                  return option.value.toLowerCase().contains(
                    textEditingValue.text.toLowerCase(),
                  );
                });
              },
              displayStringForOption: (CatalogItem option) => option.value,
              onSelected: (CatalogItem selection) {
                _controller.text = selection.value;
                widget.onChanged(selection);
              },
              fieldViewBuilder: (
                BuildContext context,
                TextEditingController fieldTextEditingController,
                FocusNode fieldFocusNode,
                VoidCallback onFieldSubmitted,
              ) {
                return TextField(
                  controller: fieldTextEditingController,
                  focusNode: fieldFocusNode,
                  decoration: InputDecoration(
                    hintText: 'Select or search country...',
                    prefixIcon: const Icon(Icons.public),
                    suffixIcon: fieldTextEditingController.text.isNotEmpty
                        ? IconButton(
                            icon: const Icon(Icons.clear),
                            onPressed: () {
                              fieldTextEditingController.clear();
                              widget.onChanged(null);
                            },
                          )
                        : const Icon(Icons.arrow_drop_down),
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
                    errorText: widget.errorText,
                    filled: true,
                    fillColor: Colors.white,
                  ),
                );
              },
              optionsViewBuilder: (
                BuildContext context,
                AutocompleteOnSelected<CatalogItem> onSelected,
                Iterable<CatalogItem> options,
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
                          final CatalogItem option = options.elementAt(index);
                          final bool isSelected =
                              widget.selectedCountryId == option.id;
                          return InkWell(
                            onTap: () {
                              onSelected(option);
                            },
                            child: Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 16,
                                vertical: 12,
                              ),
                              color: isSelected
                                  ? AppTheme.primaryBlue.withValues(alpha: 0.1)
                                  : null,
                              child: Row(
                                children: [
                                  Icon(
                                    Icons.public,
                                    size: 20,
                                    color: isSelected
                                        ? AppTheme.primaryBlue
                                        : Colors.grey,
                                  ),
                                  const SizedBox(width: 12),
                                  Expanded(
                                    child: Text(
                                      option.value,
                                      style: TextStyle(
                                        fontSize: 14,
                                        fontWeight: isSelected
                                            ? FontWeight.w600
                                            : FontWeight.normal,
                                        color: isSelected
                                            ? AppTheme.primaryBlue
                                            : Colors.black87,
                                      ),
                                    ),
                                  ),
                                  if (isSelected)
                                    const Icon(
                                      Icons.check,
                                      size: 20,
                                      color: AppTheme.primaryBlue,
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
          ],
        );
      },
    );
  }
}
