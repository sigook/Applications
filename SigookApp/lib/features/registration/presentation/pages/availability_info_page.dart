import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/enums.dart' as enums;
import '../../../../core/theme/app_theme.dart';
import '../../../catalog/presentation/notifiers/catalog_notifiers.dart';
import '../../domain/entities/availability_info.dart';
import '../../domain/entities/availability_type.dart';
import '../../domain/entities/available_time.dart';
import '../providers/registration_providers.dart';
import '../widgets/multi_select_chips.dart';

class AvailabilityInfoPage extends ConsumerStatefulWidget {
  const AvailabilityInfoPage({super.key});

  @override
  ConsumerState<AvailabilityInfoPage> createState() =>
      _AvailabilityInfoPageState();
}

class _AvailabilityInfoPageState extends ConsumerState<AvailabilityInfoPage> {
  String? _selectedAvailabilityId;
  String? _selectedAvailabilityName;
  List<AvailableTime> _selectedTimes = [];
  List<enums.DayOfWeek> _selectedDays = [];

  String? _availabilityError;
  String? _timesError;
  String? _daysError;

  @override
  void initState() {
    super.initState();

    // Load existing data
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.availabilityInfo != null) {
        setState(() {
          _selectedAvailabilityId = form.availabilityInfo!.availabilityType.id;
          _selectedAvailabilityName = form.availabilityInfo!.availabilityType.value;
          _selectedTimes = List.from(form.availabilityInfo!.availableTimes);
          _selectedDays = List<enums.DayOfWeek>.from(
            form.availabilityInfo!.availableDays,
          );
        });
      }
    });
  }

  void _validateAndSave() {
    setState(() {
      _availabilityError = _selectedAvailabilityId == null
          ? 'Please select availability type'
          : null;
      _timesError = _selectedTimes.isEmpty
          ? 'Please select at least one time slot'
          : null;
      _daysError = _selectedDays.isEmpty
          ? 'Please select at least one day'
          : null;

      if (_availabilityError == null &&
          _timesError == null &&
          _daysError == null) {
        final availabilityInfo = AvailabilityInfo(
          availabilityType: AvailabilityType(
            id: _selectedAvailabilityId!,
            value: _selectedAvailabilityName!,
          ),
          availableTimes: _selectedTimes,
          availableDays: _selectedDays,
        );
        ref
            .read(registrationViewModelProvider.notifier)
            .updateAvailabilityInfo(availabilityInfo);
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
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),
              _buildAvailabilityTypeSelector(),
              const SizedBox(height: 32),
              _buildTimeSlotSelector(),
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

  Widget _buildAvailabilityTypeSelector() {
    final availabilityAsync = ref.watch(availabilityProvider);

    return availabilityAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Availability Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          const Center(child: CircularProgressIndicator()),
        ],
      ),
      error: (error, _) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Availability Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
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
                const Expanded(
                  child: Text(
                    'Failed to load availability types',
                    style: TextStyle(color: Colors.red),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.read(availabilityProvider.notifier).refresh();
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (types) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Availability Type',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: types.map((type) {
              final isSelected = _selectedAvailabilityId == type.id;
              return ChoiceChip(
                label: Text(type.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    setState(() {
                      _selectedAvailabilityId = type.id;
                      _selectedAvailabilityName = type.value;
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
          if (_availabilityError != null) ...[
            const SizedBox(height: 8),
            Text(
              _availabilityError!,
              style: const TextStyle(color: Colors.red, fontSize: 12),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildTimeSlotSelector() {
    final timeSlotsAsync = ref.watch(availabilityTimeProvider);

    return timeSlotsAsync.when(
      loading: () => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Available Time Slots',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          const Center(child: CircularProgressIndicator()),
        ],
      ),
      error: (error, _) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Available Time Slots',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
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
                const Expanded(
                  child: Text(
                    'Failed to load time slots',
                    style: TextStyle(color: Colors.red),
                  ),
                ),
                TextButton(
                  onPressed: () {
                    ref.read(availabilityTimeProvider.notifier).refresh();
                  },
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        ],
      ),
      data: (timeSlots) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Available Time Slots',
            style: TextStyle(fontWeight: FontWeight.w600, fontSize: 16),
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: timeSlots.map((slot) {
              final isSelected = _selectedTimes.any((t) => t.id == slot.id);
              return FilterChip(
                label: Text(slot.value),
                selected: isSelected,
                onSelected: (selected) {
                  setState(() {
                    if (selected) {
                      _selectedTimes.add(AvailableTime(id: slot.id, value: slot.value));
                    } else {
                      _selectedTimes.removeWhere((t) => t.id == slot.id);
                    }
                  });
                  _validateAndSave();
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
          if (_timesError != null) ...[
            const SizedBox(height: 8),
            Text(
              _timesError!,
              style: const TextStyle(color: Colors.red, fontSize: 12),
            ),
          ],
        ],
      ),
    );
  }
}
