import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/chip_display_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../catalog/presentation/providers/catalog_providers.dart';
import '../../../../preferences/presentation/viewmodels/preferences_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/chip_selector.dart';
import '../../../widgets/searchable_toggle_list.dart';
import '../../../widgets/section_edit_actions.dart';

class PreferencesSectionCard extends ConsumerStatefulWidget {
  const PreferencesSectionCard({super.key});

  @override
  ConsumerState<PreferencesSectionCard> createState() =>
      _PreferencesSectionCardState();
}

class _PreferencesSectionCardState
    extends ConsumerState<PreferencesSectionCard> {
  Set<String> _availabilityIds = {};
  Set<String> _availabilityTimeIds = {};
  Set<String> _availabilityDayIds = {};
  String? _liftId;
  Set<String> _languageIds = {};
  Set<String> _skillIds = {};

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    setState(() {
      _availabilityIds = Set.from(profile?.availabilityIds ?? []);
      _availabilityTimeIds = Set.from(profile?.availabilityTimeIds ?? []);
      _availabilityDayIds = Set.from(profile?.availabilityDayIds ?? []);
      _liftId = profile?.liftId;
      _languageIds = Set.from(profile?.languageIds ?? []);
      _skillIds = Set.from(profile?.skills ?? []);
    });
  }

  void _toggle(Set<String> set, String id, bool selected) {
    setState(() {
      if (selected) {
        set.add(id);
      } else {
        set.remove(id);
      }
    });
  }

  List<Map<String, String>> _resolveItems(
    Set<String> selectedIds,
    AsyncValue<dynamic> catalogAsync,
  ) {
    final items = catalogAsync.asData?.value as List<dynamic>? ?? [];
    return selectedIds
        .map((id) {
          final matches = items
              .cast<dynamic>()
              .where((item) => (item.id as String?) == id);
          if (matches.isEmpty) return null;
          final item = matches.first;
          final value =
              item.value != null ? item.value as String : item.toString();
          return {'id': id, 'value': value};
        })
        .whereType<Map<String, String>>()
        .toList();
  }

  Map<String, String> _buildFields() {
    final availabilities =
        _resolveItems(_availabilityIds, ref.read(availabilityListProvider));
    final availabilityTimes =
        _resolveItems(_availabilityTimeIds, ref.read(availabilityTimeListProvider));
    final availabilityDays =
        _resolveItems(_availabilityDayIds, ref.read(daysOfWeekProvider));
    final languages =
        _resolveItems(_languageIds, ref.read(languagesProvider));
    final skills = _skillIds.toList();

    String? liftJson;
    if (_liftId != null) {
      final liftItems =
          ref.read(liftingCapacitiesProvider).asData?.value as List<dynamic>? ??
              [];
      final match = liftItems
          .cast<dynamic>()
          .where((item) => (item.id as String?) == _liftId);
      if (match.isNotEmpty) {
        final item = match.first;
        final value =
            item.value != null ? item.value as String : item.toString();
        liftJson = jsonEncode({'id': _liftId, 'value': value});
      }
    }

    return {
      'availabilities': jsonEncode(availabilities),
      'availabilityTimes': jsonEncode(availabilityTimes),
      'availabilityDays': jsonEncode(availabilityDays),
      'languages': jsonEncode(languages),
      'skills': jsonEncode(skills),
      // ignore: use_null_aware_elements
      if (liftJson != null) 'lift': liftJson,
    };
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(preferencesViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      preferencesViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen<PreferencesState>(preferencesViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    final actions = SectionEditActions(
      isEditingThis: vm.isEditing,
      isAnyEditing: false,
      isSaving: vm.isSaving,
      onEdit: profile != null
          ? ref.read(preferencesViewModelProvider.notifier).startEditing
          : null,
      onCancel:
          ref.read(preferencesViewModelProvider.notifier).cancelEditing,
      onSave: () => ref
          .read(preferencesViewModelProvider.notifier)
          .save(_buildFields()),
    );

    if (!vm.isEditing) {
      return ProfileSectionCard(
        title: 'Preferences',
        icon: Icons.work_outline,
        iconGradient: const [Color(0xFF2196F3), Color(0xFF64B5F6)],
        trailing: actions,
        children: [
          ChipDisplayRow(
            label: 'Can you lift up to',
            icon: Icons.fitness_center_outlined,
            chips: profile?.liftCapacity != null ? [profile!.liftCapacity!] : [],
          ),
          ChipDisplayRow(
            label: 'Availability',
            icon: Icons.schedule_outlined,
            chips: profile?.availabilities ?? [],
          ),
          ChipDisplayRow(
            label: 'Available Time',
            icon: Icons.access_time_outlined,
            chips: profile?.availabilityTimes ?? [],
          ),
          ChipDisplayRow(
            label: 'Available Days',
            icon: Icons.calendar_today_outlined,
            chips: profile?.availabilityDays ?? [],
          ),
          ChipDisplayRow(
            label: 'Skills',
            icon: Icons.stars_outlined,
            chips: profile?.skills ?? [],
          ),
          ChipDisplayRow(
            label: 'Languages',
            icon: Icons.language_outlined,
            chips: profile?.languages ?? [],
          ),
        ],
      );
    }

    return ProfileSectionCard(
      title: 'Preferences',
      icon: Icons.work_outline,
      iconGradient: const [Color(0xFF2196F3), Color(0xFF64B5F6)],
      trailing: actions,
      children: [
        ChipSelector(
          label: 'Can you lift up to',
          asyncValue: ref.watch(liftingCapacitiesProvider),
          selectedIds: _liftId != null ? {_liftId!} : {},
          singleSelect: true,
          onToggle: (id, selected) =>
              setState(() => _liftId = selected ? id : null),
        ),
        const SizedBox(height: 12),
        ChipSelector(
          label: 'Availability',
          asyncValue: ref.watch(availabilityListProvider),
          selectedIds: _availabilityIds,
          singleSelect: false,
          onToggle: (id, selected) => _toggle(_availabilityIds, id, selected),
        ),
        const SizedBox(height: 12),
        ChipSelector(
          label: 'Available Time',
          asyncValue: ref.watch(availabilityTimeListProvider),
          selectedIds: _availabilityTimeIds,
          singleSelect: false,
          onToggle: (id, selected) =>
              _toggle(_availabilityTimeIds, id, selected),
        ),
        const SizedBox(height: 12),
        ChipSelector(
          label: 'Available Days',
          asyncValue: ref.watch(daysOfWeekProvider),
          selectedIds: _availabilityDayIds,
          singleSelect: false,
          onToggle: (id, selected) =>
              _toggle(_availabilityDayIds, id, selected),
        ),
        const SizedBox(height: 12),
        SearchableToggleList(
          label: 'Skills',
          asyncValue: ref.watch(skillsProvider),
          selectedIds: _skillIds,
          onToggle: (id, selected) => _toggle(_skillIds, id, selected),
          onAddCustom: (value) => setState(() => _skillIds.add(value)),
        ),
        const SizedBox(height: 12),
        SearchableToggleList(
          label: 'Languages',
          asyncValue: ref.watch(languagesProvider),
          selectedIds: _languageIds,
          onToggle: (id, selected) => _toggle(_languageIds, id, selected),
        ),
      ],
    );
  }
}
