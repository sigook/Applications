import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../domain/entities/lifting_capacity.dart';
import '../../domain/entities/preferences_info.dart';
import '../../domain/entities/language.dart';
import '../../domain/entities/skill.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/available_time.dart';
import '../../domain/entities/day_of_week.dart';
import '../providers/registration_providers.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';
import '../widgets/availability_type_selector.dart';
import '../widgets/availability_time_selector.dart';
import '../widgets/day_selector.dart';
import '../widgets/language_autocomplete_field.dart';
import '../widgets/skill_autocomplete_field.dart';

class PreferencesPage extends ConsumerStatefulWidget {
  const PreferencesPage({super.key});

  @override
  ConsumerState<PreferencesPage> createState() => _PreferencesPageState();
}

class _PreferencesPageState extends ConsumerState<PreferencesPage> {
  AvailabilityType? _selectedAvailabilityType;
  List<AvailableTime> _selectedAvailableTimes = [];
  List<DayOfWeekEntity> _selectedDays = [];
  LiftingCapacity? _selectedLiftingCapacity;
  bool _hasVehicle = false;
  List<Language> _selectedLanguages = [];
  List<Skill> _selectedSkills = [];

  String? _availabilityTypeError;
  String? _availableTimesError;
  String? _availableDaysError;
  String? _liftingCapacityError;
  String? _languagesError;
  String? _skillsError;

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.preferencesInfo != null) {
        final info = form.preferencesInfo!;
        _selectedAvailabilityType = info.availabilityType;
        _selectedAvailableTimes = List.from(info.availableTimes);
        _selectedDays = List.from(info.availableDays);
        _selectedLiftingCapacity = info.liftingCapacity;
        _hasVehicle = info.hasVehicle;
        _selectedLanguages = List.from(info.languages);
        _selectedSkills = List.from(info.skills);
        setState(() {});
      }
    });
  }

  void _validateAndSave() {
    setState(() {
      final preferencesInfo = PreferencesInfo(
        availabilityType:
            _selectedAvailabilityType ?? AvailabilityType(id: '', value: ''),
        availableTimes: _selectedAvailableTimes,
        availableDays: _selectedDays,
        liftingCapacity: _selectedLiftingCapacity,
        hasVehicle: _hasVehicle,
        languages: _selectedLanguages,
        skills: _selectedSkills,
      );
      _availabilityTypeError = null;
      _availableTimesError = null;
      _availableDaysError = null;
      _liftingCapacityError = null;
      _languagesError = null;
      _skillsError = null;

      ref
          .read(registrationViewModelProvider.notifier)
          .updatePreferencesInfo(preferencesInfo);
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
                'Preferences',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Tell us about your availability and capabilities',
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),

              AvailabilityTypeSelector(
                selectedType: _selectedAvailabilityType,
                onChanged: (value) {
                  setState(() {
                    _selectedAvailabilityType = value;
                  });
                  _validateAndSave();
                },
                errorText: _availabilityTypeError,
              ),
              const SizedBox(height: 24),

              AvailabilityTimeSelector(
                selectedTimes: _selectedAvailableTimes,
                onChanged: (times) {
                  setState(() {
                    _selectedAvailableTimes = times;
                  });
                  _validateAndSave();
                },
                errorText: _availableTimesError,
              ),
              const SizedBox(height: 24),

              DaySelector(
                selectedDays: _selectedDays,
                onChanged: (days) {
                  setState(() {
                    _selectedDays = days;
                  });
                  _validateAndSave();
                },
                errorText: _availableDaysError,
              ),
              const SizedBox(height: 32),

              Text(
                'Physical Capabilities',
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              _buildLiftingCapacitySelector(),
              const SizedBox(height: 24),

              _buildVehicleCheckbox(),
              const SizedBox(height: 32),

              Text(
                'Professional Skills',
                style: Theme.of(
                  context,
                ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 16),
              LanguageAutocompleteField(
                selectedLanguages: _selectedLanguages,
                onChanged: (languages) {
                  setState(() {
                    _selectedLanguages = languages;
                  });
                  _validateAndSave();
                },
                errorText: _languagesError,
              ),
              const SizedBox(height: 24),

              SkillAutocompleteField(
                selectedSkills: _selectedSkills,
                onChanged: (skills) {
                  setState(() {
                    _selectedSkills = skills;
                  });
                  _validateAndSave();
                },
                errorText: _skillsError,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildLiftingCapacitySelector() {
    final capacitiesAsync = ref.watch(liftingCapacitiesProvider);
    final catalogCapacities = capacitiesAsync.value ?? [];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'Can you lift up to',
          style: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: Color(0xFF1E293B),
          ),
        ),
        if (_liftingCapacityError != null) ...[
          const SizedBox(height: 4),
          Text(
            _liftingCapacityError!,
            style: TextStyle(color: Colors.red.shade700, fontSize: 12),
          ),
        ],
        const SizedBox(height: 12),
        if (catalogCapacities.isEmpty)
          Text(
            'Loading options...',
            style: TextStyle(color: Colors.grey.shade600),
          )
        else
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: catalogCapacities.map((capacity) {
              final isSelected =
                  _selectedLiftingCapacity?.id == capacity.id ||
                  _selectedLiftingCapacity?.value == capacity.value;
              return FilterChip(
                label: Text(capacity.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    setState(() {
                      _selectedLiftingCapacity = LiftingCapacity(
                        id: capacity.id,
                        value: capacity.value,
                      );
                    });
                    _validateAndSave();
                  }
                },
                selectedColor: AppTheme.primaryBlue.withValues(alpha: 0.2),
                checkmarkColor: AppTheme.primaryBlue,
                labelStyle: TextStyle(
                  color: isSelected ? AppTheme.primaryBlue : Colors.black87,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),
      ],
    );
  }

  Widget _buildVehicleCheckbox() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.grey.shade50,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: Colors.grey.shade300),
      ),
      child: CheckboxListTile(
        title: const Text(
          'Do you have your own vehicle?',
          style: TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: Color(0xFF1E293B),
          ),
        ),
        value: _hasVehicle,
        activeColor: AppTheme.primaryBlue,
        onChanged: (value) {
          setState(() {
            _hasVehicle = value ?? false;
          });
          _validateAndSave();
        },
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      ),
    );
  }
}
