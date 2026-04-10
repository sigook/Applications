import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../widgets/has_vehicle_row.dart';
import '../../../widgets/section_edit_actions.dart';

class BasicInfoSectionCard extends StatefulWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final VoidCallback? onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields,
  ) onSave;
  final VoidCallback onCancel;

  const BasicInfoSectionCard({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
  });

  @override
  State<BasicInfoSectionCard> createState() => _BasicInfoSectionCardState();
}

class _BasicInfoSectionCardState extends State<BasicInfoSectionCard> {
  final _firstNameController = TextEditingController();
  final _middleNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _secondLastNameController = TextEditingController();
  late bool _hasVehicle;

  bool get _isEditing => widget.editingSection == ProfileSection.personal;

  @override
  void initState() {
    super.initState();
    _hasVehicle = widget.profile?.hasVehicle ?? false;
  }

  @override
  void didUpdateWidget(BasicInfoSectionCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    final wasEditing = oldWidget.editingSection == ProfileSection.personal;
    if (!wasEditing && _isEditing) {
      _firstNameController.text = widget.profile?.firstName ?? '';
      _middleNameController.text = widget.profile?.middleName ?? '';
      _lastNameController.text = widget.profile?.lastName ?? '';
      _secondLastNameController.text = widget.profile?.secondLastName ?? '';
      _hasVehicle = widget.profile?.hasVehicle ?? false;
    }
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _middleNameController.dispose();
    _lastNameController.dispose();
    _secondLastNameController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

    return ProfileSectionCard(
      title: 'Basic Information',
      icon: Icons.person_outline,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
      trailing: SectionEditActions(
        isEditingThis: _isEditing,
        isAnyEditing: widget.editingSection != null,
        isSaving: widget.isSaving,
        onEdit: widget.onEdit,
        onCancel: widget.onCancel,
        onSave: () => widget.onSave(ProfileSection.personal, {
          'firstName': _firstNameController.text,
          'middleName': _middleNameController.text,
          'lastName': _lastNameController.text,
          'secondLastName': _secondLastNameController.text,
          'hasVehicle': _hasVehicle.toString(),
        }),
      ),
      children: [
        if (!_isEditing)
          ProfileInfoRow(
            label: 'Full Name',
            value: profile?.fullName.isNotEmpty == true
                ? profile!.fullName
                : 'N/A',
            icon: Icons.badge_outlined,
          )
        else ...[
          ProfileInfoRow(
            label: 'First Name',
            value: profile?.firstName ?? 'N/A',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _firstNameController,
          ),
          ProfileInfoRow(
            label: 'Middle Name',
            value: profile?.middleName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _middleNameController,
          ),
          ProfileInfoRow(
            label: 'Last Name',
            value: profile?.lastName ?? 'N/A',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _lastNameController,
          ),
          ProfileInfoRow(
            label: 'Second Last Name',
            value: profile?.secondLastName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _secondLastNameController,
          ),
        ],
        ProfileInfoRow(
          label: 'Date of Birth',
          value: profile?.formattedBirthDay ?? 'N/A',
          icon: Icons.cake_outlined,
        ),
        ProfileInfoRow(
          label: 'Gender',
          value: profile?.gender ?? 'N/A',
          icon: Icons.wc_outlined,
        ),
        if (!_isEditing)
          ProfileInfoRow(
            label: 'Do you have your own vehicle?',
            value: profile?.hasVehicle == true ? 'Yes' : 'No',
            icon: Icons.directions_car_outlined,
          )
        else
          HasVehicleRow(
            value: _hasVehicle,
            onChanged: (v) => setState(() => _hasVehicle = v),
          ),
      ],
    );
  }
}
