import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/profile_photo.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/value_objects/zip_code.dart';
import 'package:sigook_app_flutter/features/registration/presentation/widgets/profile_photo_picker.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../../domain/entities/basic_info.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/value_objects/name.dart';
import '../../domain/entities/country.dart';
import '../../domain/entities/province.dart';
import '../../domain/entities/city.dart';
import '../../domain/entities/value_objects/phone_number.dart';
import '../../domain/services/phone_validation_service.dart';
import '../../data/services/phone_number_parser_validation_service.dart';
import '../providers/registration_providers.dart';
import '../widgets/location_selector.dart';
import '../widgets/phone_number_field.dart';
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

  late PhoneValidationService _phoneService;
  PhoneNumber _mobileNumber = PhoneNumber.empty();

  DateTime? _selectedDate;
  String? _selectedGenderId;
  String? _selectedGender;

  Country? _selectedCountry;
  Province? _selectedProvince;
  City? _selectedCity;

  String? _firstNameError;
  String? _lastNameError;
  String? _dateError;
  String? _countryError;
  String? _provinceStateError;
  String? _cityError;
  String? _addressError;
  String? _zipCodeError;
  String? _mobileNumberError;

  final Set<String> _touchedFields = {};
  bool _attemptedSubmit = false;

  @override
  void initState() {
    super.initState();
    _firstNameController = TextEditingController();
    _lastNameController = TextEditingController();
    _addressController = TextEditingController();
    _zipCodeController = TextEditingController();
    _phoneService = PhoneNumberParserValidationService();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.basicInfo != null) {
        final info = form.basicInfo!;
        _firstNameController.text = info.firstName.value;
        _lastNameController.text = info.lastName.value;
        _selectedDate = info.dateOfBirth;
        _selectedGenderId = info.gender.id;
        _selectedGender = info.gender.value;
        _selectedCountry = info.country;
        _selectedProvince = info.provinceState;
        _selectedCity = info.city;
        _addressController.text = info.address;
        _zipCodeController.text = info.zipCode.value;
        _mobileNumber = info.mobileNumber;
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
    super.dispose();
  }

  bool _shouldShowError(String fieldName) {
    return _attemptedSubmit || _touchedFields.contains(fieldName);
  }

  void _validate() {
    final firstName = Name(_firstNameController.text);
    final lastName = Name(_lastNameController.text);

    final currentPhoto =
        ref.read(registrationViewModelProvider).basicInfo?.profilePhoto ??
        ProfilePhoto.empty();

    final basicInfo = BasicInfo(
      profilePhoto: currentPhoto,
      firstName: firstName,
      lastName: lastName,
      dateOfBirth: _selectedDate ?? DateTime.now(),
      gender: Gender(id: _selectedGenderId ?? '', value: _selectedGender ?? ''),
      country: _selectedCountry,
      provinceState: _selectedProvince,
      city: _selectedCity,
      address: _addressController.text,
      zipCode:
          ZipCode.parse(
            input: _zipCodeController.text,
            countryCode: _selectedCountry?.code ?? 'US',
            provinceCode: _selectedProvince?.code,
          ).fold(
            (error) => _selectedCountry?.code == 'CA'
                ? ZipCode.emptyCA
                : ZipCode.emptyUS,
            (validZip) => validZip,
          ),
      mobileNumber: _mobileNumber,
    );

    setState(() {
      _firstNameError = _shouldShowError('firstName')
          ? firstName.errorMessage
          : null;
      _lastNameError = _shouldShowError('lastName')
          ? lastName.errorMessage
          : null;
      _dateError = _shouldShowError('dateOfBirth') && _selectedDate == null
          ? 'Date of birth is required'
          : null;
      _countryError = _shouldShowError('country')
          ? basicInfo.countryError
          : null;
      _provinceStateError = _shouldShowError('provinceState')
          ? basicInfo.provinceStateError
          : null;
      _cityError = _shouldShowError('city') ? basicInfo.cityError : null;
      _addressError = _shouldShowError('address')
          ? basicInfo.addressError
          : null;
      _zipCodeError = _shouldShowError('zipCode')
          ? basicInfo.zipCodeError
          : null;
      _mobileNumberError = _shouldShowError('mobileNumber')
          ? basicInfo.mobileNumberError
          : null;

      if (_shouldShowError('dateOfBirth') &&
          !basicInfo.isAdult &&
          _selectedDate != null) {
        _dateError = 'You must be at least 18 years old';
      }
    });

    if (basicInfo.isValid) {
      ref
          .read(registrationViewModelProvider.notifier)
          .updateBasicInfo(basicInfo);
    }
  }

  void _markTouched(String fieldName) {
    _touchedFields.add(fieldName);
    _validate();
  }

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

  void _onCountryChanged(Country? country) {
    setState(() {
      _selectedCountry = country;
      _selectedProvince = null;
      _selectedCity = null;
    });
    _markTouched('country');
    _validate();
  }

  void _onProvinceChanged(Province? province) {
    setState(() {
      _selectedProvince = province;
      _selectedCity = null;
    });
    _markTouched('provinceState');
    _validate();
  }

  void _onCityChanged(City? city) {
    setState(() {
      _selectedCity = city;
    });
    _markTouched('city');
    _validate();
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;

    ref.listen(registrationFormStateProvider, (previous, next) {
      if (previous?.currentStep == 0 && next.currentStep != 0) {
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
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),

              ProfilePhotoPicker(
                errorText: 'Profile photo is required',
                showError:
                    _shouldShowError('profilePhoto') &&
                    !(ref
                            .watch(registrationViewModelProvider)
                            .basicInfo
                            ?.profilePhoto
                            .hasPhoto ??
                        false),
              ),
              const SizedBox(height: 32),
              CustomTextField(
                label: 'First Name',
                hint: 'Enter your first name',
                controller: _firstNameController,
                errorText: _firstNameError,
                onChanged: (value) {},
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('firstName');
                },
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'Last Name',
                hint: 'Enter your last name',
                controller: _lastNameController,
                errorText: _lastNameError,
                onChanged: (value) {},
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('lastName');
                },
              ),
              const SizedBox(height: 24),

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
                          color: _selectedDate != null
                              ? Colors.black
                              : Colors.grey,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              _buildGenderSelector(),
              const SizedBox(height: 32),
              Text(
                'Location',
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              LocationSelector(
                selectedCountry: _selectedCountry,
                selectedProvince: _selectedProvince,
                selectedCity: _selectedCity,
                onCountryChanged: _onCountryChanged,
                onProvinceChanged: _onProvinceChanged,
                onCityChanged: _onCityChanged,
                countryError: _countryError,
                provinceError: _provinceStateError,
                cityError: _cityError,
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'Address',
                hint: 'Enter your street address',
                controller: _addressController,
                errorText: _addressError,
                maxLines: 2,
                onChanged: (value) {},
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('address');
                },
              ),
              const SizedBox(height: 24),
              CustomTextField(
                label: 'ZIP Code',
                hint: 'Enter your ZIP code',
                controller: _zipCodeController,
                errorText: _zipCodeError,
                keyboardType: TextInputType.text,
                onChanged: (value) {},
                onFocusChanged: (hasFocus) {
                  if (!hasFocus) _markTouched('zipCode');
                },
              ),
              const SizedBox(height: 24),
              PhoneNumberField(
                label: 'Mobile Number',
                initialValue: _mobileNumber.value,
                countryCode: _selectedCountry?.code ?? 'US',
                errorText: _mobileNumberError,
                onChanged: (value) {
                  final countryCode = _selectedCountry?.code ?? 'US';
                  final validated = _phoneService.validate(value, countryCode);
                  setState(() {
                    _mobileNumber = validated;
                  });
                  _validate();
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
          style: Theme.of(
            context,
          ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
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
}
