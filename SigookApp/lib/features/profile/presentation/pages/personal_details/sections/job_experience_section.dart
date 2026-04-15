import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/inputs/date_picker_field.dart';
import '../../../../job_experience/domain/entities/job_experience.dart';
import '../../../../job_experience/presentation/providers/job_experience_providers.dart';
import '../../../../job_experience/presentation/viewmodels/job_experience_viewmodel.dart';
import '../../../../job_experience/presentation/widgets/job_experience_card.dart';

class JobExperienceSectionCard extends ConsumerStatefulWidget {
  const JobExperienceSectionCard({super.key});

  @override
  ConsumerState<JobExperienceSectionCard> createState() =>
      _JobExperienceSectionCardState();
}

class _JobExperienceSectionCardState
    extends ConsumerState<JobExperienceSectionCard> {
  // Add form controllers
  final _companyController = TextEditingController();
  final _supervisorController = TextEditingController();
  final _dutiesController = TextEditingController();
  DateTime? _startDate;
  DateTime? _endDate;
  bool _isCurrent = false;

  // Edit form controllers
  final _editCompanyController = TextEditingController();
  final _editSupervisorController = TextEditingController();
  final _editDutiesController = TextEditingController();
  DateTime? _editStartDate;
  DateTime? _editEndDate;
  bool _editIsCurrent = false;

  @override
  void dispose() {
    _companyController.dispose();
    _supervisorController.dispose();
    _dutiesController.dispose();
    _editCompanyController.dispose();
    _editSupervisorController.dispose();
    _editDutiesController.dispose();
    super.dispose();
  }

  void _resetAddForm() {
    _companyController.clear();
    _supervisorController.clear();
    _dutiesController.clear();
    setState(() {
      _startDate = null;
      _endDate = null;
      _isCurrent = false;
    });
  }

  void _populateEditForm(JobExperience experience) {
    _editCompanyController.text = experience.company;
    _editSupervisorController.text = experience.supervisor ?? '';
    _editDutiesController.text = experience.duties ?? '';
    setState(() {
      _editStartDate = experience.startDate != null
          ? DateTime.tryParse(experience.startDate!)
          : null;
      _editEndDate = experience.endDate != null
          ? DateTime.tryParse(experience.endDate!)
          : null;
      _editIsCurrent = experience.isCurrentJobPosition;
    });
  }

  Future<void> _pickDate({required bool isEdit, required bool isStart}) async {
    final now = DateTime.now();
    DateTime? current;
    if (isEdit) {
      current = isStart ? _editStartDate : _editEndDate;
    } else {
      current = isStart ? _startDate : _endDate;
    }

    final picked = await showDatePicker(
      context: context,
      initialDate: current ?? now,
      firstDate: DateTime(1950),
      lastDate: DateTime(2100),
      builder: (context, child) => Theme(
        data: Theme.of(context).copyWith(
          colorScheme:
              const ColorScheme.light(primary: AppTheme.primaryBlue),
        ),
        child: child!,
      ),
    );
    if (picked == null) return;
    setState(() {
      if (isEdit) {
        if (isStart) {
          _editStartDate = picked;
        } else {
          _editEndDate = picked;
        }
      } else {
        if (isStart) {
          _startDate = picked;
        } else {
          _endDate = picked;
        }
      }
    });
  }

  Future<void> _submitAdd() async {
    if (_companyController.text.trim().isEmpty) {
      showProfileError(context, 'Company name is required');
      return;
    }
    if (_startDate == null) {
      showProfileError(context, 'Start date is required');
      return;
    }
    if (!_isCurrent && _endDate == null) {
      showProfileError(
          context, 'Enter an end date or check "Currently working here"');
      return;
    }

    await ref.read(jobExperienceViewModelProvider.notifier).add(
          company: _companyController.text.trim(),
          supervisor: _supervisorController.text.trim().isNotEmpty
              ? _supervisorController.text.trim()
              : null,
          duties: _dutiesController.text.trim().isNotEmpty
              ? _dutiesController.text.trim()
              : null,
          startDate: _startDate!.toUtc().toIso8601String(),
          endDate: _isCurrent ? null : _endDate?.toUtc().toIso8601String(),
          isCurrentJobPosition: _isCurrent,
        );
  }

  Future<void> _submitEdit(String id) async {
    if (_editCompanyController.text.trim().isEmpty) {
      showProfileError(context, 'Company name is required');
      return;
    }
    if (_editStartDate == null) {
      showProfileError(context, 'Start date is required');
      return;
    }
    if (!_editIsCurrent && _editEndDate == null) {
      showProfileError(
          context, 'Enter an end date or check "Currently working here"');
      return;
    }

    await ref.read(jobExperienceViewModelProvider.notifier).update(
          id: id,
          company: _editCompanyController.text.trim(),
          supervisor: _editSupervisorController.text.trim().isNotEmpty
              ? _editSupervisorController.text.trim()
              : null,
          duties: _editDutiesController.text.trim().isNotEmpty
              ? _editDutiesController.text.trim()
              : null,
          startDate: _editStartDate!.toUtc().toIso8601String(),
          endDate:
              _editIsCurrent ? null : _editEndDate?.toUtc().toIso8601String(),
          isCurrentJobPosition: _editIsCurrent,
        );
  }

  Future<void> _confirmDelete(String id) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Experience'),
        content: const Text(
            'Are you sure you want to delete this work experience?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
            child: const Text('Delete',
                style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;
    await ref.read(jobExperienceViewModelProvider.notifier).delete(id);
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(jobExperienceViewModelProvider);
    final experiencesAsync = ref.watch(jobExperienceListProvider);

    ref.listen<JobExperienceState>(jobExperienceViewModelProvider,
        (prev, next) {
      if (!mounted) return;
      if (next.justAdded && !(prev?.justAdded ?? false)) {
        _resetAddForm();
        showProfileSuccess(context, 'Work experience added successfully!');
      }
      if (next.addError != null && next.addError != prev?.addError) {
        showProfileError(context, next.addError!);
      }
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Work experience updated successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
      if (next.justDeleted && !(prev?.justDeleted ?? false)) {
        showProfileSuccess(context, 'Work experience deleted.');
      }
      if (next.deleteError != null && next.deleteError != prev?.deleteError) {
        showProfileError(context, next.deleteError!);
      }
      // Populate edit form when editing starts
      if (next.editingId != null && next.editingId != prev?.editingId) {
        final experiences =
            experiencesAsync.asData?.value ?? [];
        final experience = experiences.where((e) => e.id == next.editingId).firstOrNull;
        if (experience != null) _populateEditForm(experience);
      }
    });

    return ProfileSectionCard(
      title: 'Work Experience',
      icon: Icons.work_history_outlined,
      iconGradient: const [Color(0xFF1565C0), Color(0xFF42A5F5)],
      children: [
        experiencesAsync.when(
          data: (experiences) => experiences.isEmpty
              ? const SizedBox.shrink()
              : Column(
                  children: [
                    ...experiences.map((e) {
                      final isEditingThis = vm.editingId == e.id;
                      if (isEditingThis) {
                        return _buildEditForm(e, vm);
                      }
                      return JobExperienceCard(
                        experience: e,
                        isDeleting: vm.deletingId == e.id,
                        onEdit: (vm.editingId == null && vm.deletingId == null)
                            ? () => ref
                                .read(jobExperienceViewModelProvider.notifier)
                                .startEditing(e.id!)
                            : null,
                        onDelete: (vm.editingId == null && vm.deletingId == null)
                            ? () => _confirmDelete(e.id!)
                            : null,
                      );
                    }),
                    const SizedBox(height: 4),
                  ],
                ),
          loading: () => const Padding(
            padding: EdgeInsets.symmetric(vertical: 8),
            child: Center(child: CircularProgressIndicator(strokeWidth: 2)),
          ),
          error: (_, _) => Text(
            'Could not load work experience',
            style: TextStyle(fontSize: 14, color: Colors.grey.shade500),
          ),
        ),
        if (vm.editingId == null) ...[
          if (!vm.showForm)
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: () => ref
                    .read(jobExperienceViewModelProvider.notifier)
                    .startAdding(),
                icon: const Icon(Icons.add, size: 18),
                label: const Text('Add Work Experience'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppTheme.primaryBlue,
                  side: const BorderSide(color: AppTheme.primaryBlue),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
              ),
            )
          else ...[
            const Divider(height: 24),
            _buildAddForm(vm),
          ],
        ],
      ],
    );
  }

  Widget _buildEditForm(JobExperience experience, JobExperienceState vm) {
    final id = experience.id!;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: Text(
            'Edit Experience',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade600,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        TextField(
          controller: _editCompanyController,
          textCapitalization: TextCapitalization.words,
          decoration: InputDecoration(
            labelText: 'Company *',
            prefixIcon: const Icon(Icons.business_outlined, size: 20),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _editSupervisorController,
          textCapitalization: TextCapitalization.words,
          decoration: InputDecoration(
            labelText: 'Supervisor',
            prefixIcon: const Icon(Icons.person_outline, size: 20),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _editDutiesController,
          maxLines: 3,
          textCapitalization: TextCapitalization.sentences,
          decoration: InputDecoration(
            labelText: 'Duties / Responsibilities',
            alignLabelWithHint: true,
            prefixIcon: const Padding(
              padding: EdgeInsets.only(bottom: 44),
              child: Icon(Icons.description_outlined, size: 20),
            ),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        Row(
          children: [
            Expanded(
              child: DatePickerField(
                label: 'Start Date *',
                value: _editStartDate,
                onTap: () => _pickDate(isEdit: true, isStart: true),
              ),
            ),
            if (!_editIsCurrent) ...[
              const SizedBox(width: 12),
              Expanded(
                child: DatePickerField(
                  label: 'End Date',
                  value: _editEndDate,
                  onTap: () => _pickDate(isEdit: true, isStart: false),
                ),
              ),
            ],
          ],
        ),
        const SizedBox(height: 4),
        InkWell(
          onTap: () => setState(() {
            _editIsCurrent = !_editIsCurrent;
            if (_editIsCurrent) _editEndDate = null;
          }),
          borderRadius: BorderRadius.circular(8),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Row(
              children: [
                Checkbox(
                  value: _editIsCurrent,
                  onChanged: (v) => setState(() {
                    _editIsCurrent = v ?? false;
                    if (_editIsCurrent) _editEndDate = null;
                  }),
                  activeColor: AppTheme.primaryBlue,
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
                const Text('Currently working here',
                    style: TextStyle(fontSize: 14)),
              ],
            ),
          ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: ElevatedButton.icon(
                onPressed: vm.isSaving ? null : () => _submitEdit(id),
                icon: vm.isSaving
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.save_outlined, size: 18),
                label: Text(vm.isSaving ? 'Saving...' : 'Save Changes'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12)),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
              ),
            ),
            const SizedBox(width: 8),
            IconButton(
              onPressed: vm.isSaving
                  ? null
                  : () => ref
                      .read(jobExperienceViewModelProvider.notifier)
                      .cancelEditing(),
              icon: Icon(Icons.cancel_outlined,
                  color: Colors.grey.shade500, size: 22),
              tooltip: 'Cancel',
            ),
          ],
        ),
        const SizedBox(height: 12),
      ],
    );
  }

  Widget _buildAddForm(JobExperienceState vm) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: Text(
            'New Experience',
            style: TextStyle(
              fontSize: 12,
              color: Colors.grey.shade600,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        TextField(
          controller: _companyController,
          textCapitalization: TextCapitalization.words,
          decoration: InputDecoration(
            labelText: 'Company *',
            prefixIcon: const Icon(Icons.business_outlined, size: 20),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _supervisorController,
          textCapitalization: TextCapitalization.words,
          decoration: InputDecoration(
            labelText: 'Supervisor',
            prefixIcon: const Icon(Icons.person_outline, size: 20),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _dutiesController,
          maxLines: 3,
          textCapitalization: TextCapitalization.sentences,
          decoration: InputDecoration(
            labelText: 'Duties / Responsibilities',
            alignLabelWithHint: true,
            prefixIcon: const Padding(
              padding: EdgeInsets.only(bottom: 44),
              child: Icon(Icons.description_outlined, size: 20),
            ),
            border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12)),
            contentPadding:
                const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            isDense: true,
          ),
        ),
        const SizedBox(height: 10),
        Row(
          children: [
            Expanded(
              child: DatePickerField(
                label: 'Start Date *',
                value: _startDate,
                onTap: () => _pickDate(isEdit: false, isStart: true),
              ),
            ),
            if (!_isCurrent) ...[
              const SizedBox(width: 12),
              Expanded(
                child: DatePickerField(
                  label: 'End Date',
                  value: _endDate,
                  onTap: () => _pickDate(isEdit: false, isStart: false),
                ),
              ),
            ],
          ],
        ),
        const SizedBox(height: 4),
        InkWell(
          onTap: () => setState(() {
            _isCurrent = !_isCurrent;
            if (_isCurrent) _endDate = null;
          }),
          borderRadius: BorderRadius.circular(8),
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 4),
            child: Row(
              children: [
                Checkbox(
                  value: _isCurrent,
                  onChanged: (v) => setState(() {
                    _isCurrent = v ?? false;
                    if (_isCurrent) _endDate = null;
                  }),
                  activeColor: AppTheme.primaryBlue,
                  materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                ),
                const Text('Currently working here',
                    style: TextStyle(fontSize: 14)),
              ],
            ),
          ),
        ),
        const SizedBox(height: 12),
        Row(
          children: [
            Expanded(
              child: ElevatedButton.icon(
                onPressed: vm.isAdding ? null : _submitAdd,
                icon: vm.isAdding
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.save_outlined, size: 18),
                label: Text(vm.isAdding ? 'Saving...' : 'Save'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.primaryBlue,
                  foregroundColor: Colors.white,
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12)),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
              ),
            ),
            const SizedBox(width: 8),
            IconButton(
              onPressed: vm.isAdding
                  ? null
                  : () {
                      _resetAddForm();
                      ref
                          .read(jobExperienceViewModelProvider.notifier)
                          .cancelAdding();
                    },
              icon: Icon(Icons.cancel_outlined,
                  color: Colors.grey.shade500, size: 22),
              tooltip: 'Cancel',
            ),
          ],
        ),
      ],
    );
  }
}
