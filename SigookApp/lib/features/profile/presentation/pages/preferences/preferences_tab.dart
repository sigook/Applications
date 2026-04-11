import 'package:flutter/material.dart';
import '../../widgets/profile_action_buttons.dart';
import 'sections/emergency_section.dart';
import 'sections/preferences_section.dart';

class PreferencesTab extends StatelessWidget {
  final VoidCallback onLogout;
  final String appVersion;

  const PreferencesTab({
    super.key,
    required this.onLogout,
    required this.appVersion,
  });

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        const PreferencesSectionCard(),
        const SizedBox(height: 12),
        const EmergencySectionCard(),
        const SizedBox(height: 12),
        ProfileActionButtons(onLogout: onLogout),
        const SizedBox(height: 16),
        if (appVersion.isNotEmpty)
          Padding(
            padding: const EdgeInsets.only(bottom: 24),
            child: Center(
              child: Text(
                appVersion,
                style: TextStyle(fontSize: 12, color: Colors.grey.shade500),
              ),
            ),
          ),
      ],
    );
  }
}
