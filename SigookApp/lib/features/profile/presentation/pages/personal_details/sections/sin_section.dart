import 'package:flutter/material.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../widgets/document_file_row.dart';
import '../../../widgets/section_edit_actions.dart';

class SinSectionCard extends StatefulWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final VoidCallback? onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) onSave;
  final VoidCallback onCancel;
  final void Function(String url, String title) onPreviewDocument;
  final Future<PickedFileData?> Function() onPickFile;

  const SinSectionCard({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
    required this.onPreviewDocument,
    required this.onPickFile,
  });

  @override
  State<SinSectionCard> createState() => _SinSectionCardState();
}

class _SinSectionCardState extends State<SinSectionCard> {
  final _sinController = TextEditingController();
  bool _deleteSinFile = false;
  PickedFileData? _replaceSinFile;

  bool get _isEditing => widget.editingSection == ProfileSection.sin;

  @override
  void didUpdateWidget(SinSectionCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    final wasEditing = oldWidget.editingSection == ProfileSection.sin;
    if (!wasEditing && _isEditing) {
      _sinController.text = widget.profile?.socialInsurance ?? '';
      _deleteSinFile = false;
      _replaceSinFile = null;
    }
  }

  @override
  void dispose() {
    _sinController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

    return ProfileSectionCard(
      title: 'SIN / SSN',
      icon: Icons.security_outlined,
      iconGradient: const [Color(0xFFFF9800), Color(0xFFFFB74D)],
      trailing: SectionEditActions(
        isEditingThis: _isEditing,
        isAnyEditing: widget.editingSection != null,
        isSaving: widget.isSaving,
        onEdit: widget.onEdit,
        onCancel: widget.onCancel,
        onSave: () => widget.onSave(
          ProfileSection.sin,
          {
            'socialInsurance': _sinController.text,
            if (_deleteSinFile) '_deleteSinFile': 'true',
          },
          filePaths: {
            if (_replaceSinFile != null) 'sinFile': _replaceSinFile!.path,
          },
        ),
      ),
      children: [
        ProfileInfoRow(
          label: 'SIN / SSN #',
          value: _isEditing
              ? (profile?.socialInsurance ?? '')
              : profile?.maskedSocialInsurance ?? 'N/A',
          icon: Icons.lock_outlined,
          isEditing: _isEditing,
          controller: _isEditing ? _sinController : null,
        ),
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
        DocumentFileRow(
          label: 'File',
          fileName: profile?.socialInsuranceFileName,
          fileUrl: profile?.socialInsuranceFileUrl,
          isMarkedForDeletion: _deleteSinFile,
          pendingFile: _replaceSinFile,
          isEditing: _isEditing,
          onDelete: () => setState(() {
            _deleteSinFile = true;
            _replaceSinFile = null;
          }),
          onUndo: () => setState(() => _deleteSinFile = false),
          onPickFile: () async {
            final file = await widget.onPickFile();
            if (file != null) {
              setState(() {
                _replaceSinFile = file;
                _deleteSinFile = false;
              });
            }
          },
          onClearPick: () => setState(() => _replaceSinFile = null),
          onPreview: profile?.socialInsuranceFileUrl != null
              ? () => widget.onPreviewDocument(
                  profile!.socialInsuranceFileUrl!, 'SIN File')
              : null,
        ),
      ],
    );
  }
}
