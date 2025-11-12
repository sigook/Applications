import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/address_info.dart';
import '../providers/registration_providers.dart';
import '../widgets/country_autocomplete_field.dart';
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
  String? _selectedProvinceId;
  String? _selectedProvinceName;
  String? _selectedCityId;
  String? _selectedCityName;

  String? _countryError;
  String? _provinceStateError;
  String? _cityError;
  String? _addressError;

  @override
  void initState() {
    super.initState();
    _addressController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.addressInfo != null) {
        _selectedCountryName = form.addressInfo!.country;
        _selectedProvinceName = form.addressInfo!.provinceState;
        _selectedCityName = form.addressInfo!.city;
        _addressController.text = form.addressInfo!.address;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _addressController.dispose();
    super.dispose();
  }

  void _onCountryChanged(CatalogItem? country) {
    setState(() {
      _selectedCountryId = country?.id;
      _selectedCountryName = country?.value;

      // Clear province and city when country changes
      _selectedProvinceId = null;
      _selectedProvinceName = null;
      _selectedCityId = null;
      _selectedCityName = null;
    });
    _validateAndSave();
  }

  void _onProvinceChanged(CatalogItem? province) {
    setState(() {
      _selectedProvinceId = province?.id;
      _selectedProvinceName = province?.value;

      // Clear city when province changes
      _selectedCityId = null;
      _selectedCityName = null;
    });
    _validateAndSave();
  }

  void _onCityChanged(CatalogItem? city) {
    setState(() {
      _selectedCityId = city?.id;
      _selectedCityName = city?.value;
    });
    _validateAndSave();
  }

  void _validateAndSave() {
    setState(() {
      final addressInfo = AddressInfo(
        country: _selectedCountryName ?? '',
        provinceState: _selectedProvinceName ?? '',
        city: _selectedCityName ?? '',
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
              if (_selectedCountryId != null) _buildProvinceDropdown(),
              if (_selectedCountryId != null) const SizedBox(height: 24),
              if (_selectedProvinceId != null) _buildCityDropdown(),
              if (_selectedProvinceId != null) const SizedBox(height: 24),
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

  Widget _buildProvinceDropdown() {
    if (_selectedCountryId == null) return const SizedBox.shrink();

    final provincesAsync = ref.watch(provincesProvider(_selectedCountryId!));

    return provincesAsync.when(
      data: (provinces) {
        if (provinces.isEmpty) {
          return Text(
            'No provinces available for this country',
            style: TextStyle(color: Colors.grey.shade600),
          );
        }

        return _buildCatalogDropdown(
          label: 'Province / State',
          hint: 'Select province/state...',
          selectedValue: _selectedProvinceName,
          items: provinces,
          onChanged: _onProvinceChanged,
          errorText: _provinceStateError,
          icon: Icons.map,
        );
      },
      loading: () => const LinearProgressIndicator(),
      error: (error, stack) => Text(
        'Error loading provinces: ${error.toString()}',
        style: const TextStyle(color: Colors.red),
      ),
    );
  }

  Widget _buildCityDropdown() {
    if (_selectedProvinceId == null) return const SizedBox.shrink();

    final citiesAsync = ref.watch(citiesProvider(_selectedProvinceId!));

    return citiesAsync.when(
      data: (cities) {
        if (cities.isEmpty) {
          return Text(
            'No cities available for this province',
            style: TextStyle(color: Colors.grey.shade600),
          );
        }

        return _buildCatalogDropdown(
          label: 'City',
          hint: 'Select city...',
          selectedValue: _selectedCityName,
          items: cities,
          onChanged: _onCityChanged,
          errorText: _cityError,
          icon: Icons.location_city,
        );
      },
      loading: () => const LinearProgressIndicator(),
      error: (error, stack) => Text(
        'Error loading cities: ${error.toString()}',
        style: const TextStyle(color: Colors.red),
      ),
    );
  }

  Widget _buildCatalogDropdown({
    required String label,
    required String hint,
    required String? selectedValue,
    required List<CatalogItem> items,
    required Function(CatalogItem?) onChanged,
    String? errorText,
    IconData? icon,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: Color(0xFF1E293B),
          ),
        ),
        const SizedBox(height: 8),
        DropdownButtonFormField<String>(
          isExpanded: true,
          initialValue: selectedValue,
          decoration: InputDecoration(
            hintText: hint,
            prefixIcon: icon != null ? Icon(icon) : null,
            errorText: errorText,
            filled: true,
            fillColor: Colors.grey.shade50,
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Color(0xFF0EA5E9), width: 2),
            ),
          ),
          items: items.map((item) {
            return DropdownMenuItem<String>(
              value: item.value,
              child: Text(
                item.value,
                overflow: TextOverflow.ellipsis,
                maxLines: 1,
                softWrap: false,
              ),
            );
          }).toList(),
          onChanged: (value) {
            final selectedItem = items.firstWhere(
              (item) => item.value == value,
            );
            onChanged(selectedItem);
          },
        ),
      ],
    );
  }
}
