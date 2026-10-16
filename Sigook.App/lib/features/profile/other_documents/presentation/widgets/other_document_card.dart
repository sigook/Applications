import 'package:flutter/material.dart';
import '../../../domain/entities/worker_profile.dart';

class OtherDocumentCard extends StatelessWidget {
  final WorkerOtherDocument document;
  final VoidCallback? onPreview;
  final VoidCallback? onDelete;

  const OtherDocumentCard({
    super.key,
    required this.document,
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
          color: Colors.teal.shade50,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: Colors.teal.shade200),
        ),
        child: Row(
          children: [
            Icon(Icons.health_and_safety_outlined, size: 18, color: Colors.teal.shade700),
            const SizedBox(width: 8),
            Expanded(
              child: Text(
                document.description?.isNotEmpty == true
                    ? document.description!
                    : 'Document',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                  color: Colors.teal.shade700,
                ),
              ),
            ),
            if (document.fileUrl != null) ...[
              InkWell(
                onTap: onPreview,
                borderRadius: BorderRadius.circular(4),
                child: Icon(Icons.visibility_outlined, size: 18, color: Colors.teal.shade400),
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
