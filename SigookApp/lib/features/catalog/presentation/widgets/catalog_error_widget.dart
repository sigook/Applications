import 'package:flutter/material.dart';
import '../../../../core/widgets/error_state_widget.dart';

class CatalogErrorWidget extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const CatalogErrorWidget({
    super.key,
    required this.message,
    required this.onRetry,
  });

  @override
  Widget build(BuildContext context) {
    return ErrorStateWidget(
      title: 'Connection Error',
      message: message,
      onRetry: onRetry,
      icon: Icons.wifi_off_rounded,
    );
  }
}
