import 'package:flutter/material.dart';

enum ActionButtonType { filled, outlined }

class ActionButton extends StatelessWidget {
  final String label;
  final VoidCallback? onPressed;
  final IconData? icon;
  final ActionButtonType type;
  final Color? backgroundColor;
  final Color? foregroundColor;
  final double height;
  final bool isLoading;

  const ActionButton({
    super.key,
    required this.label,
    this.onPressed,
    this.icon,
    this.type = ActionButtonType.filled,
    this.backgroundColor,
    this.foregroundColor,
    this.height = 56,
    this.isLoading = false,
  });

  @override
  Widget build(BuildContext context) {
    final content = Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        if (icon != null && !isLoading) ...[
          Icon(icon, size: 22),
          const SizedBox(width: 12),
        ],
        if (isLoading)
          const SizedBox(
            width: 20,
            height: 20,
            child: CircularProgressIndicator(strokeWidth: 2),
          )
        else
          Text(
            label,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              letterSpacing: 0.5,
            ),
          ),
      ],
    );

    return SizedBox(
      width: double.infinity,
      height: height,
      child: type == ActionButtonType.filled
          ? ElevatedButton(
              onPressed: isLoading ? null : onPressed,
              style: ElevatedButton.styleFrom(
                backgroundColor: backgroundColor,
                foregroundColor: foregroundColor,
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: content,
            )
          : OutlinedButton(
              onPressed: isLoading ? null : onPressed,
              style: OutlinedButton.styleFrom(
                foregroundColor: foregroundColor ?? backgroundColor,
                side: BorderSide(
                  color: foregroundColor ?? backgroundColor ?? Colors.grey,
                  width: 2,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: content,
            ),
    );
  }
}
