import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:phone_numbers_parser/phone_numbers_parser.dart';
import '../../../../core/theme/app_theme.dart';

/// Phone number input field with country-aware formatting
/// Follows Single Responsibility - handles phone number input UI only
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

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.initialValue ?? '');
    _focusNode = FocusNode();
    _focusNode.addListener(_handleFocusChange);
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

  void _formatPhoneNumber(String value) {
    if (value.isEmpty) {
      widget.onChanged('');
      return;
    }

    try {
      // Format as you type using phone_numbers_parser
      final isoCode = _getIsoCode(widget.countryCode);
      final parsed = PhoneNumber.parse(value, callerCountry: isoCode);
      
      // Get national format for display
      final formatted = parsed.formatNsn();

      if (formatted.isNotEmpty && formatted != value) {
        setState(() {
          // Update controller without triggering onChange again
          final cursorPos = _controller.selection.baseOffset;
          _controller.value = TextEditingValue(
            text: formatted,
            selection: TextSelection.collapsed(
              offset: cursorPos.clamp(0, formatted.length),
            ),
          );
        });
      }

      widget.onChanged(value);
    } catch (e) {
      // If formatting fails, use raw value
      widget.onChanged(value);
    }
  }

  /// Convert country code to IsoCode for phone_numbers_parser
  IsoCode _getIsoCode(String countryCode) {
    switch (countryCode.toUpperCase()) {
      case 'US':
        return IsoCode.US;
      case 'CA':
        return IsoCode.CA;
      default:
        return IsoCode.US;
    }
  }

  String _getPlaceholder() {
    if (widget.hint != null) return widget.hint!;
    
    switch (widget.countryCode.toUpperCase()) {
      case 'US':
      case 'CA':
        return '(555) 123-4567';
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
              color: Color(0xFF1E293B),
            ),
          ),
          const SizedBox(height: 8),
        ],
        TextField(
          controller: _controller,
          focusNode: _focusNode,
          keyboardType: TextInputType.phone,
          inputFormatters: [
            FilteringTextInputFormatter.allow(RegExp(r'[0-9+\-\(\)\s]')),
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
                      color: Color(0xFF1E293B),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Container(
                    width: 1,
                    height: 24,
                    color: Colors.grey.shade300,
                  ),
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
              borderSide: const BorderSide(color: AppTheme.primaryBlue, width: 2),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red),
            ),
            focusedErrorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red, width: 2),
            ),
            contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
          ),
          onChanged: _formatPhoneNumber,
        ),
        if (widget.errorText == null) ...[
          const SizedBox(height: 4),
          Text(
            'Format: ${_getPlaceholder()}',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade600,
            ),
          ),
        ],
      ],
    );
  }
}
