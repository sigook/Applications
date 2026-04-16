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
                _buildAvatar(),
                const SizedBox(width: 10),
                Expanded(child: _buildRatingStars()),
                Text(
                  comment.formattedDate,
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                ),
              ],
            ),
            const SizedBox(height: 10),
            Text(
              comment.comment,
              style: TextStyle(
                fontSize: 13,
                color: Colors.grey.shade700,
                height: 1.45,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAvatar() {
    if (comment.logo != null && comment.logo!.isNotEmpty) {
      return CircleAvatar(
        radius: 16,
        backgroundImage: NetworkImage(comment.logo!),
        backgroundColor: Colors.blueGrey.shade100,
      );
    }
    return CircleAvatar(
      radius: 16,
      backgroundColor: Colors.blueGrey.shade100,
      child: Text(
        '#${comment.numberId}',
        style: TextStyle(
          fontSize: 9,
          fontWeight: FontWeight.bold,
          color: Colors.blueGrey.shade700,
        ),
      ),
    );
  }

  Widget _buildRatingStars() {
    final fullStars = comment.rate.floor();
    final hasHalf = (comment.rate - fullStars) >= 0.5;
    return Row(
      children: List.generate(5, (i) {
        if (i < fullStars) {
          return const Icon(Icons.star, size: 14, color: Color(0xFFFFC107));
        } else if (i == fullStars && hasHalf) {
          return const Icon(Icons.star_half, size: 14, color: Color(0xFFFFC107));
        } else {
          return Icon(Icons.star_border, size: 14, color: Colors.grey.shade300);
        }
      }),
    );
  }
}
