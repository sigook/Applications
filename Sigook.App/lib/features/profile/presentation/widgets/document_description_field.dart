import 'package:flutter/material.dart';

class DocumentDescriptionField extends StatelessWidget {
  static const maxLength = 100;
  static final _allowed = RegExp(r'^[-_ a-zA-Z0-9]+$');

  final TextEditingController controller;
  final String? errorText;
  final ValueChanged<String>? onChanged;

  const DocumentDescriptionField({
    super.key,
    required this.controller,
    this.errorText,
    this.onChanged,
  });

  static String? validate(String value) {
    final text = value.trim();
    if (text.isEmpty) return 'Description is required';
    if (text.length > maxLength) {
      return 'Description must be at most $maxLength characters';
    }
    if (!_allowed.hasMatch(text)) {
      return 'Only letters, numbers, spaces, - and _ are allowed';
    }
    return null;
  }

  @override
  Widget build(BuildContext context) {
    return TextField(
      controller: controller,
      maxLength: maxLength,
      onChanged: onChanged,
      decoration: InputDecoration(
        labelText: 'Description *',
        errorText: errorText,
        counterText: '',
        prefixIcon: const Icon(Icons.short_text_outlined, size: 20),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 12,
        ),
        isDense: true,
      ),
    );
  }
}
