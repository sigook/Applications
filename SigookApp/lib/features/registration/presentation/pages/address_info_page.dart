import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/address_info.dart';
import '../providers/registration_providers.dart';
import '../widgets/custom_text_field.dart';

class AddressInfoPage extends ConsumerStatefulWidget {
  const AddressInfoPage({super.key});

  @override
  ConsumerState<AddressInfoPage> createState() => _AddressInfoPageState();
}

class _AddressInfoPageState extends ConsumerState<AddressInfoPage> {
  late TextEditingController _countryController;
  late TextEditingController _provinceStateController;
  late TextEditingController _cityController;
  late TextEditingController _addressController;

  String? _countryError;
  String? _provinceStateError;
  String? _cityError;
  String? _addressError;

  @override
  void initState() {
    super.initState();
    _countryController = TextEditingController();
    _provinceStateController = TextEditingController();
    _cityController = TextEditingController();
    _addressController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.addressInfo != null) {
        _countryController.text = form.addressInfo!.country;
        _provinceStateController.text = form.addressInfo!.provinceState;
        _cityController.text = form.addressInfo!.city;
        _addressController.text = form.addressInfo!.address;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _countryController.dispose();
    _provinceStateController.dispose();
    _cityController.dispose();
    _addressController.dispose();
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      final addressInfo = AddressInfo(
        country: _countryController.text,
        provinceState: _provinceStateController.text,
        city: _cityController.text,
        address: _addressController.text,
      );

      _countryError = addressInfo.countryError;
      _provinceStateError = addressInfo.provinceStateError;
      _cityError = addressInfo.cityError;
      _addressError = addressInfo.addressError;

      if (addressInfo.isValid) {
        ref.read(registrationViewModelProvider.notifier).updateAddressInfo(addressInfo);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;
    
    return SingleChildScrollView(
      padding: EdgeInsets.all(isMobile ? 12 : 16),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Card(
        elevation: 0,
        color: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Address Information',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
          ),
          const SizedBox(height: 8),
          Text(
            'Please provide your address details',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.grey.shade600,
                ),
          ),
          const SizedBox(height: 32),
          CustomTextField(
            label: 'Country',
            hint: 'Enter your country',
            initialValue: _countryController.text,
            errorText: _countryError,
            onChanged: (value) {
              _countryController.text = value;
              _validateAndSave();
            },
          ),
          const SizedBox(height: 24),
          CustomTextField(
            label: 'Province / State',
            hint: 'Enter your province or state',
            initialValue: _provinceStateController.text,
            errorText: _provinceStateError,
            onChanged: (value) {
              _provinceStateController.text = value;
              _validateAndSave();
            },
          ),
          const SizedBox(height: 24),
          CustomTextField(
            label: 'City',
            hint: 'Enter your city',
            initialValue: _cityController.text,
            errorText: _cityError,
            onChanged: (value) {
              _cityController.text = value;
              _validateAndSave();
            },
          ),
          const SizedBox(height: 24),
          CustomTextField(
            label: 'Address',
            hint: 'Enter your street address',
            initialValue: _addressController.text,
            errorText: _addressError,
            maxLines: 2,
            onChanged: (value) {
              _addressController.text = value;
              _validateAndSave();
            },
          ),
        ],
      ),
        ),
      ),
    );
  }
}
