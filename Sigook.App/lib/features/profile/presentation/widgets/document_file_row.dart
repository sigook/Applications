import 'package:flutter/material.dart';
import '../../../../core/services/file_picker_service.dart';
import '../../../../core/theme/app_theme.dart';

/// A document row that handles view and edit states for a single file field.
///
/// In view mode: shows the file name and an optional preview button.
/// In edit mode: shows pending/deletion state with undo, replace, delete, and upload actions.
class DocumentFileRow extends StatelessWidget {
  final String label;
  final String? fileName;
  final String? fileUrl;
  final bool isMarkedForDeletion;
  final PickedFileData? pendingFile;
  final bool isEditing;
  final VoidCallback onDelete;
  final VoidCallback onUndo;
  final VoidCallback onPickFile;
  final VoidCallback onClearPick;
  final VoidCallback? onPreview;

  const DocumentFileRow({
    super.key,
    required this.label,
    required this.fileName,
    required this.fileUrl,
    required this.isMarkedForDeletion,
    required this.pendingFile,
    required this.isEditing,
    required this.onDelete,
    required this.onUndo,
    required this.onPickFile,
    required this.onClearPick,
    this.onPreview,
  });

  @override
  Widget build(BuildContext context) {
    final hasFile = fileName != null && fileName!.isNotEmpty;
    final hasPending = pendingFile != null;
    final hasUrl = fileUrl != null && fileUrl!.isNotEmpty;

    if (!isEditing) {
      return Padding(
        padding: const EdgeInsets.only(bottom: 12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Icon(
              Icons.attach_file_outlined,
              size: 20,
              color: AppTheme.primaryBlue,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: TextStyle(
                      fontSize: 12,
                      color: Colors.grey.shade600,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    hasFile ? label.replaceAll(' (File)', '') : 'Not uploaded',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w600,
                      color: hasFile ? AppTheme.textDark : Colors.grey,
                    ),
                  ),
                ],
              ),
            ),
            if (hasUrl)
              IconButton(
                onPressed: onPreview,
                icon: const Icon(Icons.visibility_outlined, size: 20),
                color: AppTheme.primaryBlue,
                tooltip: 'Preview',
                padding: EdgeInsets.zero,
                constraints:
                    const BoxConstraints(minWidth: 32, minHeight: 32),
              ),
          ],
        ),
      );
    }

    // ── Edit mode ────────────────────────────────────────────────────────────

    final String displayName;
    final TextStyle nameStyle;
    final String? statusText;

    if (hasPending) {
      displayName = pendingFile!.name;
      nameStyle = const TextStyle(fontSize: 14, color: AppTheme.primaryBlue);
      statusText = hasFile ? 'Will replace on save' : 'Will upload on save';
    } else if (isMarkedForDeletion) {
      displayName = fileName!;
      nameStyle = const TextStyle(
        fontSize: 14,
        color: AppTheme.errorRed,
        decoration: TextDecoration.lineThrough,
      );
      statusText = 'Will be removed on save';
    } else {
      displayName = hasFile ? fileName! : 'Not uploaded';
      nameStyle = TextStyle(
        fontSize: 14,
        color: hasFile ? AppTheme.textDark : Colors.grey,
      );
      statusText = null;
    }

    final Widget actions;
    if (hasPending || isMarkedForDeletion) {
      actions = TextButton.icon(
        onPressed: hasPending ? onClearPick : onUndo,
        icon: const Icon(Icons.undo, size: 16),
        label: const Text('Undo'),
        style: TextButton.styleFrom(
          foregroundColor: AppTheme.primaryBlue,
          padding: const EdgeInsets.symmetric(horizontal: 8),
        ),
      );
    } else if (hasFile) {
      actions = Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          IconButton(
            onPressed: onDelete,
            icon: const Icon(Icons.delete_outline, size: 20),
            color: AppTheme.errorRed,
            tooltip: 'Remove',
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
          ),
          IconButton(
            onPressed: onPickFile,
            icon: const Icon(Icons.swap_horiz, size: 20),
            color: AppTheme.primaryBlue,
            tooltip: 'Replace',
            padding: EdgeInsets.zero,
            constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
          ),
        ],
      );
    } else {
      actions = IconButton(
        onPressed: onPickFile,
        icon: const Icon(Icons.upload_file, size: 20),
        color: AppTheme.primaryBlue,
        tooltip: 'Upload',
        padding: EdgeInsets.zero,
        constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
      );
    }

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 4),
      child: Row(
        children: [
          const Icon(Icons.attach_file_outlined, size: 18, color: Colors.grey),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: const TextStyle(
                    fontSize: 12,
                    color: Colors.grey,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 2),
                Text(displayName, style: nameStyle),
                if (statusText != null)
                  Text(
                    statusText,
                    style: TextStyle(
                      fontSize: 11,
                      color: hasPending
                          ? AppTheme.primaryBlue
                          : AppTheme.errorRed,
                    ),
                  ),
              ],
            ),
          ),
          actions,
        ],
      ),
    );
  }
}
