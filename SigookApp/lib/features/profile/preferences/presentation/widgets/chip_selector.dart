import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/theme/app_theme.dart';

/// Async chip selector backed by a Riverpod [AsyncValue].
/// Supports single-select and multi-select via [singleSelect].
class ChipSelector extends StatelessWidget {
  final String label;
  final AsyncValue<dynamic> asyncValue;
  final Set<String> selectedIds;
  final bool singleSelect;
  final void Function(String id, bool selected) onToggle;

  const ChipSelector({
    super.key,
    required this.label,
    required this.asyncValue,
    required this.selectedIds,
    required this.singleSelect,
    required this.onToggle,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(
            fontSize: 12,
            color: Colors.grey.shade600,
            fontWeight: FontWeight.w500,
          ),
        ),
        const SizedBox(height: 6),
        asyncValue.when(
          data: (items) {
            final list = items as List;
            return Wrap(
              spacing: 6,
              runSpacing: 4,
              children: list.map((item) {
                final id = (item.id as String?) ?? '';
                final value = item.value != null
                    ? item.value as String
                    : item.toString();
                final isSelected = selectedIds.contains(id);
                return FilterChip(
                  label: Text(value, style: const TextStyle(fontSize: 12)),
                  selected: isSelected,
                  onSelected: (s) => onToggle(id, s),
                  selectedColor:
                      AppTheme.primaryBlue.withValues(alpha: 0.15),
                  checkmarkColor: AppTheme.primaryBlue,
                  labelStyle: TextStyle(
                    color: isSelected ? AppTheme.primaryBlue : AppTheme.textDark,
                    fontWeight:
                        isSelected ? FontWeight.w600 : FontWeight.normal,
                  ),
                  side: BorderSide(
                    color: isSelected
                        ? AppTheme.primaryBlue
                        : Colors.grey.shade300,
                  ),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 4,
                    vertical: 0,
                  ),
                );
              }).toList(),
            );
          },
          loading: () => const SizedBox(
            height: 24,
            width: 24,
            child: CircularProgressIndicator(strokeWidth: 2),
          ),
          error: (_, _) => Text(
            'Failed to load options',
            style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
          ),
        ),
      ],
    );
  }
}
