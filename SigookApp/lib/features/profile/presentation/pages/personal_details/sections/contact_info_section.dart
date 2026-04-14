import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../../../core/utils/phone_formatter.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../contact_info/presentation/viewmodels/contact_info_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../../../registration/domain/entities/city.dart';
import '../../../../../registration/domain/entities/country.dart';
import '../../../../../registration/domain/entities/province.dart';
import '../../../../../registration/presentation/widgets/location_selector.dart';
import '../../../widgets/section_edit_actions.dart';

class ContactInfoSectionCard extends ConsumerStatefulWidget {
  const ContactInfoSectionCard({super.key});

  @override
  ConsumerState<ContactInfoSectionCard> createState() =>
      _ContactInfoSectionCardState();
}

class _ContactInfoSectionCardState
    extends ConsumerState<ContactInfoSectionCard> {
  final _mobileMaskFormatter = MaskTextInputFormatter(
    mask: '### ###-####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _phoneMaskFormatter = MaskTextInputFormatter(
    mask: '### ###-####',
    filter: {'#': RegExp(r'[0-9]')},
    type: MaskAutoCompletionType.lazy,
  );
  final _mobileNumberController = TextEditingController();
  final _phoneController = TextEditingController();
  final _addressController = TextEditingController();
  final _postalCodeController = TextEditingController();
  Country? _editCountry;
  Province? _editProvince;
  City? _editCity;

  @override
  void dispose() {
    _mobileNumberController.dispose();
    _phoneController.dispose();
    _addressController.dispose();
    _postalCodeController.dispose();
    super.dispose();
  }

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    _mobileNumberController.text =
        _mobileMaskFormatter.maskText(profile?.mobileNumber ?? '');
    _phoneController.text =
        _phoneMaskFormatter.maskText(profile?.phone ?? '');
    _addressController.text = profile?.address ?? '';
    _postalCodeController.text = profile?.postalCode ?? '';
    setState(() {
      _editCountry = null;
      _editProvince = null;
      _editCity = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(contactInfoViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      contactInfoViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen<ContactInfoState>(contactInfoViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    return ProfileSectionCard(
      title: 'Contact Information',
      icon: Icons.contact_phone_outlined,
      iconGradient: const [Color(0xFF4CAF50), Color(0xFF81C784)],
      trailing: SectionEditActions(
        isEditingThis: vm.isEditing,
        isAnyEditing: false,
        isSaving: vm.isSaving,
        onEdit: profile != null
            ? ref.read(contactInfoViewModelProvider.notifier).startEditing
            : null,
        onCancel:
            ref.read(contactInfoViewModelProvider.notifier).cancelEditing,
        onSave: () => ref.read(contactInfoViewModelProvider.notifier).save({
          'mobileNumber': _mobileNumberController.text,
          'phone': _phoneController.text,
          'address': _addressController.text,
          'postalCode': _postalCodeController.text,
          if (_editCity?.id != null) 'cityId': _editCity!.id!,
        }),
      ),
      children: [
        ProfileInfoRow(
          label: 'Mobile Number',
          value: formatPhone(profile?.mobileNumber),
          icon: Icons.phone_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _mobileNumberController : null,
          inputFormatters: vm.isEditing ? [_mobileMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Phone',
          value: formatPhone(profile?.phone),
          icon: Icons.phone_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _phoneController : null,
          inputFormatters: vm.isEditing ? [_phoneMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Email',
          value: profile?.email ?? 'N/A',
          icon: Icons.email_outlined,
        ),
        if (!vm.isEditing) ...[
          ProfileInfoRow(
            label: 'Country',
            value: profile?.country ?? 'N/A',
            icon: Icons.flag_outlined,
          ),
          ProfileInfoRow(
            label: 'State / Province',
            value: profile?.province ?? 'N/A',
            icon: Icons.map_outlined,
          ),
          ProfileInfoRow(
            label: 'City',
            value: profile?.city ?? 'N/A',
            icon: Icons.location_city_outlined,
          ),
        ],
        if (vm.isEditing) ...[
          const SizedBox(height: 4),
          Text(
            'Location',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade600,
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(height: 8),
          LocationSelector(
            selectedCountry: _editCountry,
            selectedProvince: _editProvince,
            selectedCity: _editCity,
            onCountryChanged: (c) => setState(() {
              _editCountry = c;
              _editProvince = null;
              _editCity = null;
            }),
            onProvinceChanged: (p) => setState(() {
              _editProvince = p;
              _editCity = null;
            }),
            onCityChanged: (c) => setState(() => _editCity = c),
          ),
          const SizedBox(height: 8),
        ],
        ProfileInfoRow(
          label: 'Address',
          value: profile?.address ?? 'N/A',
          icon: Icons.home_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _addressController : null,
        ),
        ProfileInfoRow(
          label: 'Postal / ZIP Code',
          value: profile?.postalCode ?? 'N/A',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _postalCodeController : null,
        ),
      ],
    );
  }
}
