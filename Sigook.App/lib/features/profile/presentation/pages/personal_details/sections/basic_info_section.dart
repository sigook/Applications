import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/inputs/date_picker_field.dart';
import '../../../../../catalog/presentation/providers/catalog_providers.dart';
import '../../../../domain/validators/profile_validators.dart';
import '../../../../preferences/presentation/widgets/chip_selector.dart';
import '../../../../personal_details/presentation/viewmodels/personal_details_viewmodel.dart';
import '../../../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../../../../personal_details/presentation/widgets/has_vehicle_row.dart';
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
  DateTime? _birthDay;
  String? _genderId;
  Map<String, String?> _errors = {};

  bool _validate() {
    setState(
      () => _errors = {
        'birthDay': ProfileValidators.birthDay(_birthDay),
        'gender': ProfileValidators.gender(_genderId),
        'firstName': ProfileValidators.firstName(_firstNameController.text),
        'middleName': ProfileValidators.optionalName(
          _middleNameController.text,
          'Middle name',
        ),
        'lastName': ProfileValidators.lastName(_lastNameController.text),
        'secondLastName': ProfileValidators.optionalName(
          _secondLastNameController.text,
          'Second last name',
        ),
      },
    );
    return _errors.values.every((e) => e == null);
  }

  Future<void> _pickBirthDay() async {
    final now = DateTime.now();
    final latest = DateTime(
      now.year - ProfileValidators.minimumAge,
      now.month,
      now.day,
    );
    final picked = await showDatePicker(
      context: context,
      initialDate: _birthDay != null && !_birthDay!.isAfter(latest)
          ? _birthDay!
          : latest,
      firstDate: DateTime(1900),
      lastDate: latest,
      builder: (context, child) => Theme(
        data: Theme.of(context).copyWith(
          colorScheme: const ColorScheme.light(primary: AppTheme.primaryBlue),
        ),
        child: child!,
      ),
    );
    if (picked == null) return;
    setState(() {
      _birthDay = picked;
      _errors = {..._errors, 'birthDay': null};
    });
  }

  void _save() {
    if (!_validate()) return;
    final genders = ref.read(gendersProvider).asData?.value ?? [];
    final gender = genders.where((g) => g.id == _genderId).firstOrNull;
    ref.read(personalDetailsViewModelProvider.notifier).save({
      'birthDay': DateTime(
        _birthDay!.year,
        _birthDay!.month,
        _birthDay!.day,
      ).toIso8601String(),
      'genderId': _genderId!,
      if (gender != null) 'genderValue': gender.value,
      'firstName': _firstNameController.text.trim(),
      'middleName': _middleNameController.text.trim(),
      'lastName': _lastNameController.text.trim(),
      'secondLastName': _secondLastNameController.text.trim(),
      'hasVehicle': _hasVehicle.toString(),
    });
  }

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
    setState(() {
      _hasVehicle = profile?.hasVehicle ?? false;
      _birthDay = profile?.birthDay != null
          ? DateTime.tryParse(profile!.birthDay!)
          : null;
      _genderId = profile?.genderId;
      _errors = {};
    });
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(personalDetailsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(personalDetailsViewModelProvider.select((s) => s.isEditing), (
      prev,
      next,
    ) {
      if (prev == false && next == true) _populateFields();
    });

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
        onCancel: ref
            .read(personalDetailsViewModelProvider.notifier)
            .cancelEditing,
        onSave: _save,
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
            errorText: _errors['firstName'],
          ),
          ProfileInfoRow(
            label: 'Middle Name',
            value: profile?.middleName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _middleNameController,
            errorText: _errors['middleName'],
          ),
          ProfileInfoRow(
            label: 'Last Name',
            value: profile?.lastName ?? 'N/A',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _lastNameController,
            errorText: _errors['lastName'],
          ),
          ProfileInfoRow(
            label: 'Second Last Name',
            value: profile?.secondLastName ?? '',
            icon: Icons.badge_outlined,
            isEditing: true,
            controller: _secondLastNameController,
            errorText: _errors['secondLastName'],
          ),
        ],
        if (!vm.isEditing) ...[
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
        ] else ...[
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: DatePickerField(
              label: 'Date of Birth *',
              value: _birthDay,
              onTap: _pickBirthDay,
              errorText: _errors['birthDay'],
            ),
          ),
          ChipSelector(
            label: 'Gender *',
            icon: Icons.wc_outlined,
            asyncValue: ref.watch(gendersProvider),
            selectedIds: _genderId != null ? {_genderId!} : {},
            singleSelect: true,
            onToggle: (id, selected) => setState(() {
              _genderId = selected ? id : null;
              _errors = {..._errors, 'gender': null};
            }),
          ),
          if (_errors['gender'] != null)
            Padding(
              padding: const EdgeInsets.only(top: 4),
              child: Text(
                _errors['gender']!,
                style: const TextStyle(color: Colors.red, fontSize: 12),
              ),
            ),
          const SizedBox(height: 12),
        ],
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
