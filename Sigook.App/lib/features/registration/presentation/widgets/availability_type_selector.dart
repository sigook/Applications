import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/availability_type.dart';

class AvailabilityTypeSelector extends ConsumerWidget {
  final AvailabilityType? selectedType;
  final Function(AvailabilityType?) onChanged;
  final String? errorText;

  const AvailabilityTypeSelector({
    super.key,
    this.selectedType,
    required this.onChanged,
    this.errorText,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final availabilityAsync = ref.watch(availabilityListProvider);
    final availability = availabilityAsync.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Availability Type',
          style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
        const SizedBox(height: 12),
        if (availability.isEmpty)
          Text(
            'No availability types available',
            style: TextStyle(color: Colors.grey.shade600),
          )
        else
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: availability.map((type) {
              final isSelected =
                  selectedType?.id == type.id ||
                  selectedType?.value == type.value;
              return FilterChip(
                label: Text(type.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    onChanged(AvailabilityType(id: type.id, value: type.value));
                  }
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
