import 'package:flutter/material.dart';
import '../../../comments/presentation/pages/comments_section.dart';

class CommentsTab extends StatelessWidget {
  const CommentsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        const CommentsSectionCard(),
        const SizedBox(height: 24),
      ],
    );
  }
}
