import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/notifiers/catalog_notifiers.dart';
import '../../domain/entities/personal_info.dart';
import '../../domain/entities/gender.dart';
import '../../domain/entities/value_objects/name.dart';
import '../providers/registration_providers.dart';
import '../widgets/custom_text_field.dart';

class PersonalInfoPage extends ConsumerStatefulWidget {
  const PersonalInfoPage({super.key});

  @override
  ConsumerState<PersonalInfoPage> createState() => _PersonalInfoPageState();
}

class _PersonalInfoPageState extends ConsumerState<PersonalInfoPage> {
  late TextEditingController _firstNameController;
  late TextEditingController _lastNameController;
  DateTime? _selectedDate;
  String? _selectedGender;
  String? _selectedGenderId;

  String? _firstNameError;
  String? _lastNameError;
  String? _dateError;

  @override
  void initState() {
    super.initState();
    _firstNameController = TextEditingController();
    _lastNameController = TextEditingController();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.personalInfo != null) {
        _firstNameController.text = form.personalInfo!.firstName.value;
        _lastNameController.text = form.personalInfo!.lastName.value;
        _selectedDate = form.personalInfo!.dateOfBirth;
        _selectedGenderId = form.personalInfo!.gender.id;
        _selectedGender = form.personalInfo!.gender.value;
        setState(() {});
      }
    });
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    super.dispose();
  }

  void _validateAndSave() {
    setState(() {
      final firstName = Name(_firstNameController.text);
      final lastName = Name(_lastNameController.text);

      _firstNameError = firstName.errorMessage;
      _lastNameError = lastName.errorMessage;
      _dateError = _selectedDate == null ? 'Date of birth is required' : null;

      if (_firstNameError == null &&
          _lastNameError == null &&
          _dateError == null &&
          _selectedDate != null &&
          _selectedGender != null &&
          _selectedGenderId != null) {
        final personalInfo = PersonalInfo(
          firstName: firstName,
          lastName: lastName,
          dateOfBirth: _selectedDate!,
          gender: Gender(id: _selectedGenderId!, value: _selectedGender!),
        );

        if (!personalInfo.isAdult) {
          _dateError = 'You must be at least 18 years old';
        } else {
          ref
              .read(registrationViewModelProvider.notifier)
              .updatePersonalInfo(personalInfo);
        }
      }
    });
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
      _validateAndSave();
    }
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
                'Personal Information',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Please provide your basic information',
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),
              CustomTextField(
                label: 'First Name',
                hint: 'Enter your first name',
                initialValue: _firstNameController.text,
                errorText: _firstNameError,
                onChanged: (value) {
                  _firstNameController.text = value;
                  _validateAndSave();
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
                  _validateAndSave();
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
              // Gender selector with API data
              _buildGenderSelector(),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildGenderSelector() {
    final gendersAsync = ref.watch(gendersProvider);

    return gendersAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Gender',
            style: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 8),
          const Center(
            child: SizedBox(
              height: 24,
              width: 24,
              child: CircularProgressIndicator(strokeWidth: 2),
            ),
          ),
        ],
      ),
      error: (error, stackTrace) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Gender',
            style: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 8),
          Container(
            padding: const EdgeInsets.all(12),
            decoration: BoxDecoration(
              color: Colors.red.shade50,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.red.shade200),
            ),
            child: Row(
              children: [
                Icon(Icons.error_outline, color: Colors.red.shade700, size: 20),
                const SizedBox(width: 8),
                Expanded(
                  child: Text(
                    'Failed to load genders',
                    style: TextStyle(color: Colors.red.shade700, fontSize: 13),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.invalidate(gendersProvider);
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (genders) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Gender',
            style: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w600),
          ),
          const SizedBox(height: 12),
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
                    _validateAndSave();
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
      ),
    );
  }
}
