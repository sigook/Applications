import 'package:flutter/material.dart';
import 'package:mask_text_input_formatter/mask_text_input_formatter.dart';
import '../../../../../../core/utils/phone_formatter.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../../../registration/domain/entities/city.dart';
import '../../../../../registration/domain/entities/country.dart';
import '../../../../../registration/domain/entities/province.dart';
import '../../../../../registration/presentation/widgets/location_selector.dart';
import '../../../widgets/section_edit_actions.dart';

class ContactInfoSectionCard extends StatefulWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final VoidCallback? onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields,
  ) onSave;
  final VoidCallback onCancel;

  const ContactInfoSectionCard({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
  });

  @override
  State<ContactInfoSectionCard> createState() => _ContactInfoSectionCardState();
}

class _ContactInfoSectionCardState extends State<ContactInfoSectionCard> {
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

  bool get _isEditing => widget.editingSection == ProfileSection.contact;

  @override
  void didUpdateWidget(ContactInfoSectionCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    final wasEditing = oldWidget.editingSection == ProfileSection.contact;
    if (!wasEditing && _isEditing) {
      _mobileNumberController.text =
          _mobileMaskFormatter.maskText(widget.profile?.mobileNumber ?? '');
      _phoneController.text =
          _phoneMaskFormatter.maskText(widget.profile?.phone ?? '');
      _addressController.text = widget.profile?.address ?? '';
      _postalCodeController.text = widget.profile?.postalCode ?? '';
      _editCountry = null;
      _editProvince = null;
      _editCity = null;
    }
  }

  @override
  void dispose() {
    _mobileNumberController.dispose();
    _phoneController.dispose();
    _addressController.dispose();
    _postalCodeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

    return ProfileSectionCard(
      title: 'Contact Information',
      icon: Icons.contact_phone_outlined,
      iconGradient: const [Color(0xFF4CAF50), Color(0xFF81C784)],
      trailing: SectionEditActions(
        isEditingThis: _isEditing,
        isAnyEditing: widget.editingSection != null,
        isSaving: widget.isSaving,
        onEdit: widget.onEdit,
        onCancel: widget.onCancel,
        onSave: () => widget.onSave(ProfileSection.contact, {
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
          isEditing: _isEditing,
          controller: _isEditing ? _mobileNumberController : null,
          inputFormatters: _isEditing ? [_mobileMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Phone',
          value: formatPhone(profile?.phone),
          icon: Icons.phone_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _phoneController : null,
          inputFormatters: _isEditing ? [_phoneMaskFormatter] : null,
        ),
        ProfileInfoRow(
          label: 'Email',
          value: profile?.email ?? 'N/A',
          icon: Icons.email_outlined,
        ),
        if (!_isEditing) ...[
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
        if (_isEditing) ...[
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
          isEditing: _isEditing,
          controller: _isEditing ? _addressController : null,
        ),
        ProfileInfoRow(
          label: 'Postal / ZIP Code',
          value: profile?.postalCode ?? 'N/A',
          icon: Icons.markunread_mailbox_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _postalCodeController : null,
        ),
      ],
    );
  }
}
