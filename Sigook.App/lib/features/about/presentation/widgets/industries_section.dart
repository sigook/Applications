import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/cards/custom_card.dart';
import 'about_constants.dart';
import 'industry_chip.dart';

class IndustriesSection extends StatelessWidget {
  const IndustriesSection({super.key});

  @override
  Widget build(BuildContext context) {
    return CustomCard(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Industries We Serve',
            style: AppTheme.heading3.copyWith(
              color: AppTheme.textDark,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: kAboutIndustries
                .map((i) => IndustryChip(item: i))
                .toList(),
          ),
        ],
      ),
    );
  }
}
