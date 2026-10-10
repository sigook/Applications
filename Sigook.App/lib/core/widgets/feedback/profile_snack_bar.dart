import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

void showProfileSuccess(BuildContext context, String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: AppTheme.successGreen,
      behavior: SnackBarBehavior.floating,
    ),
  );
}

void showProfileError(BuildContext context, String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(
      content: Text(message),
      backgroundColor: AppTheme.errorRed,
      behavior: SnackBarBehavior.floating,
    ),
  );
}
