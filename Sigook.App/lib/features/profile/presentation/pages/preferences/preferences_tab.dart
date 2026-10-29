import 'package:flutter/material.dart';
import 'sections/emergency_section.dart';
import 'sections/preferences_section.dart';

class PreferencesTab extends StatelessWidget {
  const PreferencesTab({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: const [
        SizedBox(height: 16),
        PreferencesSectionCard(),
        SizedBox(height: 12),
        EmergencySectionCard(),
        SizedBox(height: 24),
      ],
    );
  }
}
