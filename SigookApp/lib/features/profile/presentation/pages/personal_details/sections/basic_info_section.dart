import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../personal_details/presentation/viewmodels/personal_details_viewmodel.dart';
import '../../../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/has_vehicle_row.dart';
import '../../../widgets/section_edit_actions.dart';

class BasicInfoSectionCard extends ConsumerStatefulWidget {
  const BasicInfoSectionCard({super.key});

  @override
  ConsumerState<BasicInfoSectionCard> createState() =>
      _BasicInfoSectionCardState();
}

class _BasicInfoSectionCardState extends ConsumerState<BasicInfoSectionCard> {
  final _firstNameController = TextEditingController();
  final _middleNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _secondLastNameController = TextEditingController();
  late bool _hasVehicle = false;

  @override
  void dispose() {
    _firstNameController.dispose();
    _middleNameController.dispose();
    _lastNameController.dispose();
    _secondLastNameController.dispose();
    super.dispose();
  }

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    _firstNameController.text = profile?.firstName ?? '';
    _middleNameController.text = profile?.middleName ?? '';
    _lastNameController.text = profile?.lastName ?? '';
    _secondLastNameController.text = profile?.secondLastName ?? '';
    setState(() => _hasVehicle = profile?.hasVehicle ?? false);
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(personalDetailsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      personalDetailsViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen(personalDetailsViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    return ProfileSectionCard(
      title: 'Basic Information',
      icon: Icons.person_outline,
      iconGradient: const [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
      trailing: SectionEditActions(
        isEditingThis: vm.isEditing,
        isAnyEditing: false,
        isSaving: vm.isSaving,
        onEdit: profile != null
            ? ref.read(personalDetailsViewModelProvider.notifier).startEditing
            : null,
        onCancel:
            ref.read(personalDetailsViewModelProvider.notifier).cancelEditing,
        onSave: () =>
            ref.read(personalDetailsViewModelProvider.notifier).save({
              'firstName': _firstNameController.text,
              'middleName': _middleNameController.text,
              'lastName': _lastNameController.text,
              'secondLastName': _secondLastNameController.text,
              'hasVehicle': _hasVehicle.toString(),
            }),
      ),
      children: [
        if (!vm.isEditing)
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
        if (!vm.isEditing)
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
