import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';

class AvailabilityTimeSelector extends ConsumerWidget {
  final List<String> selectedTimes;
  final Function(List<String>) onChanged;
  final String? errorText;

  const AvailabilityTimeSelector({
    super.key,
    required this.selectedTimes,
    required this.onChanged,
    this.errorText,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final timeSlotsAsync = ref.watch(availabilityTimeListProvider);
    final timeSlots = timeSlotsAsync.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Available Time Slots',
          style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
        const SizedBox(height: 12),
        if (timeSlots.isEmpty)
          Text(
            'No time slots available',
            style: TextStyle(color: Colors.grey.shade600),
          )
        else
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: timeSlots.map((slot) {
              final isSelected = selectedTimes.contains(slot.value);
              return FilterChip(
                label: Text(slot.value),
                selected: isSelected,
                onSelected: (selected) {
                  final newTimes = List<String>.from(selectedTimes);
                  if (selected) {
                    newTimes.add(slot.value);
                  } else {
                    newTimes.remove(slot.value);
                  }
                  onChanged(newTimes);
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
