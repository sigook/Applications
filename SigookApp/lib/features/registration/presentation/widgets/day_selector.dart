import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/day_of_week.dart';

class DaySelector extends ConsumerWidget {
  final List<DayOfWeekEntity> selectedDays;
  final Function(List<DayOfWeekEntity>) onChanged;
  final String? errorText;

  const DaySelector({
    super.key,
    required this.selectedDays,
    required this.onChanged,
    this.errorText,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final daysAsync = ref.watch(daysOfWeekProvider);
    final catalogDays = daysAsync.value ?? [];
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Available Days',
          style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
        const SizedBox(height: 12),
        if (catalogDays.isEmpty)
          Text(
            'Loading days...',
            style: TextStyle(color: Colors.grey.shade600),
          )
        else
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: catalogDays.map((catalogDay) {
              final isSelected = selectedDays.any((d) => d.id == catalogDay.id || d.value == catalogDay.value);
              return FilterChip(
                label: Text(catalogDay.value),
                selected: isSelected,
                onSelected: (selected) {
                  final newDays = List<DayOfWeekEntity>.from(selectedDays);
                  if (selected) {
                    newDays.add(DayOfWeekEntity(
                      id: catalogDay.id ?? '',
                      value: catalogDay.value,
                    ));
                  } else {
                    newDays.removeWhere((d) => d.id == catalogDay.id || d.value == catalogDay.value);
                  }
                  onChanged(newDays);
                },
                selectedColor: AppTheme.primaryBlue.withValues(alpha: 0.2),
                checkmarkColor: AppTheme.primaryBlue,
                labelStyle: TextStyle(
                  color: isSelected ? AppTheme.primaryBlue : Colors.black87,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),
        if (errorText != null) ...[
          const SizedBox(height: 8),
          Text(
            errorText!,
            style: const TextStyle(color: Colors.red, fontSize: 12),
          ),
        ],
      ],
    );
  }
}
