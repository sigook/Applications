import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class ActivityDetailRow extends StatelessWidget {
  final String label;
  final String value;
  final String? hint;
  final bool isBold;

  const ActivityDetailRow({
    super.key,
    required this.label,
    required this.value,
    this.hint,
    this.isBold = false,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(label, style: const TextStyle(fontSize: 13, color: AppTheme.textMedium)),
                if (hint != null && hint!.isNotEmpty)
                  Text(hint!, style: const TextStyle(fontSize: 12, color: AppTheme.textMedium)),
              ],
            ),
          ),
          const SizedBox(width: 8),
          Text(
            value,
            style: TextStyle(
              fontSize: 14,
              color: AppTheme.textDark,
              fontWeight: isBold ? FontWeight.w600 : FontWeight.normal,
            ),
          ),
        ],
      ),
    );
  }
}
