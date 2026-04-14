import 'package:flutter/material.dart';
import '../../../domain/entities/worker_profile.dart';

/// Displays a single [WorkerLicense] as a tappable card.
/// Tapping calls [onPreview] when a file URL is available.
class LicenseCard extends StatelessWidget {
  final WorkerLicense license;
  final VoidCallback? onPreview;

  const LicenseCard({
    super.key,
    required this.license,
    this.onPreview,
  });

  @override
  Widget build(BuildContext context) {
    final isExpired = license.isExpired;
    final color =
        isExpired ? Colors.red.shade700 : Colors.green.shade700;
    final bgColor =
        isExpired ? Colors.red.shade50 : Colors.green.shade50;
    final borderColor =
        isExpired ? Colors.red.shade200 : Colors.green.shade200;

    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: license.fileUrl != null ? onPreview : null,
        borderRadius: BorderRadius.circular(12),
        child: Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: bgColor,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: borderColor),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(Icons.card_membership_outlined, size: 18, color: color),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      license.description ?? 'License',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: color,
                      ),
                    ),
                  ),
                  if (isExpired)
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 2),
                      decoration: BoxDecoration(
                        color: Colors.red.shade100,
                        borderRadius: BorderRadius.circular(8),
                      ),
                      child: Text(
                        'Expired',
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w600,
                          color: Colors.red.shade700,
                        ),
                      ),
                    ),
                  if (license.fileUrl != null)
                    Icon(
                      Icons.visibility_outlined,
                      size: 18,
                      color: isExpired
                          ? Colors.red.shade400
                          : Colors.green.shade400,
                    ),
                ],
              ),
              if (license.number != null && license.number!.isNotEmpty) ...[
                const SizedBox(height: 8),
                Row(
                  children: [
                    Icon(Icons.numbers_outlined,
                        size: 16, color: Colors.grey.shade600),
                    const SizedBox(width: 6),
                    Text(
                      'No. ${license.number}',
                      style: TextStyle(
                          fontSize: 13, color: Colors.grey.shade700),
                    ),
                  ],
                ),
              ],
              const SizedBox(height: 4),
              Row(
                children: [
                  Icon(Icons.calendar_today_outlined,
                      size: 16, color: Colors.grey.shade600),
                  const SizedBox(width: 6),
                  Expanded(
                    child: Text(
                      'Issued: ${license.formattedIssued}  ·  Expires: ${license.formattedExpires}',
                      style: TextStyle(
                          fontSize: 12, color: Colors.grey.shade600),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
