import 'package:flutter/material.dart';
import '../theme/app_theme.dart';

class ErrorStateWidget extends StatelessWidget {
  final String message;
  final String? title;
  final VoidCallback? onRetry;
  final IconData icon;

  const ErrorStateWidget({
    super.key,
    required this.message,
    this.title,
    this.onRetry,
    this.icon = Icons.error_outline,
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
            Icon(icon, size: AppTheme.spacing64, color: AppTheme.errorRed),
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
            if (onRetry != null) ...[
              SizedBox(height: AppTheme.spacing24),
              ElevatedButton.icon(
                onPressed: onRetry,
                icon: const Icon(Icons.refresh),
                label: const Text('Retry'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  padding: EdgeInsets.symmetric(
                    horizontal: AppTheme.spacing24,
                    vertical: AppTheme.spacing12,
                  ),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
