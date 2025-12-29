import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../domain/entities/documents_info.dart';
import '../../domain/entities/uploaded_file.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../providers/registration_providers.dart';
import '../widgets/file_upload_modal.dart';

class DocumentsPage extends ConsumerStatefulWidget {
  const DocumentsPage({super.key});

  @override
  ConsumerState<DocumentsPage> createState() => _DocumentsPageState();
}

class _DocumentsPageState extends ConsumerState<DocumentsPage> {
  IdentificationDocument? _identification1;
  IdentificationDocument? _identification2;
  UploadedFile? _resume;

  @override
  void initState() {
    super.initState();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.documentsInfo != null) {
        final info = form.documentsInfo!;
        _identification1 = info.identification1;
        _identification2 = info.identification2;
        _resume = info.resume;
        setState(() {});
      }
    });
  }

  void _validateAndSave() {
    setState(() {
      final documentsInfo = DocumentsInfo(
        identification1: _identification1,
        identification2: _identification2,
        resume: _resume,
      );

      if (documentsInfo.isValid) {
        ref
            .read(registrationViewModelProvider.notifier)
            .updateDocumentsInfo(documentsInfo);
      }
    });
  }

  Future<void> _showFileUploadModal({
    required String title,
    required String description,
    required Function(
      String fileName,
      CatalogItem identificationType,
      String identificationNumber,
      String filePath,
      int fileSize,
    )
    onFileUploaded,
  }) async {
    final result = await showDialog<Map<String, dynamic>>(
      context: context,
      builder: (context) =>
          FileUploadModal(title: title, description: description),
    );

    if (result != null && mounted) {
      final fileName = result['file'] as String;
      final identificationType = result['identificationType'] as CatalogItem;
      final identificationNumber = result['identificationNumber'] as String;
      final filePath = (result['filePath'] as String?) ?? '';
      final fileSize = (result['fileSize'] as int?) ?? 0;

      onFileUploaded(
        fileName,
        identificationType,
        identificationNumber,
        filePath,
        fileSize,
      );

      _validateAndSave();
    }
  }

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final isMobile = screenWidth < 600;

    return SingleChildScrollView(
      padding: EdgeInsets.all(isMobile ? 12 : 16),
      keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
      child: Card(
        elevation: 0,
        color: Colors.white,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Documents',
                style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Upload your identification and optional resume',
                style: Theme.of(
                  context,
                ).textTheme.bodyMedium?.copyWith(color: Colors.grey.shade600),
              ),
              const SizedBox(height: 32),

              _buildFileUploadSection(
                title: 'Primary Identification *',
                description: 'Upload your primary identification document',
                icon: Icons.badge,
                files: _identification1 != null
                    ? [_identification1!.displayName]
                    : [],
                onUpload: () {
                  _showFileUploadModal(
                    title: 'Upload Primary Identification',
                    description:
                        'Select identification type and upload identification document',
                    onFileUploaded:
                        (
                          fileName,
                          identificationType,
                          identificationNumber,
                          filePath,
                          fileSize,
                        ) {
                          setState(() {
                            _identification1 = IdentificationDocument(
                              identificationTypeId: identificationType.id ?? '',
                              identificationTypeValue: identificationType.value,
                              identificationNumber: identificationNumber,
                              file: UploadedFile(
                                fileName: fileName,
                                description: '',
                                filePath: filePath,
                              ),
                            );
                          });
                          _validateAndSave();
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text(
                                'Identification uploaded: $fileName',
                              ),
                              backgroundColor: Colors.green,
                            ),
                          );
                        },
                  );
                },
                onRemove: (index) {
                  setState(() {
                    _identification1 = null;
                  });
                  _validateAndSave();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Primary identification removed'),
                      backgroundColor: Colors.orange,
                    ),
                  );
                },
              ),
              const SizedBox(height: 24),

              _buildFileUploadSection(
                title: 'Secondary Identification',
                description: 'Upload secondary identification (optional)',
                icon: Icons.description,
                files: _identification2 != null
                    ? [_identification2!.displayName]
                    : [],
                onUpload: () {
                  _showFileUploadModal(
                    title: 'Upload Secondary Identification',
                    description:
                        'Select identification type and upload identification document',
                    onFileUploaded:
                        (
                          fileName,
                          identificationType,
                          identificationNumber,
                          filePath,
                          fileSize,
                        ) {
                          setState(() {
                            _identification2 = IdentificationDocument(
                              identificationTypeId: identificationType.id ?? '',
                              identificationTypeValue: identificationType.value,
                              identificationNumber: identificationNumber,
                              file: UploadedFile(
                                fileName: fileName,
                                description: '',
                                filePath: filePath,
                              ),
                            );
                          });
                          _validateAndSave();
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text(
                                'Identification uploaded: $fileName',
                              ),
                              backgroundColor: Colors.green,
                            ),
                          );
                        },
                  );
                },
                onRemove: (index) {
                  setState(() {
                    _identification2 = null;
                  });
                  _validateAndSave();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Secondary identification removed'),
                      backgroundColor: Colors.orange,
                    ),
                  );
                },
                required: false,
              ),
              const SizedBox(height: 24),

              _buildFileUploadSection(
                title: 'Resume',
                description: 'Upload your resume (optional)',
                icon: Icons.work,
                files: _resume != null ? [_resume!.fileName] : [],
                onUpload: () {
                  _showFileUploadModal(
                    title: 'Upload Resume',
                    description: 'Upload your resume document',
                    onFileUploaded:
                        (
                          fileName,
                          identificationType,
                          identificationNumber,
                          filePath,
                          fileSize,
                        ) {
                          setState(() {
                            _resume = UploadedFile(
                              fileName: fileName,
                              description: '',
                              filePath: filePath,
                            );
                          });
                          _validateAndSave();
                          ScaffoldMessenger.of(context).showSnackBar(
                            SnackBar(
                              content: Text('Resume uploaded: $fileName'),
                              backgroundColor: Colors.green,
                            ),
                          );
                        },
                  );
                },
                onRemove: (index) {
                  setState(() {
                    _resume = null;
                  });
                  _validateAndSave();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Resume removed'),
                      backgroundColor: Colors.orange,
                    ),
                  );
                },
                required: false,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildFileUploadSection({
    required String title,
    required String description,
    required IconData icon,
    required List<String> files,
    required VoidCallback onUpload,
    required Function(int index) onRemove,
    String? errorText,
    bool required = false,
  }) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.grey.shade50,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: errorText != null ? Colors.red.shade300 : Colors.grey.shade300,
        ),
      ),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: Colors.grey.shade700),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                        color: AppTheme.textDark,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      description,
                      style: TextStyle(
                        fontSize: 13,
                        color: Colors.grey.shade600,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),

          if (files.isNotEmpty) ...[
            Container(
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.grey.shade300),
              ),
              padding: const EdgeInsets.all(8),
              child: Column(
                children: files.asMap().entries.map((entry) {
                  final index = entry.key;
                  final file = entry.value;
                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4),
                    child: Row(
                      children: [
                        const Icon(
                          Icons.check_circle,
                          color: Colors.green,
                          size: 20,
                        ),
                        const SizedBox(width: 8),
                        Expanded(
                          child: Text(
                            file,
                            style: const TextStyle(fontSize: 13),
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                        IconButton(
                          icon: const Icon(Icons.close),
                          iconSize: 18,
                          padding: EdgeInsets.zero,
                          constraints: const BoxConstraints(),
                          onPressed: () => onRemove(index),
                          color: Colors.grey.shade600,
                          tooltip: 'Remove file',
                        ),
                      ],
                    ),
                  );
                }).toList(),
              ),
            ),
            const SizedBox(height: 12),
          ],

          ElevatedButton.icon(
            onPressed: onUpload,
            icon: const Icon(Icons.upload_file, size: 20),
            label: Text(files.isEmpty ? 'Upload File' : 'Upload Another'),
            style: ElevatedButton.styleFrom(
              minimumSize: const Size(double.infinity, 44),
            ),
          ),

          if (errorText != null) ...[
            const SizedBox(height: 8),
            Text(
              errorText,
              style: TextStyle(color: Colors.red.shade700, fontSize: 12),
            ),
          ],
        ],
      ),
    );
  }
}
