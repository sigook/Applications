import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class DaySelector extends StatelessWidget {
  final List<String> selectedDays;
  final Function(List<String>) onChanged;
  final String? errorText;

  const DaySelector({
    super.key,
    required this.selectedDays,
    required this.onChanged,
    this.errorText,
  });

  static const List<String> _weekDays = [
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday',
  ];

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Available Days',
          style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
        const SizedBox(height: 12),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: _weekDays.map((day) {
            final isSelected = selectedDays.contains(day);
            return FilterChip(
              label: Text(day),
              selected: isSelected,
              onSelected: (selected) {
                final newDays = List<String>.from(selectedDays);
                if (selected) {
                  newDays.add(day);
                } else {
                  newDays.remove(day);
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
