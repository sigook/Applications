import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/city.dart';
import 'searchable_dropdown_field.dart';

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

    final provincesAsync = widget.selectedCountry != null
        ? ref.watch(provincesProvider(widget.selectedCountry!.id ?? ''))
        : null;
    final provinces = provincesAsync?.value ?? [];

    final citiesAsync = widget.selectedProvince != null
        ? ref.watch(citiesProvider(widget.selectedProvince!.id ?? ''))
        : null;
    final cities = citiesAsync?.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
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
          enabled: true,
        ),
        const SizedBox(height: 16),

        // Province/State - searchable dropdown, enabled only when country selected
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

        // City - searchable dropdown, enabled only when province selected
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
    // Derive the selected display label and option labels from items
    String? selectedLabel;
    if (value != null) {
      if (value is Country) {
        selectedLabel = value.value;
      } else if (value is Province) {
        selectedLabel = value.value;
      } else if (value is City) {
        selectedLabel = value.value;
      }
    }

    // When disabled, show a non-interactive dropdown-style field with a hint
    if (!enabled) {
      // More explicit dependency messages
      String dependencyLabel;
      if (label == 'Province/State') {
        dependencyLabel = 'country';
      } else if (label == 'City') {
        dependencyLabel = 'province';
      } else {
        dependencyLabel = label.toLowerCase();
      }

      final hintText = 'Select $dependencyLabel first';

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
          TextField(
            enabled: false,
            decoration: InputDecoration(
              hintText: hintText,
              prefixIcon: const Icon(Icons.place),
              filled: true,
              fillColor: Colors.grey.shade100,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
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

    final optionLabels = items.map((item) => item.value as String).toList();

    return SearchableDropdownField(
      label: label,
      hint: 'Select or search $label...',
      selectedValue: selectedLabel,
      options: optionLabels,
      onChanged: (selectedText) {
        if (selectedText == null) {
          onChanged(null);
          return;
        }

        // Manual search to avoid type issues with firstWhere's orElse
        dynamic selectedItem;
        for (final item in items) {
          if (item.value == selectedText) {
            selectedItem = item;
            break;
          }
        }

        if (selectedItem != null) {
          onChanged(selectedItem);
        }
      },
      errorText: errorText,
      icon: Icons.place,
      isLoading: isLoading,
    );
  }
}
