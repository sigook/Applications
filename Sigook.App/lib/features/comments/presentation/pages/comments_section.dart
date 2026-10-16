import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/widgets/cards/profile_section_card.dart';
import '../providers/comments_providers.dart';
import '../widgets/comment_card.dart';

class CommentsSectionCard extends ConsumerWidget {
  const CommentsSectionCard({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final commentsAsync = ref.watch(commentsListProvider);

    return ProfileSectionCard(
      title: 'Comments',
      icon: Icons.rate_review_outlined,
      iconGradient: const [Color(0xFF00695C), Color(0xFF4DB6AC)],
      children: [
        commentsAsync.when(
          data: (comments) {
            if (comments.isEmpty) {
              return Padding(
                padding: const EdgeInsets.symmetric(vertical: 24),
                child: Center(
                  child: Column(
                    children: [
                      Icon(Icons.comment_outlined,
                          size: 40, color: Colors.grey.shade300),
                      const SizedBox(height: 10),
                      Text(
                        'No comments yet',
                        style: TextStyle(
                            fontSize: 14, color: Colors.grey.shade500),
                      ),
                    ],
                  ),
                ),
              );
            }
            return Column(
              children: comments
                  .map((c) => CommentCard(comment: c))
                  .toList(),
            );
          },
          loading: () => const Padding(
            padding: EdgeInsets.symmetric(vertical: 24),
            child: Center(child: CircularProgressIndicator()),
          ),
          error: (_, _) => Padding(
            padding: const EdgeInsets.symmetric(vertical: 16),
            child: Row(
              children: [
                Icon(Icons.error_outline, color: Colors.red.shade400, size: 18),
                const SizedBox(width: 8),
                Text(
                  'Failed to load comments',
                  style: TextStyle(fontSize: 13, color: Colors.red.shade400),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}
