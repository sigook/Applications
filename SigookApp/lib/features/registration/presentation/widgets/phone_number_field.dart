import 'package:flutter/material.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../core/theme/app_theme.dart';

class PhoneNumberField extends StatefulWidget {
  final String? initialValue;
  final String countryCode; // ISO code (US, CA)
  final String? errorText;
  final Function(String) onChanged;
  final Function(bool)? onFocusChanged;
  final String? label;
  final String? hint;

  const PhoneNumberField({
    super.key,
    this.initialValue,
    required this.countryCode,
    this.errorText,
    required this.onChanged,
    this.onFocusChanged,
    this.label,
    this.hint,
  });

  @override
  State<PhoneNumberField> createState() => _PhoneNumberFieldState();
}

class _PhoneNumberFieldState extends State<PhoneNumberField> {
  late TextEditingController _controller;
  late FocusNode _focusNode;
  late MaskTextInputFormatter _maskFormatter;

  @override
  void initState() {
    super.initState();
    _maskFormatter = MaskTextInputFormatter(
      mask: '### ### ####',
      filter: {"#": RegExp(r'[0-9]')},
      type: MaskAutoCompletionType.lazy,
    );
    final initialText =
        widget.initialValue != null && widget.initialValue!.isNotEmpty
        ? _maskFormatter.maskText(widget.initialValue!)
        : '';
    _controller = TextEditingController(text: initialText);
    _focusNode = FocusNode();
    _focusNode.addListener(_handleFocusChange);
  }

  @override
  void didUpdateWidget(PhoneNumberField oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.initialValue != oldWidget.initialValue) {
      final newText =
          widget.initialValue != null && widget.initialValue!.isNotEmpty
          ? _maskFormatter.maskText(widget.initialValue!)
          : '';
      if (_controller.text != newText) {
        _controller.text = newText;
      }
    }
  }

  @override
  void dispose() {
    _controller.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  void _handleFocusChange() {
    widget.onFocusChanged?.call(_focusNode.hasFocus);
  }

  void _onChanged(String value) {
    final digitsOnly = _maskFormatter.getUnmaskedText();
    widget.onChanged(digitsOnly);
  }

  String _getPlaceholder() {
    if (widget.hint != null) return widget.hint!;

    switch (widget.countryCode.toUpperCase()) {
      case 'US':
      case 'CA':
        return '555 123 4567';
      default:
        return 'Enter phone number';
    }
  }

  String _getCountryPrefix() {
    switch (widget.countryCode.toUpperCase()) {
      case 'US':
      case 'CA':
        return '+1';
      default:
        return '+1';
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        if (widget.label != null) ...[
          Text(
            widget.label!,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w600,
              color: AppTheme.textDark,
            ),
          ),
          const SizedBox(height: 8),
        ],
        TextField(
          controller: _controller,
          focusNode: _focusNode,
          keyboardType: TextInputType.phone,
          inputFormatters: [
            _maskFormatter, // Apply mask for visual formatting
          ],
          decoration: InputDecoration(
            hintText: _getPlaceholder(),
            prefixIcon: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    _getCountryPrefix(),
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.w500,
                      color: AppTheme.textDark,
                    ),
                  ),
                  const SizedBox(width: 8),
                  Container(width: 1, height: 24, color: Colors.grey.shade300),
                ],
              ),
            ),
            errorText: widget.errorText,
            filled: true,
            fillColor: Colors.white,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(
                color: AppTheme.primaryBlue,
                width: 2,
              ),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red),
            ),
            focusedErrorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red, width: 2),
            ),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 16,
            ),
          ),
          onChanged: _onChanged,
        ),
        if (widget.errorText == null) ...[
          const SizedBox(height: 4),
          Text(
            'Format: ${_getPlaceholder()}',
            style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
          ),
        ],
      ],
    );
  }
}
