import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class FilePickerButton extends StatelessWidget {
  final String? fileName;
  final VoidCallback onTap;
  final bool isLoading;
  final String emptyLabel;
  final String fileLabel;

  const FilePickerButton({
    super.key,
    this.fileName,
    required this.onTap,
    this.isLoading = false,
    this.emptyLabel = 'Choose File',
    this.fileLabel = 'Selected File',
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: isLoading ? null : onTap,
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: Colors.grey.shade50,
          border: Border.all(color: Colors.grey.shade300),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Row(
          children: [
            Container(
              padding: const EdgeInsets.all(8),
              decoration: BoxDecoration(
                color: AppTheme.primaryBlue.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Icon(
                fileName != null ? Icons.insert_drive_file : Icons.upload_file,
                color: AppTheme.primaryBlue,
                size: 24,
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    fileName != null ? fileLabel : emptyLabel,
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                  ),
                  if (fileName != null) ...[
                    const SizedBox(height: 4),
                    Text(
                      fileName!,
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ],
                ],
              ),
            ),
            if (isLoading)
              const SizedBox(
                width: 24,
                height: 24,
                child: CircularProgressIndicator(strokeWidth: 2),
              )
            else
              Icon(
                Icons.arrow_forward_ios,
                size: 16,
                color: Colors.grey.shade400,
              ),
          ],
        ),
      ),
    );
  }
}
