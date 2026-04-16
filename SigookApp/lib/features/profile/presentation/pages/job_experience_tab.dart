import 'package:flutter/material.dart';
import 'personal_details/sections/job_experience_section.dart';

class WorkExperienceTab extends StatelessWidget {
  const WorkExperienceTab({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        JobExperienceSectionCard(),
        const SizedBox(height: 24),
      ],
    );
  }
}
