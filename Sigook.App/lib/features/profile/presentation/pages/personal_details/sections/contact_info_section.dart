import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../../../core/utils/phone_formatter.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../contact_info/presentation/viewmodels/contact_info_viewmodel.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/validators/profile_validators.dart';
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
  Map<String, String?> _errors = {};

  bool _validate() {
    final countryCode = _editCountry?.code ?? _editCountry?.value;
    setState(() => _errors = {
      'mobileNumber': ProfileValidators.phone(
          _mobileNumberController.text, 'Mobile number',
          required: true),
      'phone': ProfileValidators.phone(_phoneController.text, 'Phone'),
      'address': ProfileValidators.address(_addressController.text),
      'postalCode': ProfileValidators.postalCode(
          _postalCodeController.text, countryCode),
      'country': _editCountry == null ? 'Country is required' : null,
      'province': _editCountry != null && _editProvince == null
          ? 'Province/State is required'
          : null,
      'city': _editProvince != null && _editCity == null
          ? 'City is required'
          : null,
    });
    return _errors.values.every((e) => e == null);
  }

  void _save() {
    if (!_validate()) return;
    ref.read(contactInfoViewModelProvider.notifier).save({
      'mobileNumber': _mobileNumberController.text,
      'phone': _phoneController.text,
      'address': _addressController.text.trim(),
      'postalCode': _postalCodeController.text.trim(),
      if (_editCity?.id != null) 'cityId': _editCity!.id!,
    });
  }

  void _populateLocation(WorkerProfile? profile) {
    _editCountry = null;
    _editProvince = null;
    _editCity = null;
    if (profile?.countryId == null || profile?.country == null) return;
    _editCountry = Country(
      id: profile!.countryId,
      value: profile.country!,
      code: profile.countryCode,
    );
    if (profile.provinceId == null || profile.province == null) return;
    _editProvince = Province(
      id: profile.provinceId,
      value: profile.province!,
      country: _editCountry!,
    );
    if (profile.cityId == null || profile.city == null) return;
    _editCity = City(
      id: profile.cityId,
      value: profile.city!,
      province: _editProvince!,
    );
  }

  Widget _locationRow(LocationField field, Widget control) {
    final (label, icon) = switch (field) {
      LocationField.country => ('Country *', Icons.flag_outlined),
      LocationField.province => ('State / Province *', Icons.map_outlined),
      LocationField.city => ('City *', Icons.location_city_outlined),
    };
    return ProfileInfoRow(
      label: label,
      value: '',
      icon: icon,
      isEditing: true,
      child: control,
    );
  }

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
      _populateLocation(profile);
      _errors = {};
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
        onSave: _save,
      ),
      children: [
        ProfileInfoRow(
          label: 'Mobile Number',
          value: formatPhone(profile?.mobileNumber),
          icon: Icons.phone_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _mobileNumberController : null,
          inputFormatters: vm.isEditing ? [_mobileMaskFormatter] : null,
          errorText: _errors['mobileNumber'],
        ),
        ProfileInfoRow(
          label: 'Phone',
          value: formatPhone(profile?.phone),
          icon: Icons.phone_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _phoneController : null,
          inputFormatters: vm.isEditing ? [_phoneMaskFormatter] : null,
          errorText: _errors['phone'],
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
        ] else
          LocationSelector(
            selectedCountry: _editCountry,
            selectedProvince: _editProvince,
            selectedCity: _editCity,
            countryError: _errors['country'],
            provinceError: _errors['province'],
            cityError: _errors['city'],
            rowBuilder: _locationRow,
            onCountryChanged: (c) => setState(() {
              _editCountry = c;
              _editProvince = null;
              _editCity = null;
              _errors = {..._errors, 'country': null};
            }),
            onProvinceChanged: (p) => setState(() {
              _editProvince = p;
              _editCity = null;
              _errors = {..._errors, 'province': null};
            }),
            onCityChanged: (c) => setState(() {
              _editCity = c;
              _errors = {..._errors, 'city': null};
            }),
          ),
        ProfileInfoRow(
          label: 'Address',
          value: profile?.address ?? 'N/A',
          icon: Icons.home_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _addressController : null,
          errorText: _errors['address'],
        ),
        ProfileInfoRow(
          label: 'Postal / ZIP Code',
          value: profile?.postalCode ?? 'N/A',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _postalCodeController : null,
          errorText: _errors['postalCode'],
        ),
      ],
    );
  }
}
