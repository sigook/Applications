import 'package:flutter/material.dart';
import '../../../domain/entities/worker_profile.dart';
import '../../../domain/usecases/update_worker_profile.dart';
import '../../widgets/profile_action_buttons.dart';
import 'sections/emergency_section.dart';
import 'sections/preferences_section.dart';

class PreferencesTab extends StatelessWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final void Function(ProfileSection section) onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields,
  ) onSave;
  final VoidCallback onCancel;
  final VoidCallback onLogout;
  final String appVersion;

  const PreferencesTab({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
    required this.onLogout,
    required this.appVersion,
  });

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        PreferencesSectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.preferences)
              : null,
          onSave: onSave,
          onCancel: onCancel,
        ),
        const SizedBox(height: 12),
        EmergencySectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.emergency)
              : null,
          onSave: onSave,
          onCancel: onCancel,
        ),
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
