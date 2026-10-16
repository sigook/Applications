import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

Future<bool> showLogoutConfirmationDialog(BuildContext context) async {
  final shouldLogout = await showDialog<bool>(
    context: context,
    builder: (context) => AlertDialog(
      title: const Text('Logout'),
      content: const Text('Are you sure you want to logout?'),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: const Text('Cancel'),
        ),
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(true),
          style: ElevatedButton.styleFrom(
            backgroundColor: AppTheme.secondaryRed,
          ),
          child: const Text('Logout'),
        ),
      ],
    ),
  );
  return shouldLogout == true;
}
