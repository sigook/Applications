import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class ActivityStatusBadge extends StatelessWidget {
  final String status;

  const ActivityStatusBadge({super.key, required this.status});

  Color get _color => switch (status) {
        'Active' || 'Open' => AppTheme.primaryBlue,
        'Completed' || 'Approved' || 'Paid' => AppTheme.successGreen,
        'Pending' => AppTheme.warningOrange,
        'No clock-out' => AppTheme.errorRed,
        _ => AppTheme.textMedium,
      };

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 9, vertical: 3),
      decoration: BoxDecoration(
        color: _color.withValues(alpha: 0.12),
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        status,
        style: TextStyle(
          fontSize: 11,
          fontWeight: FontWeight.w600,
          color: _color,
        ),
      ),
    );
  }
}
