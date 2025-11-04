import 'package:flutter/material.dart';

class CustomTextField extends StatelessWidget {
  final String label;
  final String? hint;
  final String? initialValue;
  final String? errorText;
  final bool obscureText;
  final TextInputType? keyboardType;
  final int? maxLines;
  final ValueChanged<String>? onChanged;
  final bool enabled;
  final Widget? suffixIcon;

  const CustomTextField({
    super.key,
    required this.label,
    this.hint,
    this.initialValue,
    this.errorText,
    this.obscureText = false,
    this.keyboardType,
    this.maxLines = 1,
    this.onChanged,
    this.enabled = true,
    this.suffixIcon,
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
        TextFormField(
          initialValue: initialValue,
          obscureText: obscureText,
          keyboardType: keyboardType,
          maxLines: maxLines,
          enabled: enabled,
          decoration: InputDecoration(
            hintText: hint,
            errorText: errorText,
            suffixIcon: suffixIcon,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(
                color: Theme.of(context).colorScheme.primary,
                width: 2,
              ),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red),
            ),
            filled: true,
            fillColor: enabled ? Colors.white : Colors.grey.shade100,
          ),
          onChanged: onChanged,
        ),
      ],
    );
  }
}
