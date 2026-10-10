import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

/// Upload button + cancel icon row, reused for Resume, License, and Certificate.
class UploadActionRow extends StatelessWidget {
  final bool isUploading;
  final String label;
  final VoidCallback onUpload;
  final VoidCallback onCancel;

  const UploadActionRow({
    super.key,
    required this.isUploading,
    required this.label,
    required this.onUpload,
    required this.onCancel,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Expanded(
          child: ElevatedButton.icon(
            onPressed: isUploading ? null : onUpload,
            icon: isUploading
                ? const SizedBox(
                    width: 16,
                    height: 16,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: Colors.white,
                    ),
                  )
                : const Icon(Icons.cloud_upload_outlined, size: 18),
            label: Text(isUploading ? 'Uploading...' : label),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.primaryBlue,
              foregroundColor: Colors.white,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              padding: const EdgeInsets.symmetric(vertical: 12),
            ),
          ),
        ),
        const SizedBox(width: 8),
        IconButton(
          onPressed: isUploading ? null : onCancel,
          icon: Icon(
            Icons.cancel_outlined,
            color: Colors.grey.shade500,
            size: 22,
          ),
          tooltip: 'Cancel',
          padding: EdgeInsets.zero,
          constraints: const BoxConstraints(minWidth: 32, minHeight: 32),
        ),
      ],
    );
  }
}
