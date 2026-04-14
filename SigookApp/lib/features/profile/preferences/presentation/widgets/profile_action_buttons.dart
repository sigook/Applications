import 'package:flutter/material.dart';
import '../../../../../core/theme/app_theme.dart';

/// Logout and Delete Account buttons shown at the bottom of the profile.
class ProfileActionButtons extends StatelessWidget {
  final VoidCallback onLogout;
  final VoidCallback? onDeleteAccount;

  const ProfileActionButtons({
    super.key,
    required this.onLogout,
    this.onDeleteAccount,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: onLogout,
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.secondaryRed,
                side: const BorderSide(color: AppTheme.secondaryRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.logout, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Logout',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 56,
            child: OutlinedButton(
              onPressed: onDeleteAccount,
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.errorRed,
                side: const BorderSide(color: AppTheme.errorRed, width: 2),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: const Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.delete_forever, size: 22),
                  SizedBox(width: 12),
                  Text(
                    'Delete Account',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      letterSpacing: 0.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
