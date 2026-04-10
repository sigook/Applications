import 'package:flutter/material.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/utils/phone_formatter.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../widgets/section_edit_actions.dart';

class EmergencySectionCard extends StatefulWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final VoidCallback? onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields,
  ) onSave;
  final VoidCallback onCancel;

  const EmergencySectionCard({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
  });

  @override
  State<EmergencySectionCard> createState() => _EmergencySectionCardState();
}

class _EmergencySectionCardState extends State<EmergencySectionCard> {
  final _emergencyPhoneMaskFormatter = MaskTextInputFormatter(
    mask: '### ###-####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _nameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _phoneController = TextEditingController();

  bool get _isEditing => widget.editingSection == ProfileSection.emergency;

  @override
  void didUpdateWidget(EmergencySectionCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    final wasEditing = oldWidget.editingSection == ProfileSection.emergency;
    if (!wasEditing && _isEditing) {
      _nameController.text = widget.profile?.contactEmergencyName ?? '';
      _lastNameController.text = widget.profile?.contactEmergencyLastName ?? '';
      _phoneController.text = _emergencyPhoneMaskFormatter.maskText(
        widget.profile?.contactEmergencyPhone ?? '',
      );
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _lastNameController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

    return ProfileSectionCard(
      title: 'Emergency Information',
      icon: Icons.emergency_outlined,
      iconGradient: const [Color(0xFFF44336), Color(0xFFE57373)],
      trailing: SectionEditActions(
        isEditingThis: _isEditing,
        isAnyEditing: widget.editingSection != null,
        isSaving: widget.isSaving,
        onEdit: widget.onEdit,
        onCancel: widget.onCancel,
        onSave: () => widget.onSave(ProfileSection.emergency, {
          'contactEmergencyName': _nameController.text,
          'contactEmergencyLastName': _lastNameController.text,
          'contactEmergencyPhone': _phoneController.text,
        }),
      ),
      children: [
        ProfileInfoRow(
          label: 'Do you have any health problems / allergies?',
          value: profile?.haveAnyHealthProblem == true ? 'Yes' : 'No',
          icon: Icons.health_and_safety_outlined,
        ),
        const Padding(
          padding: EdgeInsets.only(bottom: 8),
          child: Text(
            'In case of emergency notify:',
            style: TextStyle(
              fontSize: 12,
              color: AppTheme.primaryBlue,
              fontStyle: FontStyle.italic,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
        ProfileInfoRow(
          label: 'Name',
          value: profile != null
              ? '${profile.contactEmergencyName ?? ''} ${profile.contactEmergencyLastName ?? ''}'
                    .trim()
              : 'N/A',
          icon: Icons.person_outline,
          isEditing: _isEditing,
          controller: _isEditing ? _nameController : null,
        ),
        if (_isEditing)
          ProfileInfoRow(
            label: 'Last Name',
            value: profile?.contactEmergencyLastName ?? '',
            icon: Icons.person_outline,
            isEditing: true,
            controller: _lastNameController,
          ),
        ProfileInfoRow(
          label: 'Phone',
          value: formatPhone(profile?.contactEmergencyPhone),
          icon: Icons.phone_callback_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _phoneController : null,
          inputFormatters: _isEditing ? [_emergencyPhoneMaskFormatter] : null,
        ),
      ],
    );
  }
}
