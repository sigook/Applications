import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/cards/custom_card.dart';
import 'about_constants.dart';
import 'category_row.dart';

class JobCategoriesSection extends StatelessWidget {
  const JobCategoriesSection({super.key});

  @override
  Widget build(BuildContext context) {
    return CustomCard(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Job Categories',
            style: AppTheme.heading3.copyWith(
              color: AppTheme.textDark,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          ...List.generate(kAboutCategories.length, (index) {
            final cat = kAboutCategories[index];
            return Column(
              children: [
                CategoryRow(item: cat),
                if (index < kAboutCategories.length - 1)
                  Divider(
                    height: 24,
                    color: Colors.grey.shade100,
                  ),
              ],
            );
          }),
        ],
      ),
    );
  }
}
