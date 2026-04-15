import 'package:flutter/material.dart';
import '../../domain/entities/worker_comment.dart';

class CommentCard extends StatelessWidget {
  final WorkerComment comment;

  const CommentCard({super.key, required this.comment});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: Colors.grey.shade200),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.04),
              blurRadius: 6,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 16,
                  backgroundColor: Colors.blueGrey.shade100,
                  child: Text(
                    comment.authorName.isNotEmpty
                        ? comment.authorName[0].toUpperCase()
                        : '?',
                    style: TextStyle(
                      fontSize: 13,
                      fontWeight: FontWeight.bold,
                      color: Colors.blueGrey.shade700,
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Text(
                    comment.authorName,
                    style: const TextStyle(
                        fontSize: 14, fontWeight: FontWeight.w600),
                  ),
                ),
                if (comment.createdAt != null)
                  Text(
                    comment.createdAt!,
                    style: TextStyle(
                        fontSize: 11, color: Colors.grey.shade500),
                  ),
              ],
            ),
            const SizedBox(height: 10),
            Text(
              comment.content,
              style: TextStyle(
                  fontSize: 13, color: Colors.grey.shade700, height: 1.45),
            ),
          ],
        ),
      ),
    );
  }
}
