import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/city.dart';
import '../../../../core/theme/app_theme.dart';

class LocationSelector extends ConsumerStatefulWidget {
  final Country? selectedCountry;
  final Province? selectedProvince;
  final City? selectedCity;
  final Function(Country?) onCountryChanged;
  final Function(Province?) onProvinceChanged;
  final Function(City?) onCityChanged;
  final String? countryError;
  final String? provinceError;
  final String? cityError;

  const LocationSelector({
    super.key,
    this.selectedCountry,
    this.selectedProvince,
    this.selectedCity,
    required this.onCountryChanged,
    required this.onProvinceChanged,
    required this.onCityChanged,
    this.countryError,
    this.provinceError,
    this.cityError,
  });

  @override
  ConsumerState<LocationSelector> createState() => _LocationSelectorState();
}

class _LocationSelectorState extends ConsumerState<LocationSelector> {
  @override
  Widget build(BuildContext context) {
    final countriesAsync = ref.watch(countriesListProvider);
    final countries = countriesAsync.value ?? [];

    // Get provinces for selected country
    final provincesAsync = widget.selectedCountry != null
        ? ref.watch(provincesProvider(widget.selectedCountry!.id ?? ''))
        : null;
    final provinces = provincesAsync?.value ?? [];

    // Get cities for selected province
    final citiesAsync = widget.selectedProvince != null
        ? ref.watch(citiesProvider(widget.selectedProvince!.id ?? ''))
        : null;
    final cities = citiesAsync?.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        // Country Dropdown
        _buildDropdown(
          label: 'Country',
          value: widget.selectedCountry,
          items: countries,
          onChanged: (catalogItem) {
            if (catalogItem != null) {
              final country = Country(
                id: catalogItem.id,
                value: catalogItem.value,
                code: null,
              );
              widget.onCountryChanged(country);
              // Reset province and city when country changes
              widget.onProvinceChanged(null);
              widget.onCityChanged(null);
            } else {
              widget.onCountryChanged(null);
              widget.onProvinceChanged(null);
              widget.onCityChanged(null);
            }
          },
          errorText: widget.countryError,
          isLoading: countriesAsync.isLoading,
        ),
        const SizedBox(height: 16),

        // Province/State Dropdown
        _buildDropdown(
          label: 'Province/State',
          value: widget.selectedProvince,
          items: provinces,
          onChanged: (catalogItem) {
            if (catalogItem != null && widget.selectedCountry != null) {
              final province = Province(
                id: catalogItem.id,
                value: catalogItem.value,
                code: null,
                country: widget.selectedCountry!,
              );
              widget.onProvinceChanged(province);
              // Reset city when province changes
              widget.onCityChanged(null);
            } else {
              widget.onProvinceChanged(null);
              widget.onCityChanged(null);
            }
          },
          errorText: widget.provinceError,
          isLoading: provincesAsync?.isLoading ?? false,
          enabled: widget.selectedCountry != null,
        ),
        const SizedBox(height: 16),

        // City Dropdown
        _buildDropdown(
          label: 'City',
          value: widget.selectedCity,
          items: cities,
          onChanged: (catalogItem) {
            if (catalogItem != null && widget.selectedProvince != null) {
              final city = City(
                id: catalogItem.id,
                value: catalogItem.value,
                code: null,
                province: widget.selectedProvince!,
              );
              widget.onCityChanged(city);
            } else {
              widget.onCityChanged(null);
            }
          },
          errorText: widget.cityError,
          isLoading: citiesAsync?.isLoading ?? false,
          enabled: widget.selectedProvince != null,
        ),
      ],
    );
  }

  Widget _buildDropdown({
    required String label,
    required dynamic value,
    required List items,
    required Function(dynamic) onChanged,
    String? errorText,
    bool isLoading = false,
    bool enabled = true,
  }) {
    // Find the matching catalog item based on the entity's ID
    String? selectedId;
    if (value != null) {
      if (value is Country) {
        selectedId = value.id;
      } else if (value is Province) {
        selectedId = value.id;
      } else if (value is City) {
        selectedId = value.id;
      }
    }

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
          initialValue: selectedId,
          decoration: InputDecoration(
            filled: true,
            fillColor: enabled ? Colors.white : Colors.grey.shade100,
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
              borderSide: const BorderSide(
                color: AppTheme.primaryBlue,
                width: 2,
              ),
            ),
            errorBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: const BorderSide(color: Colors.red),
            ),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 12,
            ),
            errorText: errorText,
            suffixIcon: isLoading
                ? const Padding(
                    padding: EdgeInsets.all(12.0),
                    child: SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    ),
                  )
                : null,
          ),
          items: items
              .map(
                (item) => DropdownMenuItem<String>(
                  value: item.id,
                  child: Text(item.value),
                ),
              )
              .toList(),
          onChanged: enabled && !isLoading
              ? (selectedItemId) {
                  if (selectedItemId != null) {
                    final selectedItem = items.firstWhere(
                      (item) => item.id == selectedItemId,
                    );
                    onChanged(selectedItem);
                  }
                }
              : null,
          hint: Text(
            enabled
                ? 'Select $label'
                : 'Select ${label.split('/')[0].toLowerCase()} first',
            style: TextStyle(color: Colors.grey.shade600),
          ),
        ),
      ],
    );
  }
}
