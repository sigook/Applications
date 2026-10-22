import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/display/profile_info_row.dart';

class SearchableDropdownField extends StatefulWidget {
  final String label;
  final String hint;
  final String? selectedValue;
  final List<String> options;
  final ValueChanged<String?> onChanged;
  final String? errorText;
  final IconData icon;
  final bool isLoading;
  final bool isRequired;
  final bool dense;

  const SearchableDropdownField({
    super.key,
    required this.label,
    required this.hint,
    this.selectedValue,
    required this.options,
    required this.onChanged,
    this.errorText,
    this.icon = Icons.arrow_drop_down_circle,
    this.isLoading = false,
    this.isRequired = false,
    this.dense = false,
  });

  @override
  State<SearchableDropdownField> createState() =>
      _SearchableDropdownFieldState();
}

class _SearchableDropdownFieldState extends State<SearchableDropdownField> {
  late final TextEditingController _controller;
  final _focusNode = FocusNode();

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.selectedValue);
    _controller.addListener(_onTextChanged);
    _focusNode.addListener(_onFocusChanged);
  }

  void _onFocusChanged() {
    if (_focusNode.hasFocus) return;
    final selected = widget.selectedValue ?? '';
    if (_controller.text != selected) _controller.text = selected;
  }

  @override
  void didUpdateWidget(SearchableDropdownField oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.selectedValue != oldWidget.selectedValue &&
        _controller.text != (widget.selectedValue ?? '')) {
      _controller.text = widget.selectedValue ?? '';
    }
  }

  @override
  void dispose() {
    _controller.removeListener(_onTextChanged);
    _focusNode.removeListener(_onFocusChanged);
    _controller.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  void _onTextChanged() {
    if (!mounted) return;
    setState(() {});
    if (_controller.text.isEmpty && widget.selectedValue != null) {
      widget.onChanged(null);
    }
  }

  void _clear() {
    _controller.clear();
    _focusNode.requestFocus();
  }

  Widget _buildHeader() {
    return Row(
      children: [
        Text(
          widget.label,
          style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
        ),
        if (widget.isRequired)
          const Text(
            ' *',
            style: TextStyle(
              color: Colors.red,
              fontSize: 16,
              fontWeight: FontWeight.bold,
            ),
          ),
      ],
    );
  }

  InputDecoration _decoration() {
    final suffix = _controller.text.isNotEmpty
        ? IconButton(
            tooltip: 'Clear',
            icon: const Icon(Icons.clear, size: 18),
            onPressed: _clear,
          )
        : const Icon(Icons.arrow_drop_down);

    if (widget.dense) {
      return ProfileInfoRow.decoration(
        hintText: widget.hint,
        errorText: widget.errorText,
        suffixIcon: suffix,
      );
    }

    return InputDecoration(
      hintText: widget.hint,
      prefixIcon: Icon(widget.icon),
      suffixIcon: suffix,
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
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
        borderSide: const BorderSide(color: AppTheme.errorRed, width: 2),
      ),
      errorText: widget.errorText,
      filled: true,
      fillColor: Colors.white,
    );
  }

  @override
  Widget build(BuildContext context) {
    if (widget.isLoading) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (!widget.dense) ...[_buildHeader(), const SizedBox(height: 12)],
          const Center(
            child: Padding(
              padding: EdgeInsets.symmetric(vertical: 8),
              child: SizedBox(
                height: 24,
                width: 24,
                child: CircularProgressIndicator(strokeWidth: 2),
              ),
            ),
          ),
        ],
      );
    }

    final field = Autocomplete<String>(
      textEditingController: _controller,
      focusNode: _focusNode,
      optionsBuilder: (TextEditingValue textEditingValue) {
        final query = textEditingValue.text.trim().toLowerCase();
        if (query.isEmpty || textEditingValue.text == widget.selectedValue) {
          return widget.options;
        }
        return widget.options.where(
          (option) => option.toLowerCase().contains(query),
        );
      },
      onSelected: (String selection) {
        _controller.text = selection;
        widget.onChanged(selection);
      },
      fieldViewBuilder:
          (context, fieldTextEditingController, fieldFocusNode, _) {
            return TextField(
              controller: fieldTextEditingController,
              focusNode: fieldFocusNode,
              decoration: _decoration(),
            );
          },
      optionsViewBuilder: (context, onSelected, options) {
        return Align(
          alignment: Alignment.topLeft,
          child: Material(
            elevation: 4.0,
            borderRadius: BorderRadius.circular(12),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxHeight: 300, maxWidth: 400),
              child: ListView.builder(
                padding: const EdgeInsets.symmetric(vertical: 8),
                shrinkWrap: true,
                physics: const AlwaysScrollableScrollPhysics(),
                itemCount: options.length,
                itemBuilder: (context, index) {
                  final option = options.elementAt(index);
                  final isSelected = widget.selectedValue == option;
                  return InkWell(
                    onTap: () => onSelected(option),
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 16,
                        vertical: 12,
                      ),
                      color: isSelected
                          ? AppTheme.primaryBlue.withValues(alpha: 0.1)
                          : null,
                      child: Row(
                        children: [
                          Icon(
                            widget.icon,
                            size: 20,
                            color: isSelected
                                ? AppTheme.primaryBlue
                                : Colors.grey,
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Text(
                              option,
                              style: TextStyle(
                                fontSize: 14,
                                fontWeight: isSelected
                                    ? FontWeight.w600
                                    : FontWeight.normal,
                                color: isSelected
                                    ? AppTheme.primaryBlue
                                    : Colors.black87,
                              ),
                            ),
                          ),
                          if (isSelected)
                            const Icon(
                              Icons.check,
                              size: 20,
                              color: AppTheme.primaryBlue,
                            ),
                        ],
                      ),
                    ),
                  );
                },
              ),
            ),
          ),
        );
      },
    );

    if (widget.dense) return field;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [_buildHeader(), const SizedBox(height: 12), field],
    );
  }
}
