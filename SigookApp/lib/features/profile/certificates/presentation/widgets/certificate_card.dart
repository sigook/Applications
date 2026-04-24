import 'package:flutter/material.dart';
import '../../../domain/entities/worker_profile.dart';

class CertificateCard extends StatelessWidget {
  final WorkerCertificate certificate;
  final VoidCallback? onPreview;
  final VoidCallback? onDelete;

  const CertificateCard({
    super.key,
    required this.certificate,
    this.onPreview,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: Colors.orange.shade50,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: Colors.orange.shade200),
        ),
        child: Row(
          children: [
            Icon(Icons.workspace_premium_outlined, size: 18, color: Colors.orange.shade700),
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
            if (certificate.fileUrl != null) ...[
              InkWell(
                onTap: onPreview,
                borderRadius: BorderRadius.circular(4),
                child: Icon(Icons.visibility_outlined, size: 18, color: Colors.orange.shade400),
              ),
              const SizedBox(width: 8),
            ],
            if (onDelete != null)
              InkWell(
                onTap: onDelete,
                borderRadius: BorderRadius.circular(4),
                child: Icon(Icons.delete_outline, size: 18, color: Colors.red.shade400),
              ),
          ],
        ),
      ),
    );
  }
}
