import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/display/profile_info_row.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../../catalog/domain/entities/catalog_item.dart';
import '../../../../documents/presentation/viewmodels/documents_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/document_file_row.dart';
import '../../../../documents/presentation/widgets/new_document_slot.dart';
import '../../../widgets/section_edit_actions.dart';

class DocumentsSectionCard extends ConsumerStatefulWidget {
  const DocumentsSectionCard({super.key});

  @override
  ConsumerState<DocumentsSectionCard> createState() =>
      _DocumentsSectionCardState();
}

class _DocumentsSectionCardState extends ConsumerState<DocumentsSectionCard> {
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

  @override
  void dispose() {
    _idNumber1Controller.dispose();
    _idNumber2Controller.dispose();
    super.dispose();
  }

  void _populateFields() {
    final profile = ref.read(cachedWorkerProfileProvider).asData?.value;
    _idNumber1Controller.text = profile?.identificationNumber1 ?? '';
    _idNumber2Controller.text = profile?.identificationNumber2 ?? '';
    setState(() {
      _deleteId1File = false;
      _deleteId2File = false;
      _replaceId1File = null;
      _replaceId2File = null;
      _pendingId1Type = null;
      _pendingId1Number = '';
      _pendingId2Type = null;
      _pendingId2Number = '';
    });
  }

  Future<PickedFileData?> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png']);
    return result.isSuccess ? result.file : null;
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
    final vm = ref.watch(documentsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(
      documentsViewModelProvider.select((s) => s.isEditing),
      (prev, next) {
        if (prev == false && next == true) _populateFields();
      },
    );

    ref.listen<DocumentsState>(documentsViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justSaved && !(prev?.justSaved ?? false)) {
        showProfileSuccess(context, 'Changes saved successfully!');
      }
      if (next.saveError != null && next.saveError != prev?.saveError) {
        showProfileError(context, next.saveError!);
      }
    });

    return ProfileSectionCard(
      title: 'Documents',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFF9C27B0), Color(0xFFBA68C8)],
      trailing: SectionEditActions(
        isEditingThis: vm.isEditing,
        isAnyEditing: false,
        isSaving: vm.isSaving,
        onEdit: profile != null
            ? ref.read(documentsViewModelProvider.notifier).startEditing
            : null,
        onCancel: ref.read(documentsViewModelProvider.notifier).cancelEditing,
        onSave: () => ref.read(documentsViewModelProvider.notifier).save(
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
            value: vm.isEditing
                ? (profile.identificationNumber1 ?? '')
                : profile.maskedIdNumber1,
            icon: Icons.credit_card_outlined,
            isEditing: vm.isEditing,
            controller: vm.isEditing ? _idNumber1Controller : null,
          ),
          DocumentFileRow(
            label: '${profile.identificationType1!} (File)',
            fileName: profile.identificationType1FileName,
            fileUrl: profile.identificationType1FileUrl,
            isMarkedForDeletion: _deleteId1File,
            pendingFile: _replaceId1File,
            isEditing: vm.isEditing,
            onDelete: () => setState(() {
              _deleteId1File = true;
              _replaceId1File = null;
            }),
            onUndo: () => setState(() => _deleteId1File = false),
            onPickFile: () async {
              final file = await _pickFile();
              if (file != null) {
                setState(() {
                  _replaceId1File = file;
                  _deleteId1File = false;
                });
              }
            },
            onClearPick: () => setState(() => _replaceId1File = null),
            onPreview: profile.identificationType1FileUrl != null
                ? () => _previewDocument(
                    profile.identificationType1FileUrl!,
                    '${profile.identificationType1} File')
                : null,
          ),
        ],
        if (profile?.identificationType2 != null) ...[
          ProfileInfoRow(
            label: '${profile!.identificationType2!} #',
            value: vm.isEditing
                ? (profile.identificationNumber2 ?? '')
                : profile.maskedIdNumber2,
            icon: Icons.credit_card_outlined,
            isEditing: vm.isEditing,
            controller: vm.isEditing ? _idNumber2Controller : null,
          ),
          DocumentFileRow(
            label: '${profile.identificationType2!} (File)',
            fileName: profile.identificationType2FileName,
            fileUrl: profile.identificationType2FileUrl,
            isMarkedForDeletion: _deleteId2File,
            pendingFile: _replaceId2File,
            isEditing: vm.isEditing,
            onDelete: () => setState(() {
              _deleteId2File = true;
              _replaceId2File = null;
            }),
            onUndo: () => setState(() => _deleteId2File = false),
            onPickFile: () async {
              final file = await _pickFile();
              if (file != null) {
                setState(() {
                  _replaceId2File = file;
                  _deleteId2File = false;
                });
              }
            },
            onClearPick: () => setState(() => _replaceId2File = null),
            onPreview: profile.identificationType2FileUrl != null
                ? () => _previewDocument(
                    profile.identificationType2FileUrl!,
                    '${profile.identificationType2} File')
                : null,
          ),
        ],
        if (profile?.identificationType1 == null &&
            profile?.identificationType2 == null) ...[
          if (vm.isEditing) ...[
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
