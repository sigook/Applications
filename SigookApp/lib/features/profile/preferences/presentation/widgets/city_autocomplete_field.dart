import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../catalog/domain/entities/catalog_item.dart';

class CityAutocompleteField extends StatefulWidget {
  final String label;
  final IconData? icon;
  final AsyncValue<List<CatalogItem>> citiesAsync;
  final Set<String> selectedIds;
  final void Function(String id, bool selected) onToggle;

  const CityAutocompleteField({
    super.key,
    required this.label,
    this.icon,
    required this.citiesAsync,
    required this.selectedIds,
    required this.onToggle,
  });

  @override
  State<CityAutocompleteField> createState() => _CityAutocompleteFieldState();
}

class _CityAutocompleteFieldState extends State<CityAutocompleteField> {
  TextEditingController? _fieldController;

  Widget _buildLabel() {
    return Row(
      children: [
        if (widget.icon != null) ...[
          Icon(widget.icon, size: 18, color: Colors.grey.shade700),
          const SizedBox(width: 6),
        ],
        Text(
          widget.label,
          style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildLabel(),
        const SizedBox(height: 12),
        widget.citiesAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (_, _) => Text(
            'Failed to load cities',
            style: TextStyle(color: Colors.red.shade700),
          ),
          data: (cities) {
            final available =
                cities.where((c) => c.id != null && c.id!.isNotEmpty).toList();
            final selected =
                available.where((c) => widget.selectedIds.contains(c.id)).toList();

            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Autocomplete<CatalogItem>(
                  displayStringForOption: (city) => city.value,
                  optionsBuilder: (value) {
                    final query = value.text.trim().toLowerCase();
                    if (query.isEmpty) return const Iterable<CatalogItem>.empty();
                    return available.where((city) =>
                        !widget.selectedIds.contains(city.id) &&
                        city.value.toLowerCase().contains(query));
                  },
                  onSelected: (city) {
                    widget.onToggle(city.id!, true);
                    _fieldController?.clear();
                  },
                  fieldViewBuilder:
                      (context, controller, focusNode, onFieldSubmitted) {
                    _fieldController = controller;
                    return TextField(
                      controller: controller,
                      focusNode: focusNode,
                      onSubmitted: (_) => onFieldSubmitted(),
                      decoration: InputDecoration(
                        hintText: 'Type to search cities...',
                        prefixIcon: const Icon(Icons.location_city_outlined),
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
                        filled: true,
                        fillColor: Colors.white,
                      ),
                    );
                  },
                  optionsViewBuilder: (context, onSelected, options) {
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
                            itemCount: options.length,
                            itemBuilder: (context, index) {
                              final city = options.elementAt(index);
                              return InkWell(
                                onTap: () => onSelected(city),
                                child: Padding(
                                  padding: const EdgeInsets.symmetric(
                                    horizontal: 16,
                                    vertical: 12,
                                  ),
                                  child: Row(
                                    children: [
                                      const Icon(
                                        Icons.location_city_outlined,
                                        size: 20,
                                        color: AppTheme.primaryBlue,
                                      ),
                                      const SizedBox(width: 12),
                                      Expanded(
                                        child: Text(
                                          city.value,
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
                if (selected.isNotEmpty) ...[
                  const SizedBox(height: 16),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: selected.map((city) {
                      return Chip(
                        label: Text(city.value),
                        deleteIcon: const Icon(Icons.close, size: 18),
                        onDeleted: () => widget.onToggle(city.id!, false),
                        backgroundColor:
                            AppTheme.primaryBlue.withValues(alpha: 0.1),
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
        ),
      ],
    );
  }
}
