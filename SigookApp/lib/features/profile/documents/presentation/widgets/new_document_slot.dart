import 'package:flutter/material.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../../../core/services/file_picker_service.dart';
import '../../../../catalog/domain/entities/catalog_item.dart';
import '../../../../registration/presentation/widgets/file_upload_modal.dart';

/// A slot for adding a new identity document when none is set by the agency.
/// Shows a pending preview when [pendingType] and [pendingFile] are set,
/// otherwise shows an "Add" button that opens the upload modal.
class NewDocumentSlot extends StatelessWidget {
  final String docType;
  final CatalogItem? pendingType;
  final PickedFileData? pendingFile;
  final VoidCallback onUndo;
  final void Function(CatalogItem type, String number, PickedFileData file)
  onDocumentPicked;

  const NewDocumentSlot({
    super.key,
    required this.docType,
    required this.pendingType,
    required this.pendingFile,
    required this.onUndo,
    required this.onDocumentPicked,
  });

  String get _buttonLabel =>
      docType == 'id1File' ? 'Add Document 1' : 'Add Document 2';

  Future<void> _showModal(BuildContext context) async {
    final result = await showDialog<Map<String, dynamic>>(
      context: context,
      builder: (_) => const FileUploadModal(
        title: 'Add Document',
        description:
            'Select identification type, enter the number, and upload the file.',
      ),
    );
    if (result == null) return;

    final type = result['identificationType'] as CatalogItem;
    final number = result['identificationNumber'] as String;
    final filePath = result['filePath'] as String;
    final fileName = result['file'] as String;
    final fileSize = result['fileSize'] as int;

    onDocumentPicked(
      type,
      number,
      PickedFileData(name: fileName, path: filePath, size: fileSize),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (pendingType != null && pendingFile != null) {
      return Padding(
        padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 4),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    pendingType!.value,
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                      color: AppTheme.primaryBlue,
                    ),
                  ),
                  Text(
                    pendingFile!.name,
                    style: const TextStyle(
                      fontSize: 13,
                      color: AppTheme.primaryBlue,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                  const Text(
                    'Will upload on save',
                    style: TextStyle(fontSize: 12, color: Colors.grey),
                  ),
                ],
              ),
            ),
            TextButton.icon(
              onPressed: onUndo,
              icon: const Icon(Icons.undo, size: 16),
              label: const Text('Undo'),
              style: TextButton.styleFrom(
                foregroundColor: AppTheme.primaryBlue,
                padding: const EdgeInsets.symmetric(horizontal: 8),
              ),
            ),
          ],
        ),
      );
    }

    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: OutlinedButton.icon(
        onPressed: () => _showModal(context),
        icon: const Icon(Icons.add, size: 18),
        label: Text(_buttonLabel),
        style: OutlinedButton.styleFrom(
          foregroundColor: AppTheme.primaryBlue,
          side: BorderSide(color: AppTheme.primaryBlue.withValues(alpha: 0.5)),
        ),
      ),
    );
  }
}
