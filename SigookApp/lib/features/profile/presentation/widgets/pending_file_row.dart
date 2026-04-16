import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

/// Shows a pending file (picked but not yet uploaded) with name and hint.
class PendingFileRow extends StatelessWidget {
  final String fileName;
  final String hint;

  const PendingFileRow({
    super.key,
    required this.fileName,
    this.hint = 'Tap upload to save',
  });

  @override
  Widget build(BuildContext context) {
    return Row(
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
                fileName,
                style: const TextStyle(
                  fontSize: 14,
                  color: AppTheme.primaryBlue,
                ),
              ),
              Text(
                hint,
                style: const TextStyle(
                  fontSize: 11,
                  color: AppTheme.primaryBlue,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
