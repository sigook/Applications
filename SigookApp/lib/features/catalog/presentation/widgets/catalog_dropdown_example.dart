import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../notifiers/catalog_notifiers.dart';

/// Example: Country Dropdown using new AsyncNotifier pattern
class CountryDropdownExample extends ConsumerWidget {
  final String? selectedCountryId;
  final ValueChanged<String?>? onChanged;

  const CountryDropdownExample({
    super.key,
    this.selectedCountryId,
    this.onChanged,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final countriesAsync = ref.watch(countriesProvider);

    return countriesAsync.when(
      // Loading state
      loading: () => const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Country',
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: Color(0xFF757575),
            ),
          ),
          SizedBox(height: 8),
          Center(
            child: SizedBox(
              height: 24,
              width: 24,
              child: CircularProgressIndicator(strokeWidth: 2),
            ),
          ),
        ],
      ),

      // Error state with retry
      error: (error, stackTrace) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Country',
            style: TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w500,
              color: Color(0xFF757575),
            ),
          ),
          const SizedBox(height: 8),
          Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.red.shade50,
              borderRadius: BorderRadius.circular(12),
              border: Border.all(color: Colors.red.shade200),
            ),
            child: Column(
              children: [
                Row(
                  children: [
                    Icon(Icons.error_outline, color: Colors.red.shade700),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        'Failed to load countries',
                        style: TextStyle(color: Colors.red.shade700),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton.icon(
                    onPressed: () {
                      // Retry loading
                      ref.read(countriesProvider.notifier).refresh();
                    },
                    icon: const Icon(Icons.refresh),
                    label: const Text('Retry'),
                    style: OutlinedButton.styleFrom(
                      foregroundColor: Colors.red.shade700,
                      side: BorderSide(color: Colors.red.shade700),
                    ),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),

      // Success state with data
      data: (countries) => Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Country',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w500,
                  color: Color(0xFF757575),
                ),
              ),
              // Refresh button
              IconButton(
                icon: const Icon(Icons.refresh, size: 20),
                onPressed: () {
                  ref.read(countriesProvider.notifier).refresh();
                },
                tooltip: 'Refresh countries',
              ),
            ],
          ),
          const SizedBox(height: 8),
          DropdownButtonFormField<String>(
            initialValue: selectedCountryId,
            decoration: InputDecoration(
              hintText: 'Select your country',
              filled: true,
              fillColor: Colors.white,
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide.none,
              ),
              enabledBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: BorderSide.none,
              ),
              focusedBorder: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
                borderSide: const BorderSide(
                  color: Color(0xFF1565C0),
                  width: 2,
                ),
              ),
            ),
            items: countries.map((country) {
              return DropdownMenuItem<String>(
                value: country.id,
                child: Text(country.value),
              );
            }).toList(),
            onChanged: onChanged,
          ),
        ],
      ),
    );
  }
}

/// Example: Gender Selector with Radio Buttons
class GenderSelectorExample extends ConsumerWidget {
  final String? selectedGenderId;
  final ValueChanged<String?>? onChanged;

  const GenderSelectorExample({
    super.key,
    this.selectedGenderId,
    this.onChanged,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final gendersAsync = ref.watch(gendersProvider);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Gender',
          style: TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w500,
            color: Color(0xFF757575),
          ),
        ),
        const SizedBox(height: 12),
        gendersAsync.when(
          loading: () => const Center(
            child: Padding(
              padding: EdgeInsets.all(16),
              child: CircularProgressIndicator(),
            ),
          ),
          error: (error, _) => Text(
            'Error loading genders',
            style: TextStyle(color: Colors.red.shade700),
          ),
          data: (genders) => Wrap(
            spacing: 12,
            runSpacing: 12,
            children: genders.map((gender) {
              final isSelected = gender.id == selectedGenderId;
              return ChoiceChip(
                label: Text(gender.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (selected) {
                    onChanged?.call(gender.id);
                  }
                },
                selectedColor: const Color(0xFF1565C0),
                labelStyle: TextStyle(
                  color: isSelected ? Colors.white : Colors.black87,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),
        ),
      ],
    );
  }
}

/// Example: Skills Multi-Select
class SkillsMultiSelectExample extends ConsumerWidget {
  final List<String> selectedSkillIds;
  final ValueChanged<List<String>>? onChanged;

  const SkillsMultiSelectExample({
    super.key,
    required this.selectedSkillIds,
    this.onChanged,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final skillsAsync = ref.watch(skillsProvider);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'Skills',
          style: TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w500,
            color: Color(0xFF757575),
          ),
        ),
        const SizedBox(height: 12),
        skillsAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (error, _) => Column(
            children: [
              Text(
                'Error loading skills',
                style: TextStyle(color: Colors.red.shade700),
              ),
              const SizedBox(height: 8),
              ElevatedButton.icon(
                onPressed: () => ref.read(skillsProvider.notifier).refresh(),
                icon: const Icon(Icons.refresh),
                label: const Text('Retry'),
              ),
            ],
          ),
          data: (skills) => Wrap(
            spacing: 8,
            runSpacing: 8,
            children: skills.map((skill) {
              final isSelected = selectedSkillIds.contains(skill.id);
              return FilterChip(
                label: Text(skill.value),
                selected: isSelected,
                onSelected: (selected) {
                  if (skill.id == null) return; // Skip items without IDs
                  final updatedList = List<String>.from(selectedSkillIds);
                  if (selected) {
                    updatedList.add(skill.id!);
                  } else {
                    updatedList.remove(skill.id);
                  }
                  onChanged?.call(updatedList);
                },
                selectedColor: const Color(0xFF1565C0).withValues(alpha: 0.2),
                checkmarkColor: const Color(0xFF1565C0),
                labelStyle: TextStyle(
                  color: isSelected ? const Color(0xFF1565C0) : Colors.black87,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
                ),
              );
            }).toList(),
          ),
        ),
      ],
    );
  }
}

/// Example: Pull-to-Refresh with Countries List
class CountriesListWithRefresh extends ConsumerWidget {
  const CountriesListWithRefresh({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final countriesAsync = ref.watch(countriesProvider);

    return RefreshIndicator(
      onRefresh: () async {
        // Trigger refresh
        await ref.read(countriesProvider.notifier).refresh();
      },
      child: countriesAsync.when(
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (error, _) => ListView(
          children: [
            Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text('Error loading countries'),
                  const SizedBox(height: 16),
                  ElevatedButton(
                    onPressed: () {
                      ref.read(countriesProvider.notifier).refresh();
                    },
                    child: const Text('Retry'),
                  ),
                ],
              ),
            ),
          ],
        ),
        data: (countries) => ListView.builder(
          itemCount: countries.length,
          itemBuilder: (context, index) {
            final country = countries[index];
            return ListTile(title: Text(country.value));
          },
        ),
      ),
    );
  }
}
