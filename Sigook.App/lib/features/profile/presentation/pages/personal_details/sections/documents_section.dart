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
import '../../../../../catalog/presentation/providers/catalog_providers.dart';
import '../../../../../registration/domain/entities/value_objects/identification_number.dart';
import '../../../../documents/presentation/viewmodels/documents_viewmodel.dart';
import '../../../../domain/entities/worker_profile.dart';
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
  String? _idNumber1Error;
  String? _idNumber2Error;

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
      _idNumber1Error = null;
      _idNumber2Error = null;
    });
  }

  Future<PickedFileData?> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: FilePickerService.documentExtensions);
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
    ref.watch(identificationTypesListProvider);
    final vm = ref.watch(documentsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen(documentsViewModelProvider.select((s) => s.isEditing), (
      prev,
      next,
    ) {
      if (prev == false && next == true) _populateFields();
    });

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
        onSave: () => _save(profile),
      ),
      children: _buildChildren(profile, vm),
    );
  }

  String? _effectiveTypeId(CatalogItem? pending, String? existing) =>
      pending?.id ?? existing;

  void _save(WorkerProfile? profile) {
    final keepsType1 =
        profile?.identificationType1 != null && _pendingId1Type == null;
    final keepsType2 =
        profile?.identificationType2 != null && _pendingId2Type == null;
    final types = ref.read(identificationTypesListProvider).asData?.value ?? [];
    int? codeOf(String? typeId) =>
        types.where((t) => t.id == typeId).firstOrNull?.code;
    setState(() {
      _idNumber1Error = keepsType1
          ? IdentificationNumber.validate(
              _idNumber1Controller.text,
              typeCode: codeOf(profile?.identificationType1Id),
            )
          : null;
      _idNumber2Error = keepsType2
          ? IdentificationNumber.validate(
              _idNumber2Controller.text,
              typeCode: codeOf(profile?.identificationType2Id),
            )
          : null;
    });
    if (_idNumber1Error != null || _idNumber2Error != null) return;
    final type1Id = _effectiveTypeId(
      _pendingId1Type,
      profile?.identificationType1Id,
    );
    final type2Id = _effectiveTypeId(
      _pendingId2Type,
      profile?.identificationType2Id,
    );
    if (type1Id != null && type1Id == type2Id) {
      showProfileError(
        context,
        'Both identification documents cannot be the same type',
      );
      return;
    }
    ref
        .read(documentsViewModelProvider.notifier)
        .save(
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
        );
  }

  Widget _newSlot({
    required String docType,
    required CatalogItem? pendingType,
    required PickedFileData? pendingFile,
    required String? excludedTypeId,
    required bool isReplacement,
    required void Function(CatalogItem?, String, PickedFileData?) apply,
  }) {
    return NewDocumentSlot(
      docType: docType,
      pendingType: pendingType,
      pendingFile: pendingFile,
      excludedTypeId: excludedTypeId,
      isReplacement: isReplacement,
      onUndo: () => setState(() => apply(null, '', null)),
      onDocumentPicked: (type, number, file) =>
          setState(() => apply(type, number, file)),
    );
  }

  List<Widget> _buildChildren(WorkerProfile? profile, DocumentsState vm) {
    final hasType1 = profile?.identificationType1 != null;
    final hasType2 = profile?.identificationType2 != null;
    final type1Id = _effectiveTypeId(
      _pendingId1Type,
      profile?.identificationType1Id,
    );
    final type2Id = _effectiveTypeId(
      _pendingId2Type,
      profile?.identificationType2Id,
    );

    Widget slot1() => _newSlot(
      docType: 'id1File',
      pendingType: _pendingId1Type,
      pendingFile: _pendingId1Type != null ? _replaceId1File : null,
      excludedTypeId: type2Id,
      isReplacement: hasType1,
      apply: (type, number, file) {
        _pendingId1Type = type;
        _pendingId1Number = number;
        _replaceId1File = file;
        _deleteId1File = false;
        _idNumber1Error = null;
      },
    );

    Widget slot2() => _newSlot(
      docType: 'id2File',
      pendingType: _pendingId2Type,
      pendingFile: _pendingId2Type != null ? _replaceId2File : null,
      excludedTypeId: type1Id,
      isReplacement: hasType2,
      apply: (type, number, file) {
        _pendingId2Type = type;
        _pendingId2Number = number;
        _replaceId2File = file;
        _deleteId2File = false;
        _idNumber2Error = null;
      },
    );

    return [
      if (vm.isEditing && _pendingId1Type != null)
        slot1()
      else if (hasType1) ...[
        ProfileInfoRow(
          label: '${profile!.identificationType1!} #',
          value: vm.isEditing
              ? (profile.identificationNumber1 ?? '')
              : profile.maskedIdNumber1,
          icon: Icons.credit_card_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _idNumber1Controller : null,
          errorText: _idNumber1Error,
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
                  '${profile.identificationType1} File',
                )
              : null,
        ),
        if (vm.isEditing) slot1(),
      ] else if (vm.isEditing)
        slot1(),
      if (vm.isEditing && _pendingId2Type != null)
        slot2()
      else if (hasType2) ...[
        ProfileInfoRow(
          label: '${profile!.identificationType2!} #',
          value: vm.isEditing
              ? (profile.identificationNumber2 ?? '')
              : profile.maskedIdNumber2,
          icon: Icons.credit_card_outlined,
          isEditing: vm.isEditing,
          controller: vm.isEditing ? _idNumber2Controller : null,
          errorText: _idNumber2Error,
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
                  '${profile.identificationType2} File',
                )
              : null,
        ),
        if (vm.isEditing) slot2(),
      ] else if (vm.isEditing)
        slot2(),
      if (!vm.isEditing && !hasType1 && !hasType2)
        Text(
          'No documents on file',
          style: TextStyle(fontSize: 14, color: Colors.grey.shade500),
        ),
    ];
  }
}
