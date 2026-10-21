import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/display/profile_info_row.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/city.dart';
import 'searchable_dropdown_field.dart';

enum LocationField { country, province, city }

/// Country → Province → City cascade. By default each field renders its own
/// header label (registration look). When [rowBuilder] is given the fields
/// render dense and the caller decides how to frame each one.
class LocationSelector extends ConsumerWidget {
  final Country? selectedCountry;
  final Province? selectedProvince;
  final City? selectedCity;
  final Function(Country?) onCountryChanged;
  final Function(Province?) onProvinceChanged;
  final Function(City?) onCityChanged;
  final String? countryError;
  final String? provinceError;
  final String? cityError;
  final Widget Function(LocationField field, Widget control)? rowBuilder;

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
    this.rowBuilder,
  });

  bool get _dense => rowBuilder != null;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final countriesAsync = ref.watch(countriesListProvider);
    final provincesAsync = selectedCountry != null
        ? ref.watch(provincesProvider(selectedCountry!.id ?? ''))
        : null;
    final citiesAsync = selectedProvince != null
        ? ref.watch(citiesProvider(selectedProvince!.id ?? ''))
        : null;

    final country = _buildDropdown(
      field: LocationField.country,
      label: 'Country',
      selectedLabel: selectedCountry?.value,
      items: countriesAsync.value ?? [],
      onChanged: (item) {
        onCountryChanged(
          item != null ? Country(id: item.id, value: item.value) : null,
        );
        onProvinceChanged(null);
        onCityChanged(null);
      },
      errorText: countryError,
      isLoading: countriesAsync.isLoading,
      enabled: true,
    );

    final province = _buildDropdown(
      field: LocationField.province,
      label: 'Province/State',
      selectedLabel: selectedProvince?.value,
      items: provincesAsync?.value ?? [],
      onChanged: (item) {
        onProvinceChanged(
          item != null && selectedCountry != null
              ? Province(
                  id: item.id,
                  value: item.value,
                  country: selectedCountry!,
                )
              : null,
        );
        onCityChanged(null);
      },
      errorText: provinceError,
      isLoading: provincesAsync?.isLoading ?? false,
      enabled: selectedCountry != null,
    );

    final city = _buildDropdown(
      field: LocationField.city,
      label: 'City',
      selectedLabel: selectedCity?.value,
      items: citiesAsync?.value ?? [],
      onChanged: (item) {
        onCityChanged(
          item != null && selectedProvince != null
              ? City(id: item.id, value: item.value, province: selectedProvince!)
              : null,
        );
      },
      errorText: cityError,
      isLoading: citiesAsync?.isLoading ?? false,
      enabled: selectedProvince != null,
    );

    if (rowBuilder != null) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          rowBuilder!(LocationField.country, country),
          rowBuilder!(LocationField.province, province),
          rowBuilder!(LocationField.city, city),
        ],
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        country,
        const SizedBox(height: 16),
        province,
        const SizedBox(height: 16),
        city,
      ],
    );
  }

  Widget _buildDropdown({
    required LocationField field,
    required String label,
    required String? selectedLabel,
    required List<CatalogItem> items,
    required ValueChanged<CatalogItem?> onChanged,
    String? errorText,
    bool isLoading = false,
    bool enabled = true,
  }) {
    if (!enabled) return _buildDisabled(field, label);

    return SearchableDropdownField(
      label: label,
      hint: 'Select or search $label...',
      selectedValue: selectedLabel,
      options: items.map((item) => item.value).toList(),
      isRequired: true,
      dense: _dense,
      onChanged: (selectedText) {
        if (selectedText == null) {
          onChanged(null);
          return;
        }
        final selectedItem =
            items.where((item) => item.value == selectedText).firstOrNull;
        if (selectedItem != null) onChanged(selectedItem);
      },
      errorText: errorText,
      icon: Icons.place,
      isLoading: isLoading,
    );
  }

  Widget _buildDisabled(LocationField field, String label) {
    final dependency = switch (field) {
      LocationField.province => 'country',
      LocationField.city => 'province',
      LocationField.country => label.toLowerCase(),
    };
    final hintText = 'Select $dependency first';

    if (_dense) {
      return TextField(
        enabled: false,
        decoration: ProfileInfoRow.decoration(hintText: hintText),
      );
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          children: [
            Text(
              label,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppTheme.textDark,
              ),
            ),
            const Text(
              ' *',
              style: TextStyle(
                color: Colors.red,
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        TextField(
          enabled: false,
          decoration: InputDecoration(
            hintText: hintText,
            prefixIcon: const Icon(Icons.place),
            filled: true,
            fillColor: Colors.grey.shade100,
            border: OutlineInputBorder(borderRadius: BorderRadius.circular(12)),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            disabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(12),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 12,
            ),
          ),
        ),
      ],
    );
  }
}
