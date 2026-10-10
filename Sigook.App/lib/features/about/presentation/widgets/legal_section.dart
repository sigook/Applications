import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';

class LegalSection extends StatelessWidget {
  const LegalSection({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          const Divider(height: 1),
          const SizedBox(height: 16),
          Text(
            'Legal',
            style: AppTheme.bodySmall.copyWith(
              color: AppTheme.textLight,
              fontWeight: FontWeight.w600,
              letterSpacing: 1.2,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: _LegalButton(
                  icon: Icons.shield_outlined,
                  label: 'Privacy Policy',
                  onPressed: () => context.push(AppRoutes.privacyPolicy),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _LegalButton(
                  icon: Icons.gavel_rounded,
                  label: 'Terms & Conditions',
                  onPressed: () => context.push(AppRoutes.terms),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            '\u00A9 ${DateTime.now().year} SIGOOK\u00AE Work Factory. All rights reserved.',
            style: AppTheme.bodySmall.copyWith(color: AppTheme.textLight),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}

class _LegalButton extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onPressed;

  const _LegalButton({
    required this.icon,
    required this.label,
    required this.onPressed,
  });

  @override
  Widget build(BuildContext context) {
    return OutlinedButton.icon(
      onPressed: onPressed,
      icon: Icon(icon, size: 16),
      label: Text(label),
      style: OutlinedButton.styleFrom(
        foregroundColor: AppTheme.primaryBlue,
        side: BorderSide(
          color: AppTheme.primaryBlue.withValues(alpha: 0.4),
        ),
        padding: const EdgeInsets.symmetric(vertical: 12),
        textStyle: AppTheme.bodySmall.copyWith(
          fontWeight: FontWeight.w600,
        ),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
        ),
      ),
    );
  }
}
