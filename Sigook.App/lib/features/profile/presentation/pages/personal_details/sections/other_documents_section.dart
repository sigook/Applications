import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../other_documents/presentation/viewmodels/other_documents_viewmodel.dart';
import '../../../../other_documents/presentation/widgets/other_document_card.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../widgets/document_description_field.dart';
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class OtherDocumentsSectionCard extends ConsumerStatefulWidget {
  const OtherDocumentsSectionCard({super.key});

  @override
  ConsumerState<OtherDocumentsSectionCard> createState() =>
      _OtherDocumentsSectionCardState();
}

class _OtherDocumentsSectionCardState
    extends ConsumerState<OtherDocumentsSectionCard> {
  static const _whmisTrainingUrl =
      'https://aixsafety.com/wp-content/uploads/articulate_uploads/WHS-Apr2025Aix/story.html';
  static const _healthAndSafetyUrl =
      'https://www.labour.gov.on.ca/english/hs/elearn/worker/foursteps.php';

  PickedFileData? _pendingFile;
  final _descriptionController = TextEditingController();
  String? _descriptionError;

  @override
  void dispose() {
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _confirmDelete(String documentId) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Document'),
        content: const Text('Are you sure you want to delete this document? This action cannot be undone.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
    if (confirmed == true && mounted) {
      ref.read(otherDocumentsViewModelProvider.notifier).delete(documentId);
    }
  }

  void _openPage(String url, String title, {String? token}) {
    Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => DocumentPreviewPage(url: url, title: title, token: token),
      ),
    );
  }

  void _previewDocument(String url, String title) {
    _openPage(url, title, token: ref.read(authViewModelProvider).token?.accessToken);
  }

  Future<void> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: FilePickerService.documentExtensions);
    if (!result.isSuccess || result.file == null) return;
    setState(() => _pendingFile = result.file);
  }

  Future<void> _upload() async {
    if (_pendingFile == null) return;
    final error = DocumentDescriptionField.validate(_descriptionController.text);
    setState(() => _descriptionError = error);
    if (error != null) return;
    await ref.read(otherDocumentsViewModelProvider.notifier).upload(
      filePath: _pendingFile!.path,
      description: _descriptionController.text.trim(),
    );
  }

  void _reset() {
    setState(() {
      _pendingFile = null;
      _descriptionController.clear();
      _descriptionError = null;
    });
  }

  Widget _trainingLink(String label, String url) {
    return TextButton.icon(
      onPressed: () => _openPage(url, label),
      icon: const Icon(Icons.open_in_new, size: 16),
      label: Text(label),
      style: TextButton.styleFrom(
        foregroundColor: AppTheme.primaryBlue,
        padding: const EdgeInsets.symmetric(horizontal: 8),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(otherDocumentsViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen<OtherDocumentsState>(otherDocumentsViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justUploaded && !(prev?.justUploaded ?? false)) {
        _reset();
        showProfileSuccess(context, 'Document uploaded successfully!');
      }
      if (next.uploadError != null && next.uploadError != prev?.uploadError) {
        showProfileError(context, 'Failed to upload document: ${next.uploadError}');
      }
      if (next.justDeleted && !(prev?.justDeleted ?? false)) {
        showProfileSuccess(context, 'Document deleted successfully!');
      }
      if (next.deleteError != null && next.deleteError != prev?.deleteError) {
        showProfileError(context, 'Failed to delete document: ${next.deleteError}');
      }
    });

    if (profile == null ||
        (!profile.isCanada && profile.otherDocuments.isEmpty)) {
      return const SizedBox.shrink();
    }

    return ProfileSectionCard(
      title: 'WHMIS and Health and Safety Training',
      icon: Icons.health_and_safety_outlined,
      iconGradient: const [Color(0xFF00695C), Color(0xFF26A69A)],
      children: [
        if (profile.isCanada) ...[
          Text(
            'Complete the training following both links below and upload your certificates',
            style: TextStyle(fontSize: 13, color: Colors.grey.shade600),
          ),
          const SizedBox(height: 4),
          Wrap(
            spacing: 8,
            children: [
              _trainingLink('WHMIS Training', _whmisTrainingUrl),
              _trainingLink('HS Booklet', _healthAndSafetyUrl),
            ],
          ),
          const SizedBox(height: 8),
        ],
        if (profile.otherDocuments.isNotEmpty) ...[
          ...profile.otherDocuments.map(
            (doc) => OtherDocumentCard(
              document: doc,
              onPreview: doc.fileUrl != null
                  ? () => _previewDocument(
                      doc.fileUrl!, doc.description ?? 'Document')
                  : null,
              onDelete: doc.id != null ? () => _confirmDelete(doc.id!) : null,
            ),
          ),
          const SizedBox(height: 4),
        ],
        if (_pendingFile != null) ...[
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: PendingFileRow(fileName: _pendingFile!.name),
          ),
          DocumentDescriptionField(
            controller: _descriptionController,
            errorText: _descriptionError,
            onChanged: (_) {
              if (_descriptionError != null) {
                setState(() => _descriptionError = null);
              }
            },
          ),
          const SizedBox(height: 12),
          UploadActionRow(
            isUploading: vm.isUploading,
            label: 'Upload Document',
            onUpload: _upload,
            onCancel: _reset,
          ),
        ] else
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: _pickFile,
              icon: const Icon(Icons.upload_file, size: 18),
              label: const Text('Add Document'),
              style: OutlinedButton.styleFrom(
                foregroundColor: AppTheme.primaryBlue,
                side: const BorderSide(color: AppTheme.primaryBlue),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                padding: const EdgeInsets.symmetric(vertical: 12),
              ),
            ),
          ),
      ],
    );
  }
}
