import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/cards/custom_card.dart';
import '../../../../core/widgets/display/gradient_icon_container.dart';
import 'about_constants.dart';

class ServiceCard extends StatelessWidget {
  final ServiceItem service;

  const ServiceCard({super.key, required this.service});

  @override
  Widget build(BuildContext context) {
    return CustomCard(
      margin: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          GradientIconContainer(
            icon: service.icon,
            gradientColors: [service.color, service.color],
            size: 38,
            iconSize: 22,
            borderRadius: 8,
          ),
          const SizedBox(height: 12),
          Text(
            service.title,
            style: AppTheme.bodyMedium.copyWith(
              fontWeight: FontWeight.bold,
              color: AppTheme.textDark,
            ),
          ),
          const SizedBox(height: 4),
          Expanded(
            child: Text(
              service.description,
              style: AppTheme.bodySmall.copyWith(
                color: AppTheme.textLight,
                height: 1.4,
              ),
              maxLines: 3,
              overflow: TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }
}
