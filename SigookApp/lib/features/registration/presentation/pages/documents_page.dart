import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../domain/entities/documents_info.dart';
import '../providers/registration_providers.dart';
import '../widgets/file_upload_modal.dart';

class DocumentsPage extends ConsumerStatefulWidget {
  const DocumentsPage({super.key});

  @override
  ConsumerState<DocumentsPage> createState() => _DocumentsPageState();
}

class _DocumentsPageState extends ConsumerState<DocumentsPage> {
  List<String> _documents = [];
  String? _resume;

  @override
  void initState() {
    super.initState();

    // Load existing data if available
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final form = ref.read(registrationViewModelProvider);
      if (form.documentsInfo != null) {
        final info = form.documentsInfo!;
        _documents = List.from(info.documents);
        _resume = info.resume;
        setState(() {});
      }
    });
  }

  void _validateAndSave() {
    setState(() {
      final documentsInfo = DocumentsInfo(
        documents: _documents,
        resume: _resume,
      );

      if (documentsInfo.isValid) {
        ref
            .read(registrationViewModelProvider.notifier)
            .updateDocumentsInfo(documentsInfo);
      }
    });
  }

  /// Show file upload modal
  Future<void> _showFileUploadModal({
    required String title,
    required String description,
    required Function(
      String fileName,
      String identificationType,
      String identificationNumber,
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
      final identificationType = result['identificationType'];
      final identificationNumber = result['identificationNumber'] as String;

      onFileUploaded(
        fileName,
        identificationType?.value ?? 'Unknown',
        identificationNumber,
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

              // Identification (required)
              _buildFileUploadSection(
                title: 'Identification *',
                description: 'Upload your identification document(s)',
                icon: Icons.description,
                files: _documents,
                onUpload: () {
                  _showFileUploadModal(
                    title: 'Upload Identification',
                    description:
                        'Select identification type and upload identification document',
                    onFileUploaded:
                        (fileName, identificationType, identificationNumber) {
                          setState(() {
                            _documents.add(
                              '$identificationType #$identificationNumber - $fileName',
                            );
                          });
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
                    _documents.removeAt(index);
                  });
                  _validateAndSave();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Identification removed'),
                      backgroundColor: Colors.orange,
                    ),
                  );
                },
              ),
              const SizedBox(height: 24),

              // Resume (optional)
              _buildFileUploadSection(
                title: 'Resume',
                description: 'Upload your resume (optional)',
                icon: Icons.work,
                files: _resume != null ? [_resume!] : [],
                onUpload: () {
                  _showFileUploadModal(
                    title: 'Upload Resume',
                    description: 'Select identification type and upload resume',
                    onFileUploaded:
                        (fileName, identificationType, identificationNumber) {
                          setState(() {
                            _resume =
                                '$identificationType #$identificationNumber - $fileName';
                          });
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
                        color: Color(0xFF1E293B),
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

          // Show uploaded files
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

          // Upload button
          ElevatedButton.icon(
            onPressed: onUpload,
            icon: const Icon(Icons.upload_file, size: 20),
            label: Text(files.isEmpty ? 'Upload File' : 'Upload Another'),
            style: ElevatedButton.styleFrom(
              minimumSize: const Size(double.infinity, 44),
            ),
          ),

          // Error text
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
