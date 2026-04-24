import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

/// Displays a labeled row with an icon and a list of read-only chips.
/// Used in profile sections to show multi-value fields (availability, skills, etc.).
class ChipDisplayRow extends StatelessWidget {
  final String label;
  final IconData icon;
  final List<String> chips;

  const ChipDisplayRow({
    super.key,
    required this.label,
    required this.icon,
    required this.chips,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: AppTheme.primaryBlue),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    fontSize: 12,
                    color: Colors.grey.shade600,
                    fontWeight: FontWeight.w500,
                  ),
                ),
                const SizedBox(height: 6),
                chips.isEmpty
                    ? const Text(
                        'N/A',
                        style: TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: AppTheme.textDark,
                        ),
                      )
                    : Wrap(
                        spacing: 6,
                        runSpacing: 4,
                        children: chips
                            .map(
                              (c) => Chip(
                                label: Text(
                                  c,
                                  style: const TextStyle(
                                    fontSize: 12,
                                    color: AppTheme.primaryBlue,
                                    fontWeight: FontWeight.w500,
                                  ),
                                ),
                                backgroundColor:
                                    AppTheme.primaryBlue.withValues(alpha: 0.1),
                                side: BorderSide(
                                  color: AppTheme.primaryBlue
                                      .withValues(alpha: 0.3),
                                ),
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 4,
                                  vertical: 0,
                                ),
                                materialTapTargetSize:
                                    MaterialTapTargetSize.shrinkWrap,
                                visualDensity: VisualDensity.compact,
                              ),
                            )
                            .toList(),
                      ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
