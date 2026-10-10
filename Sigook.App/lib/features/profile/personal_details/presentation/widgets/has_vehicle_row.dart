import 'package:flutter/material.dart';
import '../../../../../core/theme/app_theme.dart';

class HasVehicleRow extends StatelessWidget {
  final bool value;
  final ValueChanged<bool> onChanged;

  const HasVehicleRow({
    super.key,
    required this.value,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        children: [
          const Icon(
            Icons.directions_car_outlined,
            size: 20,
            color: AppTheme.primaryBlue,
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Do you have your own vehicle?',
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 4),
                Row(
                  children: [
                    Switch(
                      value: value,
                      onChanged: onChanged,
                      activeThumbColor: AppTheme.primaryBlue,
                    ),
                    Text(
                      value ? 'Yes' : 'No',
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
