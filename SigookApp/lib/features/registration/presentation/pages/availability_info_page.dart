import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/enums.dart' as enums;
import '../../domain/entities/availability_info.dart';
import '../providers/registration_providers.dart';
import '../widgets/custom_dropdown.dart';
import '../widgets/multi_select_chips.dart';

class AvailabilityInfoPage extends ConsumerStatefulWidget {
  const AvailabilityInfoPage({super.key});

  @override
  ConsumerState<AvailabilityInfoPage> createState() => _AvailabilityInfoPageState();
}

class _AvailabilityInfoPageState extends ConsumerState<AvailabilityInfoPage> {
  enums.AvailabilityType? _selectedType;
  List<enums.TimeOfDay> _selectedTimes = [];
  List<enums.DayOfWeek> _selectedDays = [];

  String? _timesError;
  String? _daysError;

  @override
  void initState() {
    super.initState();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.availabilityInfo != null) {
        _selectedType = form.availabilityInfo!.availabilityType;
        _selectedTimes = List<enums.TimeOfDay>.from(form.availabilityInfo!.availableTimes);
        _selectedDays = List<enums.DayOfWeek>.from(form.availabilityInfo!.availableDays);
        setState(() {});
      } else {
        // Default to full-time
        _selectedType = enums.AvailabilityType.fullTime;
      }
    });
  }

  void _validateAndSave() {
    setState(() {
      _timesError = _selectedTimes.isEmpty ? 'Please select at least one time slot' : null;
      _daysError = _selectedDays.isEmpty ? 'Please select at least one day' : null;

      if (_timesError == null && _daysError == null && _selectedType != null) {
        final availabilityInfo = AvailabilityInfo(
          availabilityType: _selectedType!,
          availableTimes: _selectedTimes,
          availableDays: _selectedDays,
        );
        ref.read(registrationViewModelProvider.notifier).updateAvailabilityInfo(availabilityInfo);
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
            'Availability',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
          ),
          const SizedBox(height: 8),
          Text(
            'Please provide your availability details',
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: Colors.grey.shade600,
                ),
          ),
          const SizedBox(height: 32),
          CustomDropdown<enums.AvailabilityType>(
            label: 'Availability Type',
            value: _selectedType,
            items: enums.AvailabilityType.values,
            getLabel: (type) => type.displayName,
            hint: 'Select availability type',
            onChanged: (value) {
              setState(() {
                _selectedType = value;
              });
              _validateAndSave();
            },
          ),
          const SizedBox(height: 32),
          MultiSelectChips<enums.TimeOfDay>(
            label: 'Available Times',
            options: enums.TimeOfDay.values,
            selectedOptions: _selectedTimes,
            getLabel: (time) => time.displayName,
            errorText: _timesError,
            onChanged: (selected) {
              setState(() {
                _selectedTimes = selected;
              });
              _validateAndSave();
            },
          ),
          const SizedBox(height: 32),
          MultiSelectChips<enums.DayOfWeek>(
            label: 'Available Days',
            options: enums.DayOfWeek.values,
            selectedOptions: _selectedDays,
            getLabel: (day) => day.displayName,
            errorText: _daysError,
            onChanged: (selected) {
              setState(() {
                _selectedDays = selected;
              });
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
