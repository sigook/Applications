import 'package:flutter/material.dart';
import '../../../domain/entities/worker_profile.dart';

/// Displays a single [WorkerCertificate] as a tappable card.
/// Tapping calls [onPreview] when a file URL is available.
class CertificateCard extends StatelessWidget {
  final WorkerCertificate certificate;
  final VoidCallback? onPreview;

  const CertificateCard({
    super.key,
    required this.certificate,
    this.onPreview,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: certificate.fileUrl != null ? onPreview : null,
        borderRadius: BorderRadius.circular(12),
        child: Container(
          padding: const EdgeInsets.all(12),
          decoration: BoxDecoration(
            color: Colors.orange.shade50,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: Colors.orange.shade200),
          ),
          child: Row(
            children: [
              Icon(
                Icons.workspace_premium_outlined,
                size: 18,
                color: Colors.orange.shade700,
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  certificate.description ?? 'Certificate',
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: Colors.orange.shade700,
                  ),
                ),
              ),
              if (certificate.fileUrl != null)
                Icon(
                  Icons.visibility_outlined,
                  size: 18,
                  color: Colors.orange.shade400,
                ),
            ],
          ),
        ),
      ),
    );
  }
}
