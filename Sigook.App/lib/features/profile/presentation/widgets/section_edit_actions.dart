import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

/// Save / Cancel / Edit action set for a profile section card trailing widget.
///
/// - When [isEditingThis] is true: shows save (or spinner) + cancel.
/// - When another section is being edited ([isAnyEditing] true): hides all.
/// - Otherwise: shows the edit pencil icon.
class SectionEditActions extends StatelessWidget {
  final bool isEditingThis;
  final bool isAnyEditing;
  final bool isSaving;
  final VoidCallback onSave;
  final VoidCallback onCancel;

  /// Null disables the edit button (e.g. while profile data is loading).
  final VoidCallback? onEdit;

  const SectionEditActions({
    super.key,
    required this.isEditingThis,
    required this.isAnyEditing,
    required this.isSaving,
    required this.onSave,
    required this.onCancel,
    this.onEdit,
  });

  @override
  Widget build(BuildContext context) {
    if (isEditingThis) {
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (isSaving)
            const SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(
                strokeWidth: 2,
                color: AppTheme.primaryBlue,
              ),
            )
          else
            IconButton(
              icon: const Icon(
                Icons.check_circle_outline,
                color: AppTheme.primaryBlue,
              ),
              onPressed: onSave,
              padding: EdgeInsets.zero,
              constraints: const BoxConstraints(),
              tooltip: 'Save',
            ),
          const SizedBox(width: 8),
          IconButton(
            icon: Icon(Icons.cancel_outlined, color: Colors.grey.shade500),
            onPressed: isSaving ? null : onCancel,
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(),
            tooltip: 'Cancel',
          ),
        ],
      );
    }
    if (isAnyEditing) return const SizedBox.shrink();
    return IconButton(
      icon: const Icon(
        Icons.edit_outlined,
        color: AppTheme.primaryBlue,
        size: 20,
      ),
      onPressed: onEdit,
      padding: EdgeInsets.zero,
      constraints: const BoxConstraints(),
      tooltip: 'Edit',
    );
  }
}
