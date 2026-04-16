import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/utils/phone_formatter.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../emergency/presentation/viewmodels/emergency_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/section_edit_actions.dart';

class EmergencySectionCard extends ConsumerStatefulWidget {
  const EmergencySectionCard({super.key});

  @override
  ConsumerState<EmergencySectionCard> createState() =>
      _EmergencySectionCardState();
}

class _EmergencySectionCardState extends ConsumerState<EmergencySectionCard> {
  final _emergencyPhoneMaskFormatter = MaskTextInputFormatter(
    mask: '### ###-####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _nameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _phoneController = TextEditingController();

  @override
  void dispose() {
    _nameController.dispose();
    _lastNameController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    _nameController.text = profile?.contactEmergencyName ?? '';
    _lastNameController.text = profile?.contactEmergencyLastName ?? '';
    _phoneController.text = _emergencyPhoneMaskFormatter.maskText(
      profile?.contactEmergencyPhone ?? '',
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(emergencyViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      emergencyViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen<EmergencyState>(emergencyViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    return ProfileSectionCard(
      title: 'Emergency Information',
      icon: Icons.emergency_outlined,
      iconGradient: const [Color(0xFFF44336), Color(0xFFE57373)],
      trailing: SectionEditActions(
        isEditingThis: vm.isEditing,
        isAnyEditing: false,
        isSaving: vm.isSaving,
        onEdit: profile != null
            ? ref.read(emergencyViewModelProvider.notifier).startEditing
            : null,
        onCancel: ref.read(emergencyViewModelProvider.notifier).cancelEditing,
        onSave: () =>
            ref.read(emergencyViewModelProvider.notifier).save({
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
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _nameController : null,
        ),
        if (vm.isEditing)
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
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _phoneController : null,
          inputFormatters: vm.isEditing ? [_emergencyPhoneMaskFormatter] : null,
        ),
      ],
    );
  }
}
