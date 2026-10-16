import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';

class HeroSection extends StatelessWidget {
  const HeroSection({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.white,
      padding: const EdgeInsets.symmetric(vertical: 32, horizontal: 24),
      child: Column(
        children: [
          Image.asset(
            'assets/images/logo/sigook-logo.png',
            width: 160,
          ),
          const SizedBox(height: 16),
          Text(
            'SIGOOK\u00AE Work Factory',
            style: AppTheme.heading2.copyWith(
              color: AppTheme.primaryBlue,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            'Connecting workers with opportunities',
            style: AppTheme.bodyMedium.copyWith(color: AppTheme.textLight),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
