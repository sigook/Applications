import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/cards/profile_section_card.dart';

class AboutSection extends StatelessWidget {
  const AboutSection({super.key});

  @override
  Widget build(BuildContext context) {
    return ProfileSectionCard(
      title: 'Who We Are',
      icon: Icons.info_outline_rounded,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.primaryBlue],
      children: [
        Text(
          'We are SIGOOK\u00AE Work Factory, a staffing agency in Florida '
          'dedicated to connecting skilled workers with the right job '
          'opportunities. We specialize in temporary, permanent, and '
          'contract placements across multiple industries.',
          style: AppTheme.bodyMedium.copyWith(
            color: AppTheme.textMedium,
            height: 1.6,
          ),
        ),
        const SizedBox(height: 12),
        Text(
          'Our mission is to simplify hiring for businesses and make '
          'finding meaningful work easier for every worker \u2014 whether '
          "you're looking for your first job or your next career move.",
          style: AppTheme.bodyMedium.copyWith(
            color: AppTheme.textMedium,
            height: 1.6,
          ),
        ),
      ],
    );
  }
}
