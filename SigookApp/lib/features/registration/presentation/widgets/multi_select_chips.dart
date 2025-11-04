import 'package:flutter/material.dart';

class MultiSelectChips<T> extends StatelessWidget {
  final String label;
  final List<T> options;
  final List<T> selectedOptions;
  final String Function(T) getLabel;
  final ValueChanged<List<T>> onChanged;
  final String? errorText;

  const MultiSelectChips({
    super.key,
    required this.label,
    required this.options,
    required this.selectedOptions,
    required this.getLabel,
    required this.onChanged,
    this.errorText,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.w600,
              ),
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: options.map((option) {
            final isSelected = selectedOptions.contains(option);
            return FilterChip(
              label: Text(
                getLabel(option),
                style: const TextStyle(
                  color: Color(0xFF212121), // Dark grey text - always visible
                  fontSize: 14,
                ),
              ),
              selected: isSelected,
              onSelected: (selected) {
                final newSelection = List<T>.from(selectedOptions);
                if (selected) {
                  newSelection.add(option);
                } else {
                  newSelection.remove(option);
                }
                onChanged(newSelection);
              },
              selectedColor: Theme.of(context).colorScheme.primary.withValues(alpha: 0.2),
              checkmarkColor: Theme.of(context).colorScheme.primary,
              backgroundColor: const Color(0xFFF5F5F5), // Light grey background
              labelStyle: const TextStyle(
                color: Color(0xFF212121), // Ensure dark text
              ),
            );
          }).toList(),
        ),
        if (errorText != null)
          Padding(
            padding: const EdgeInsets.only(top: 8, left: 12),
            child: Text(
              errorText!,
              style: TextStyle(
                color: Theme.of(context).colorScheme.error,
                fontSize: 12,
              ),
            ),
          ),
      ],
    );
  }
}
