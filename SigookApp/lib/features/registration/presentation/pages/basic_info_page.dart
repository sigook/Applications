import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/value_objects/name.dart';
import '../providers/registration_providers.dart';
import '../widgets/country_autocomplete_field.dart';
import '../widgets/custom_text_field.dart';

class BasicInfoPage extends ConsumerStatefulWidget {
  const BasicInfoPage({super.key});

  @override
  ConsumerState<BasicInfoPage> createState() => _BasicInfoPageState();
}

class _BasicInfoPageState extends ConsumerState<BasicInfoPage> {
  late TextEditingController _firstNameController;
  late TextEditingController _lastNameController;
  late TextEditingController _addressController;
  late TextEditingController _zipCodeController;
  late TextEditingController _mobileNumberController;

  DateTime? _selectedDate;
  String? _selectedGenderId;
  String? _selectedGender;

  String? _selectedCountryId;
  String? _selectedCountryName;
  String? _selectedProvinceId;
  String? _selectedProvinceName;
  String? _selectedCityName;

  String? _firstNameError;
  String? _lastNameError;
  String? _dateError;
  String? _countryError;
  String? _provinceStateError;
  String? _cityError;
  String? _addressError;
  String? _zipCodeError;
  String? _mobileNumberError;

  // Track which fields have been touched (focused and blurred)
  final Set<String> _touchedFields = {};
  bool _attemptedSubmit = false;

  @override
  void initState() {
    super.initState();
    _firstNameController = TextEditingController();
    _lastNameController = TextEditingController();
    _addressController = TextEditingController();
    _zipCodeController = TextEditingController();
    _mobileNumberController = TextEditingController();

    // Load existing data if available
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.basicInfo != null) {
        final info = form.basicInfo!;
        _firstNameController.text = info.firstName.value;
        _lastNameController.text = info.lastName.value;
        _selectedDate = info.dateOfBirth;
        _selectedGenderId = info.gender.id;
        _selectedGender = info.gender.value;
        _selectedCountryName = info.country;
        _selectedProvinceName = info.provinceState;
        _selectedCityName = info.city;
        _addressController.text = info.address;
        _zipCodeController.text = info.zipCode;
        _mobileNumberController.text = info.mobileNumber;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _addressController.dispose();
    _zipCodeController.dispose();
    _mobileNumberController.dispose();
    super.dispose();
  }

  /// Helper to check if field should show error
  bool _shouldShowError(String fieldName) {
    return _attemptedSubmit || _touchedFields.contains(fieldName);
  }

  /// Validate fields and update error messages
  void _validate() {
    final firstName = Name(_firstNameController.text);
    final lastName = Name(_lastNameController.text);

    final basicInfo = BasicInfo(
      firstName: firstName,
      lastName: lastName,
      dateOfBirth: _selectedDate ?? DateTime.now(),
      gender: Gender(
        id: _selectedGenderId ?? '',
        value: _selectedGender ?? '',
      ),
      country: _selectedCountryName ?? '',
      provinceState: _selectedProvinceName ?? '',
      city: _selectedCityName ?? '',
      address: _addressController.text,
      zipCode: _zipCodeController.text,
      mobileNumber: _mobileNumberController.text,
    );

    setState(() {
      // Only show errors for touched fields or after submit attempt
      _firstNameError = _shouldShowError('firstName') ? firstName.errorMessage : null;
      _lastNameError = _shouldShowError('lastName') ? lastName.errorMessage : null;
      _dateError = _shouldShowError('dateOfBirth') && _selectedDate == null 
          ? 'Date of birth is required' 
          : null;
      _countryError = _shouldShowError('country') ? basicInfo.countryError : null;
      _provinceStateError = _shouldShowError('provinceState') ? basicInfo.provinceStateError : null;
      _cityError = _shouldShowError('city') ? basicInfo.cityError : null;
      _addressError = _shouldShowError('address') ? basicInfo.addressError : null;
      _zipCodeError = _shouldShowError('zipCode') ? basicInfo.zipCodeError : null;
      _mobileNumberError = _shouldShowError('mobileNumber') ? basicInfo.mobileNumberError : null;

      if (_shouldShowError('dateOfBirth') && !basicInfo.isAdult && _selectedDate != null) {
        _dateError = 'You must be at least 18 years old';
      }
    });

    // Always save valid data to view model
    if (basicInfo.isValid) {
      ref.read(registrationViewModelProvider.notifier).updateBasicInfo(basicInfo);
    }
  }

  /// Mark field as touched and validate
  void _markTouched(String fieldName) {
    _touchedFields.add(fieldName);
    _validate();
  }

  /// Validate all fields (called on submit)
  void _validateAndSave() {
    setState(() {
      _attemptedSubmit = true;
    });
    _validate();
  }

  Future<void> _selectDate() async {
    final date = await showDatePicker(
      context: context,
      initialDate: _selectedDate ?? DateTime(2000),
      firstDate: DateTime(1920),
      lastDate: DateTime.now(),
    );
    if (date != null) {
      setState(() {
        _selectedDate = date;
      });
      _markTouched('dateOfBirth');
    }
  }

  void _onCountryChanged(CatalogItem? country) {
    setState(() {
      _selectedCountryId = country?.id;
      _selectedCountryName = country?.value;
      _selectedProvinceId = null;
      _selectedProvinceName = null;
      _selectedCityName = null;
    });
    _markTouched('country');
  }

  void _onProvinceChanged(CatalogItem? province) {
    setState(() {
      _selectedProvinceId = province?.id;
      _selectedProvinceName = province?.value;
      _selectedCityName = null;
    });
    _markTouched('provinceState');
  }

  void _onCityChanged(CatalogItem? city) {
    setState(() {
      _selectedCityName = city?.value;
    });
    _markTouched('city');
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;

    // Listen for step changes to trigger validation when user tries to navigate away
    ref.listen(registrationFormStateProvider, (previous, next) {
      if (previous?.currentStep == 0 && next.currentStep != 0) {
        // User is leaving this step - validate all fields
        _validateAndSave();
      }
    });

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
                'Basic Information',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
              ),
              const SizedBox(height: 8),
              Text(
                'Please provide your personal and location details',
                style: Theme.of(context)
                    .textTheme
                    .bodyMedium
                    ?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),
              
              // Personal Details
              CustomTextField(
                label: 'First Name',
                hint: 'Enter your first name',
                initialValue: _firstNameController.text,
                errorText: _firstNameError,
                onChanged: (value) {
                  _firstNameController.text = value;
                },
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('firstName');
                },
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'Last Name',
                hint: 'Enter your last name',
                initialValue: _lastNameController.text,
                errorText: _lastNameError,
                onChanged: (value) {
                  _lastNameController.text = value;
                },
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('lastName');
                },
              ),
              const SizedBox(height: 24),
              
              // Date of Birth
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Date of Birth',
                    style: Theme.of(context).textTheme.titleSmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                  const SizedBox(height: 8),
                  InkWell(
                    onTap: _selectDate,
                    child: InputDecorator(
                      decoration: InputDecoration(
                        hintText: 'Select your date of birth',
                        errorText: _dateError,
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                        enabledBorder: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(12),
                          borderSide: BorderSide(color: Colors.grey.shade300),
                        ),
                        filled: true,
                        fillColor: Colors.white,
                        suffixIcon: const Icon(Icons.calendar_today),
                      ),
                      child: Text(
                        _selectedDate != null
                            ? DateFormat('MMM dd, yyyy').format(_selectedDate!)
                            : 'Select date',
                        style: TextStyle(
                          color: _selectedDate != null ? Colors.black : Colors.grey,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              
              // Gender
              _buildGenderSelector(),
              const SizedBox(height: 32),
              
              // Location Section
              Text(
                'Location',
                style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
              ),
              const SizedBox(height: 16),
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
              CustomTextField(
                label: 'Address',
                hint: 'Enter your street address',
                initialValue: _addressController.text,
                errorText: _addressError,
                maxLines: 2,
                onChanged: (value) {
                  _addressController.text = value;
                },
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('address');
                },
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'ZIP Code',
                hint: 'Enter your ZIP code',
                initialValue: _zipCodeController.text,
                errorText: _zipCodeError,
                keyboardType: TextInputType.text,
                onChanged: (value) {
                  _zipCodeController.text = value;
                },
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('zipCode');
                },
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'Mobile Number',
                hint: 'Enter your mobile number',
                initialValue: _mobileNumberController.text,
                errorText: _mobileNumberError,
                keyboardType: TextInputType.phone,
                onChanged: (value) {
                  _mobileNumberController.text = value;
                },
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('mobileNumber');
                },
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildGenderSelector() {
    final gendersAsync = ref.watch(gendersProvider);
    final genders = gendersAsync.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Gender',
          style: Theme.of(context).textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.w600,
              ),
        ),
        const SizedBox(height: 12),
        if (genders.isEmpty)
          Text(
            'No gender options available',
            style: TextStyle(color: Colors.grey.shade600),
          )
        else
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: genders.map((gender) {
              final isSelected = _selectedGenderId == gender.id;
              return ChoiceChip(
                label: Text(gender.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    setState(() {
                      _selectedGenderId = gender.id;
                      _selectedGender = gender.value;
                    });
                    _markTouched('gender');
                  }
                },
                selectedColor: AppTheme.primaryBlue,
                labelStyle: TextStyle(
                  color: isSelected ? Colors.white : Colors.black87,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),
      ],
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
