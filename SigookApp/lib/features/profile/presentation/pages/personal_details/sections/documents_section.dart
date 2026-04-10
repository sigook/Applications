import 'package:flutter/material.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../catalog/domain/entities/catalog_item.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../widgets/document_file_row.dart';
import '../../../widgets/new_document_slot.dart';
import '../../../widgets/section_edit_actions.dart';

class DocumentsSectionCard extends StatefulWidget {
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

  const DocumentsSectionCard({
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
  State<DocumentsSectionCard> createState() => _DocumentsSectionCardState();
}

class _DocumentsSectionCardState extends State<DocumentsSectionCard> {
  final _idNumber1Controller = TextEditingController();
  final _idNumber2Controller = TextEditingController();
  bool _deleteId1File = false;
  bool _deleteId2File = false;
  PickedFileData? _replaceId1File;
  PickedFileData? _replaceId2File;
  CatalogItem? _pendingId1Type;
  String _pendingId1Number = '';
  CatalogItem? _pendingId2Type;
  String _pendingId2Number = '';

  bool get _isEditing => widget.editingSection == ProfileSection.documents;

  @override
  void didUpdateWidget(DocumentsSectionCard oldWidget) {
    super.didUpdateWidget(oldWidget);
    final wasEditing = oldWidget.editingSection == ProfileSection.documents;
    if (!wasEditing && _isEditing) {
      _idNumber1Controller.text = widget.profile?.identificationNumber1 ?? '';
      _idNumber2Controller.text = widget.profile?.identificationNumber2 ?? '';
      _deleteId1File = false;
      _deleteId2File = false;
      _replaceId1File = null;
      _replaceId2File = null;
      _pendingId1Type = null;
      _pendingId1Number = '';
      _pendingId2Type = null;
      _pendingId2Number = '';
    }
  }

  @override
  void dispose() {
    _idNumber1Controller.dispose();
    _idNumber2Controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

    return ProfileSectionCard(
      title: 'Documents',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFF9C27B0), Color(0xFFBA68C8)],
      trailing: SectionEditActions(
        isEditingThis: _isEditing,
        isAnyEditing: widget.editingSection != null,
        isSaving: widget.isSaving,
        onEdit: widget.onEdit,
        onCancel: widget.onCancel,
        onSave: () => widget.onSave(
          ProfileSection.documents,
          {
            'identificationNumber1': _pendingId1Type != null
                ? _pendingId1Number
                : _idNumber1Controller.text,
            'identificationNumber2': _pendingId2Type != null
                ? _pendingId2Number
                : _idNumber2Controller.text,
            if (_pendingId1Type?.id != null)
              'identificationType1Id': _pendingId1Type!.id!,
            if (_pendingId1Type != null)
              'identificationType1Value': _pendingId1Type!.value,
            if (_pendingId2Type?.id != null)
              'identificationType2Id': _pendingId2Type!.id!,
            if (_pendingId2Type != null)
              'identificationType2Value': _pendingId2Type!.value,
            if (_deleteId1File) '_deleteId1File': 'true',
            if (_deleteId2File) '_deleteId2File': 'true',
          },
          filePaths: {
            if (_replaceId1File != null) 'id1File': _replaceId1File!.path,
            if (_replaceId2File != null) 'id2File': _replaceId2File!.path,
          },
        ),
      ),
      children: [
        if (profile?.identificationType1 != null) ...[
          ProfileInfoRow(
            label: '${profile!.identificationType1!} #',
            value: _isEditing
                ? (profile.identificationNumber1 ?? '')
                : profile.maskedIdNumber1,
            icon: Icons.credit_card_outlined,
            isEditing: _isEditing,
            controller: _isEditing ? _idNumber1Controller : null,
          ),
          DocumentFileRow(
            label: '${profile.identificationType1!} (File)',
            fileName: profile.identificationType1FileName,
            fileUrl: profile.identificationType1FileUrl,
            isMarkedForDeletion: _deleteId1File,
            pendingFile: _replaceId1File,
            isEditing: _isEditing,
            onDelete: () => setState(() {
              _deleteId1File = true;
              _replaceId1File = null;
            }),
            onUndo: () => setState(() => _deleteId1File = false),
            onPickFile: () async {
              final file = await widget.onPickFile();
              if (file != null) {
                setState(() {
                  _replaceId1File = file;
                  _deleteId1File = false;
                });
              }
            },
            onClearPick: () => setState(() => _replaceId1File = null),
            onPreview: profile.identificationType1FileUrl != null
                ? () => widget.onPreviewDocument(
                    profile.identificationType1FileUrl!,
                    '${profile.identificationType1} File')
                : null,
          ),
        ],
        if (profile?.identificationType2 != null) ...[
          ProfileInfoRow(
            label: '${profile!.identificationType2!} #',
            value: _isEditing
                ? (profile.identificationNumber2 ?? '')
                : profile.maskedIdNumber2,
            icon: Icons.credit_card_outlined,
            isEditing: _isEditing,
            controller: _isEditing ? _idNumber2Controller : null,
          ),
          DocumentFileRow(
            label: '${profile.identificationType2!} (File)',
            fileName: profile.identificationType2FileName,
            fileUrl: profile.identificationType2FileUrl,
            isMarkedForDeletion: _deleteId2File,
            pendingFile: _replaceId2File,
            isEditing: _isEditing,
            onDelete: () => setState(() {
              _deleteId2File = true;
              _replaceId2File = null;
            }),
            onUndo: () => setState(() => _deleteId2File = false),
            onPickFile: () async {
              final file = await widget.onPickFile();
              if (file != null) {
                setState(() {
                  _replaceId2File = file;
                  _deleteId2File = false;
                });
              }
            },
            onClearPick: () => setState(() => _replaceId2File = null),
            onPreview: profile.identificationType2FileUrl != null
                ? () => widget.onPreviewDocument(
                    profile.identificationType2FileUrl!,
                    '${profile.identificationType2} File')
                : null,
          ),
        ],
        if (profile?.identificationType1 == null &&
            profile?.identificationType2 == null) ...[
          if (_isEditing) ...[
            NewDocumentSlot(
              docType: 'id1File',
              pendingType: _pendingId1Type,
              pendingFile: _replaceId1File,
              onUndo: () => setState(() {
                _pendingId1Type = null;
                _pendingId1Number = '';
                _replaceId1File = null;
              }),
              onDocumentPicked: (type, number, file) => setState(() {
                _pendingId1Type = type;
                _pendingId1Number = number;
                _replaceId1File = file;
                _deleteId1File = false;
              }),
            ),
            NewDocumentSlot(
              docType: 'id2File',
              pendingType: _pendingId2Type,
              pendingFile: _replaceId2File,
              onUndo: () => setState(() {
                _pendingId2Type = null;
                _pendingId2Number = '';
                _replaceId2File = null;
              }),
              onDocumentPicked: (type, number, file) => setState(() {
                _pendingId2Type = type;
                _pendingId2Number = number;
                _replaceId2File = file;
                _deleteId2File = false;
              }),
            ),
          ] else
            Text(
              'No documents on file',
              style: TextStyle(fontSize: 14, color: Colors.grey.shade500),
            ),
        ],
      ],
    );
  }
}
