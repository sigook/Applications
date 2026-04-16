import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

/// A tappable date field that displays a selected [DateTime] or a placeholder.
/// The actual date picker logic is owned by the caller via [onTap].
class DatePickerField extends StatelessWidget {
  final String label;
  final DateTime? value;
  final VoidCallback onTap;

  const DatePickerField({
    super.key,
    required this.label,
    required this.value,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: InputDecorator(
        decoration: InputDecoration(
          labelText: label,
          prefixIcon: const Icon(Icons.calendar_today_outlined, size: 20),
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
          contentPadding:
              const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          isDense: true,
        ),
        child: Text(
          value != null
              ? '${value!.year}-${value!.month.toString().padLeft(2, '0')}-${value!.day.toString().padLeft(2, '0')}'
              : 'Select date',
          style: TextStyle(
            fontSize: 14,
            color: value != null ? AppTheme.textDark : Colors.grey,
          ),
        ),
      ),
    );
  }
}
