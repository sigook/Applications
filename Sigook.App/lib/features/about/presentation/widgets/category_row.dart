import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/display/gradient_icon_container.dart';
import 'about_constants.dart';

class CategoryRow extends StatelessWidget {
  final CategoryItem item;

  const CategoryRow({super.key, required this.item});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        GradientIconContainer(
          icon: item.icon,
          gradientColors: [AppTheme.primaryBlue, AppTheme.primaryBlue],
          size: 40,
          iconSize: 20,
          borderRadius: 10,
        ),
        const SizedBox(width: 14),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                item.title,
                style: AppTheme.bodyMedium.copyWith(
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textDark,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                item.subtitle,
                style: AppTheme.bodySmall.copyWith(color: AppTheme.textLight),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
