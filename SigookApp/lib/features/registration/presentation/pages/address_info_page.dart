import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/location_data.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../domain/entities/address_info.dart';
import '../providers/registration_providers.dart';
import '../widgets/country_autocomplete_field.dart';
import '../widgets/searchable_dropdown_field.dart';
import '../widgets/custom_text_field.dart';

class AddressInfoPage extends ConsumerStatefulWidget {
  const AddressInfoPage({super.key});

  @override
  ConsumerState<AddressInfoPage> createState() => _AddressInfoPageState();
}

class _AddressInfoPageState extends ConsumerState<AddressInfoPage> {
  late TextEditingController _addressController;

  String? _selectedCountryId;
  String? _selectedCountryName;
  String? _selectedProvinceState;
  String? _selectedCity;

  String? _countryError;
  String? _provinceStateError;
  String? _cityError;
  String? _addressError;

  List<String> _availableProvinces = LocationData.allProvincesStates;
  List<String> _availableCities = LocationData.allCities;

  @override
  void initState() {
    super.initState();
    _addressController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.addressInfo != null) {
        _selectedCountryName = form.addressInfo!.country;
        _selectedProvinceState = form.addressInfo!.provinceState;
        _selectedCity = form.addressInfo!.city;
        _addressController.text = form.addressInfo!.address;

        // Update available provinces/cities based on country
        if (_selectedCountryName != null) {
          _updateLocationLists(_selectedCountryName!);
        }

        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _addressController.dispose();
    super.dispose();
  }

  void _updateLocationLists(String country) {
    setState(() {
      _availableProvinces = LocationData.getProvincesForCountry(country);
      _availableCities = LocationData.getCitiesForCountry(country);
    });
  }

  void _onCountryChanged(CatalogItem? country) {
    setState(() {
      _selectedCountryId = country?.id;
      _selectedCountryName = country?.value;

      // Update provinces and cities based on new country
      if (country != null) {
        _updateLocationLists(country.value);
      }

      // Clear province and city when country changes
      _selectedProvinceState = null;
      _selectedCity = null;
    });
    _validateAndSave();
  }

  void _validateAndSave() {
    setState(() {
      final addressInfo = AddressInfo(
        country: _selectedCountryName ?? '',
        provinceState: _selectedProvinceState ?? '',
        city: _selectedCity ?? '',
        address: _addressController.text,
      );

      _countryError = addressInfo.countryError;
      _provinceStateError = addressInfo.provinceStateError;
      _cityError = addressInfo.cityError;
      _addressError = addressInfo.addressError;

      if (addressInfo.isValid) {
        ref
            .read(registrationViewModelProvider.notifier)
            .updateAddressInfo(addressInfo);
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
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),
              CountryAutocompleteField(
                selectedCountryId: _selectedCountryId,
                selectedCountryName: _selectedCountryName,
                onChanged: _onCountryChanged,
                errorText: _countryError,
              ),
              const SizedBox(height: 24),
              SearchableDropdownField(
                label: 'Province / State',
                hint: 'Select or search province/state...',
                selectedValue: _selectedProvinceState,
                options: _availableProvinces,
                onChanged: (value) {
                  setState(() {
                    _selectedProvinceState = value;
                    // Clear city when province changes
                    _selectedCity = null;
                  });
                  _validateAndSave();
                },
                errorText: _provinceStateError,
                icon: Icons.map,
              ),
              const SizedBox(height: 24),
              SearchableDropdownField(
                label: 'City',
                hint: 'Select or search city...',
                selectedValue: _selectedCity,
                options: _availableCities,
                onChanged: (value) {
                  setState(() {
                    _selectedCity = value;
                  });
                  _validateAndSave();
                },
                errorText: _cityError,
                icon: Icons.location_city,
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
