import 'package:flutter/material.dart';

class GradientIconContainer extends StatelessWidget {
  final IconData icon;
  final List<Color> gradientColors;
  final double size;
  final double iconSize;
  final double borderRadius;
  final bool withShadow;

  const GradientIconContainer({
    super.key,
    required this.icon,
    required this.gradientColors,
    this.size = 48,
    this.iconSize = 24,
    this.borderRadius = 12,
    this.withShadow = false,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
          colors: gradientColors,
        ),
        borderRadius: BorderRadius.circular(borderRadius),
        boxShadow: withShadow
            ? [
                BoxShadow(
                  color: gradientColors.first.withValues(alpha: 0.3),
                  blurRadius: 8,
                  offset: const Offset(0, 2),
                ),
              ]
            : null,
      ),
      child: Icon(icon, color: Colors.white, size: iconSize),
    );
  }
}
