import 'package:flutter/material.dart';
import '../../config/environment.dart';
import '../../theme/app_theme.dart';

class EnvironmentBanner extends StatelessWidget {
  final Widget child;

  const EnvironmentBanner({super.key, required this.child});

  @override
  Widget build(BuildContext context) {
    if (EnvironmentConfig.isProduction) return child;

    final apiHost = Uri.tryParse(EnvironmentConfig.apiBaseUrl)?.host ?? '';

    return Column(
      children: [
        Container(
          width: double.infinity,
          color: AppTheme.warningOrange,
          child: SafeArea(
            bottom: false,
            child: Padding(
              padding: const EdgeInsets.symmetric(vertical: 4, horizontal: 16),
              child: Text(
                '${EnvironmentConfig.environmentName.toUpperCase()} · $apiHost',
                textAlign: TextAlign.center,
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                  letterSpacing: 0.5,
                ),
              ),
            ),
          ),
        ),
        Expanded(
          child: MediaQuery.removePadding(
            context: context,
            removeTop: true,
            child: child,
          ),
        ),
      ],
    );
  }
}
