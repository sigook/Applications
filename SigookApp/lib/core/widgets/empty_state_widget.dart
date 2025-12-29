import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class EmptyStateWidget extends StatelessWidget {
  final String message;
  final String? title;
  final IconData icon;
  final VoidCallback? onAction;
  final String? actionLabel;

  const EmptyStateWidget({
    super.key,
    required this.message,
    this.title,
    this.icon = Icons.inbox_outlined,
    this.onAction,
    this.actionLabel,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: EdgeInsets.all(AppTheme.spacing24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              size: AppTheme.spacing64 + AppTheme.spacing16,
              color: AppTheme.textLight,
            ),
            SizedBox(height: AppTheme.spacing16),
            if (title != null) ...[
              Text(
                title!,
                style: AppTheme.heading3,
                textAlign: TextAlign.center,
              ),
              SizedBox(height: AppTheme.spacing8),
            ],
            Text(
              message,
              style: AppTheme.bodyMedium.copyWith(color: AppTheme.textLight),
              textAlign: TextAlign.center,
            ),
            if (onAction != null && actionLabel != null) ...[
              SizedBox(height: AppTheme.spacing24),
              ElevatedButton(onPressed: onAction, child: Text(actionLabel!)),
            ],
          ],
        ),
      ),
    );
  }
}
