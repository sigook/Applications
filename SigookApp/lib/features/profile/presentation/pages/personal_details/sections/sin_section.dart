import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../sin/presentation/viewmodels/sin_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/document_file_row.dart';
import '../../../widgets/section_edit_actions.dart';

class SinSectionCard extends ConsumerStatefulWidget {
  const SinSectionCard({super.key});

  @override
  ConsumerState<SinSectionCard> createState() => _SinSectionCardState();
}

class _SinSectionCardState extends ConsumerState<SinSectionCard> {
  final _sinController = TextEditingController();
  bool _deleteSinFile = false;
  PickedFileData? _replaceSinFile;
  bool _expiresSin = false;
  DateTime? _dueDate;

  @override
  void dispose() {
    _sinController.dispose();
    super.dispose();
  }

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    _sinController.text = profile?.socialInsurance ?? '';
    DateTime? parsed;
    if (profile?.dueDate != null) {
      parsed = DateTime.tryParse(profile!.dueDate!);
    }
    setState(() {
      _deleteSinFile = false;
      _replaceSinFile = null;
      _expiresSin = profile?.socialInsuranceExpire ?? false;
      _dueDate = parsed;
    });
  }

  Future<void> _pickDueDate() async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: _dueDate ?? now,
      firstDate: now,
      lastDate: DateTime(now.year + 20),
    );
    if (picked != null) setState(() => _dueDate = picked);
  }

  Future<void> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png']);
    if (!result.isSuccess || result.file == null) return;
    setState(() {
      _replaceSinFile = result.file;
      _deleteSinFile = false;
    });
  }

  void _previewDocument(String url, String title) {
    final token = ref.read(authViewModelProvider).token?.accessToken;
    Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) =>
            DocumentPreviewPage(url: url, title: title, token: token),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(sinViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      sinViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen<SinState>(sinViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    return ProfileSectionCard(
      title: 'SIN / SSN',
      icon: Icons.security_outlined,
      iconGradient: const [Color(0xFFFF9800), Color(0xFFFFB74D)],
      trailing: SectionEditActions(
        isEditingThis: vm.isEditing,
        isAnyEditing: false,
        isSaving: vm.isSaving,
        onEdit: profile != null
            ? ref.read(sinViewModelProvider.notifier).startEditing
            : null,
        onCancel: ref.read(sinViewModelProvider.notifier).cancelEditing,
        onSave: () => ref.read(sinViewModelProvider.notifier).save(
          {
            'socialInsurance': _sinController.text,
            'socialInsuranceExpire': _expiresSin.toString(),
            if (_expiresSin && _dueDate != null)
              'dueDate': _dueDate!.toIso8601String(),
            if (_deleteSinFile) '_deleteSinFile': 'true',
          },
          filePath: _replaceSinFile?.path,
        ),
      ),
      children: [
        ProfileInfoRow(
          label: 'SIN / SSN #',
          value: vm.isEditing
              ? (profile?.socialInsurance ?? '')
              : profile?.maskedSocialInsurance ?? 'N/A',
          icon: Icons.lock_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _sinController : null,
        ),
        if (!vm.isEditing) ...[
          ProfileInfoRow(
            label: 'Expires',
            value: profile?.socialInsuranceExpire == true ? 'Yes' : 'No',
            icon: Icons.event_busy_outlined,
          ),
          if (profile?.socialInsuranceExpire == true)
            ProfileInfoRow(
              label: 'Due Date',
              value: profile?.formattedDueDate ?? 'N/A',
              icon: Icons.calendar_today_outlined,
            ),
        ],
        if (vm.isEditing) ...[
          SwitchListTile(
            contentPadding: const EdgeInsets.symmetric(horizontal: 4),
            title: Row(
              children: [
                const Icon(Icons.event_busy_outlined, size: 18, color: Colors.grey),
                const SizedBox(width: 8),
                const Text('Expires', style: TextStyle(fontSize: 14)),
              ],
            ),
            value: _expiresSin,
            onChanged: (v) => setState(() {
              _expiresSin = v;
              if (!v) _dueDate = null;
            }),
          ),
          if (_expiresSin)
            ListTile(
              contentPadding: const EdgeInsets.symmetric(horizontal: 4),
              leading: const Icon(Icons.calendar_today_outlined, size: 18, color: Colors.grey),
              title: Text(
                _dueDate != null
                    ? '${_dueDate!.year}-${_dueDate!.month.toString().padLeft(2, '0')}-${_dueDate!.day.toString().padLeft(2, '0')}'
                    : 'Select due date',
                style: TextStyle(
                  fontSize: 14,
                  color: _dueDate != null ? Colors.black87 : Colors.grey,
                ),
              ),
              trailing: const Icon(Icons.edit_calendar_outlined, size: 18),
              onTap: _pickDueDate,
            ),
        ],
        DocumentFileRow(
          label: 'File',
          fileName: profile?.socialInsuranceFileName,
          fileUrl: profile?.socialInsuranceFileUrl,
          isMarkedForDeletion: _deleteSinFile,
          pendingFile: _replaceSinFile,
          isEditing: vm.isEditing,
          onDelete: () => setState(() {
            _deleteSinFile = true;
            _replaceSinFile = null;
          }),
          onUndo: () => setState(() => _deleteSinFile = false),
          onPickFile: _pickFile,
          onClearPick: () => setState(() => _replaceSinFile = null),
          onPreview: profile?.socialInsuranceFileUrl != null
              ? () =>
                  _previewDocument(profile!.socialInsuranceFileUrl!, 'SIN File')
              : null,
        ),
      ],
    );
  }
}
