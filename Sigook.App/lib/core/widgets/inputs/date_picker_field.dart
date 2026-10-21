import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';
import '../display/profile_info_row.dart';

/// A tappable date field that displays a selected [DateTime] or a placeholder.
/// The actual date picker logic is owned by the caller via [onTap].
/// [dense] renders it with the same decoration as an editing [ProfileInfoRow]
/// and without a floating label.
class DatePickerField extends StatelessWidget {
  final String label;
  final DateTime? value;
  final VoidCallback onTap;
  final String? errorText;
  final bool dense;

  const DatePickerField({
    super.key,
    required this.label,
    required this.value,
    required this.onTap,
    this.errorText,
    this.dense = false,
  });

  String get _text => value != null
      ? '${value!.year}-${value!.month.toString().padLeft(2, '0')}-${value!.day.toString().padLeft(2, '0')}'
      : 'Select date';

  @override
  Widget build(BuildContext context) {
    final text = Text(
      _text,
      style: TextStyle(
        fontSize: 14,
        color: value != null ? AppTheme.textDark : Colors.grey,
      ),
    );

    return Semantics(
      button: true,
      label: dense ? label : null,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(dense ? 8 : 12),
        child: dense
            ? InputDecorator(
                decoration: ProfileInfoRow.decoration(
                  errorText: errorText,
                  suffixIcon: const Icon(Icons.calendar_today_outlined, size: 18),
                ),
                child: text,
              )
            : InputDecorator(
                decoration: InputDecoration(
                  labelText: label,
                  errorText: errorText,
                  prefixIcon: const Icon(Icons.calendar_today_outlined, size: 20),
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  contentPadding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 12,
                  ),
                  isDense: true,
                ),
                child: text,
              ),
      ),
    );
  }
}
