import 'package:flutter/material.dart';
import '../../../../core/widgets/cards/profile_section_card.dart';

class CommentsSectionCard extends StatelessWidget {
  const CommentsSectionCard({super.key});

  @override
  Widget build(BuildContext context) {
    // No endpoint available yet. Wire datasource → repository → use case →
    // FutureProvider here when the API is ready.
    return ProfileSectionCard(
      title: 'Comments',
      icon: Icons.rate_review_outlined,
      iconGradient: const [Color(0xFF00695C), Color(0xFF4DB6AC)],
      children: [
        Padding(
          padding: const EdgeInsets.symmetric(vertical: 24),
          child: Center(
            child: Column(
              children: [
                Icon(Icons.comment_outlined,
                    size: 40, color: Colors.grey.shade300),
                const SizedBox(height: 10),
                Text(
                  'No comments yet',
                  style:
                      TextStyle(fontSize: 14, color: Colors.grey.shade500),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}
