import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/providers/file_picker_provider.dart';
import '../../../../core/services/file_picker_service.dart';
import '../../../catalog/domain/entities/catalog_item.dart';
import '../../../catalog/presentation/providers/catalog_providers.dart';

/// Modal dialog for uploading files with identification type selection
class FileUploadModal extends ConsumerStatefulWidget {
  final String title;
  final String description;

  const FileUploadModal({
    super.key,
    required this.title,
    required this.description,
  });

  @override
  ConsumerState<FileUploadModal> createState() => _FileUploadModalState();
}

class _FileUploadModalState extends ConsumerState<FileUploadModal> {
  CatalogItem? _selectedIdentificationType;
  String _searchQuery = '';
  String _identificationNumber = '';
  PickedFileData? _selectedFile;
  bool _isPickingFile = false;
  final TextEditingController _searchController = TextEditingController();
  final TextEditingController _identificationNumberController =
      TextEditingController();

  @override
  void dispose() {
    _searchController.dispose();
    _identificationNumberController.dispose();
    super.dispose();
  }

  /// Pick file using file picker service
  Future<void> _pickFile() async {
    setState(() {
      _isPickingFile = true;
    });

    try {
      final filePickerService = ref.read(filePickerServiceProvider);

      // Pick file with PDF, JPG, PNG allowed
      final result = await filePickerService.pickFile(
        allowedExtensions: ['pdf', 'jpg', 'jpeg', 'png'],
        maxFileSizeMB: 10,
      );

      if (!mounted) return;

      if (result.isSuccess) {
        setState(() {
          _selectedFile = result.file;
          _isPickingFile = false;
        });
      } else if (result.isError) {
        setState(() {
          _isPickingFile = false;
        });

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(result.errorMessage ?? 'Failed to pick file'),
            backgroundColor: Colors.red,
          ),
        );
      } else {
        // Cancelled
        setState(() {
          _isPickingFile = false;
        });
      }
    } catch (e) {
      if (!mounted) return;

      setState(() {
        _isPickingFile = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Error picking file: $e'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final identificationTypesAsync = ref.watch(identificationTypesListProvider);

    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Container(
        constraints: const BoxConstraints(maxWidth: 500, maxHeight: 600),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Header
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Theme.of(context).primaryColor.withValues(alpha: 0.1),
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(16),
                  topRight: Radius.circular(16),
                ),
              ),
              child: Row(
                children: [
                  Icon(
                    Icons.upload_file,
                    color: Theme.of(context).primaryColor,
                    size: 28,
                  ),
                  const SizedBox(width: 12),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          widget.title,
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Color(0xFF1E293B),
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          widget.description,
                          style: TextStyle(
                            fontSize: 13,
                            color: Colors.grey.shade700,
                          ),
                        ),
                      ],
                    ),
                  ),
                  IconButton(
                    onPressed: () => Navigator.of(context).pop(),
                    icon: const Icon(Icons.close),
                    color: Colors.grey.shade600,
                  ),
                ],
              ),
            ),

            // Content
            Flexible(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Identification Type Section
                    const Text(
                      'Identification Type',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: Color(0xFF1E293B),
                      ),
                    ),
                    const SizedBox(height: 8),

                    // Searchable Dropdown
                    identificationTypesAsync.when(
                      data: (identificationTypes) {
                        // Filter based on search query
                        final filteredTypes = identificationTypes.where((type) {
                          return type.value.toLowerCase().contains(
                            _searchQuery.toLowerCase(),
                          );
                        }).toList();

                        return Column(
                          children: [
                            // Search field
                            TextField(
                              controller: _searchController,
                              decoration: InputDecoration(
                                hintText: 'Search identification type...',
                                prefixIcon: const Icon(Icons.search),
                                suffixIcon: _searchQuery.isNotEmpty
                                    ? IconButton(
                                        icon: const Icon(Icons.clear),
                                        onPressed: () {
                                          setState(() {
                                            _searchController.clear();
                                            _searchQuery = '';
                                          });
                                        },
                                      )
                                    : null,
                                border: OutlineInputBorder(
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                contentPadding: const EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 12,
                                ),
                              ),
                              onChanged: (value) {
                                setState(() {
                                  _searchQuery = value;
                                });
                              },
                            ),
                            const SizedBox(height: 12),

                            // Dropdown container
                            Container(
                              constraints: const BoxConstraints(maxHeight: 200),
                              decoration: BoxDecoration(
                                border: Border.all(color: Colors.grey.shade300),
                                borderRadius: BorderRadius.circular(8),
                              ),
                              child: filteredTypes.isEmpty
                                  ? Padding(
                                      padding: const EdgeInsets.all(16),
                                      child: Center(
                                        child: Text(
                                          'No identification types found',
                                          style: TextStyle(
                                            color: Colors.grey.shade600,
                                            fontSize: 14,
                                          ),
                                        ),
                                      ),
                                    )
                                  : ListView.builder(
                                      shrinkWrap: true,
                                      itemCount: filteredTypes.length,
                                      itemBuilder: (context, index) {
                                        final type = filteredTypes[index];
                                        final isSelected =
                                            _selectedIdentificationType?.id ==
                                            type.id;

                                        return InkWell(
                                          onTap: () {
                                            setState(() {
                                              _selectedIdentificationType =
                                                  type;
                                            });
                                          },
                                          child: Container(
                                            padding: const EdgeInsets.symmetric(
                                              horizontal: 16,
                                              vertical: 12,
                                            ),
                                            decoration: BoxDecoration(
                                              color: isSelected
                                                  ? Theme.of(context)
                                                        .primaryColor
                                                        .withValues(alpha: 0.1)
                                                  : null,
                                              border: Border(
                                                bottom: BorderSide(
                                                  color: Colors.grey.shade200,
                                                  width: 1,
                                                ),
                                              ),
                                            ),
                                            child: Row(
                                              children: [
                                                Expanded(
                                                  child: Text(
                                                    type.value,
                                                    style: TextStyle(
                                                      fontSize: 14,
                                                      fontWeight: isSelected
                                                          ? FontWeight.w600
                                                          : FontWeight.normal,
                                                      color: isSelected
                                                          ? Theme.of(
                                                              context,
                                                            ).primaryColor
                                                          : const Color(
                                                              0xFF1E293B,
                                                            ),
                                                    ),
                                                  ),
                                                ),
                                                if (isSelected)
                                                  Icon(
                                                    Icons.check_circle,
                                                    color: Theme.of(
                                                      context,
                                                    ).primaryColor,
                                                    size: 20,
                                                  ),
                                              ],
                                            ),
                                          ),
                                        );
                                      },
                                    ),
                            ),
                          ],
                        );
                      },
                      loading: () => const Center(
                        child: Padding(
                          padding: EdgeInsets.all(20),
                          child: CircularProgressIndicator(),
                        ),
                      ),
                      error: (error, stack) => Container(
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          color: Colors.red.shade50,
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.red.shade300),
                        ),
                        child: Row(
                          children: [
                            Icon(
                              Icons.error_outline,
                              color: Colors.red.shade700,
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Text(
                                'Failed to load identification types',
                                style: TextStyle(
                                  color: Colors.red.shade700,
                                  fontSize: 14,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),

                    const SizedBox(height: 24),

                    // Selected Type Display
                    if (_selectedIdentificationType != null) ...[
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.green.shade50,
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.green.shade300),
                        ),
                        child: Row(
                          children: [
                            Icon(
                              Icons.check_circle,
                              color: Colors.green.shade700,
                              size: 20,
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  const Text(
                                    'Selected:',
                                    style: TextStyle(
                                      fontSize: 12,
                                      fontWeight: FontWeight.w500,
                                    ),
                                  ),
                                  Text(
                                    _selectedIdentificationType!.value,
                                    style: TextStyle(
                                      fontSize: 14,
                                      fontWeight: FontWeight.w600,
                                      color: Colors.green.shade900,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 16),

                      // Identification Number Field
                      const Text(
                        'Identification Number',
                        style: TextStyle(
                          fontSize: 14,
                          fontWeight: FontWeight.w600,
                          color: Color(0xFF1E293B),
                        ),
                      ),
                      const SizedBox(height: 8),
                      TextField(
                        controller: _identificationNumberController,
                        decoration: InputDecoration(
                          hintText: 'Enter identification number',
                          prefixIcon: const Icon(Icons.numbers),
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                          contentPadding: const EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 12,
                          ),
                        ),
                        onChanged: (value) {
                          setState(() {
                            _identificationNumber = value.trim();
                          });
                        },
                      ),
                      const SizedBox(height: 24),
                    ],

                    // File Upload Section
                    const Text(
                      'Upload File',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: Color(0xFF1E293B),
                      ),
                    ),
                    const SizedBox(height: 8),
                    // Selected file display
                    if (_selectedFile != null) ...[
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.green.shade50,
                          borderRadius: BorderRadius.circular(8),
                          border: Border.all(color: Colors.green.shade300),
                        ),
                        child: Row(
                          children: [
                            Icon(
                              Icons.check_circle,
                              color: Colors.green.shade700,
                              size: 20,
                            ),
                            const SizedBox(width: 8),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    _selectedFile!.name,
                                    style: TextStyle(
                                      fontSize: 14,
                                      fontWeight: FontWeight.w600,
                                      color: Colors.green.shade900,
                                    ),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                  Text(
                                    _selectedFile!.formattedSize,
                                    style: TextStyle(
                                      fontSize: 12,
                                      color: Colors.green.shade700,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            IconButton(
                              icon: const Icon(Icons.close),
                              iconSize: 20,
                              onPressed: () {
                                setState(() {
                                  _selectedFile = null;
                                });
                              },
                              color: Colors.grey.shade600,
                              tooltip: 'Remove file',
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 12),
                    ],
                    Container(
                      decoration: BoxDecoration(
                        border: Border.all(
                          color:
                              (_selectedIdentificationType != null &&
                                  _identificationNumber.isNotEmpty)
                              ? Theme.of(context).primaryColor.withAlpha(128)
                              : Colors.grey.shade300,
                          width:
                              (_selectedIdentificationType != null &&
                                  _identificationNumber.isNotEmpty)
                              ? 2
                              : 1,
                          style: BorderStyle.solid,
                        ),
                        borderRadius: BorderRadius.circular(8),
                        color:
                            (_selectedIdentificationType != null &&
                                _identificationNumber.isNotEmpty)
                            ? Theme.of(context).primaryColor.withAlpha(5)
                            : Colors.grey.shade50,
                      ),
                      child: Material(
                        color: Colors.transparent,
                        child: InkWell(
                          onTap:
                              (_selectedIdentificationType != null &&
                                  _identificationNumber.isNotEmpty &&
                                  !_isPickingFile)
                              ? _pickFile
                              : null,
                          borderRadius: BorderRadius.circular(8),
                          child: Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 16,
                              vertical: 40,
                            ),
                            child: Column(
                              children: [
                                if (_isPickingFile)
                                  const CircularProgressIndicator()
                                else
                                  Icon(
                                    _selectedFile != null
                                        ? Icons.cloud_done_outlined
                                        : Icons.cloud_upload_outlined,
                                    size: 48,
                                    color: _selectedFile != null
                                        ? Colors.green
                                        : _selectedIdentificationType != null &&
                                              _identificationNumber.isNotEmpty
                                        ? Theme.of(context).primaryColor
                                        : Colors.grey.shade400,
                                  ),
                                const SizedBox(height: 12),
                                Text(
                                  _isPickingFile
                                      ? 'Selecting file...'
                                      : _selectedFile != null
                                      ? 'Click to change file'
                                      : _selectedIdentificationType != null &&
                                            _identificationNumber.isNotEmpty
                                      ? 'Click to select file'
                                      : _selectedIdentificationType == null
                                      ? 'Select identification type first'
                                      : 'Enter identification number',
                                  style: TextStyle(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w600,
                                    color:
                                        _selectedIdentificationType != null &&
                                            _identificationNumber.isNotEmpty
                                        ? const Color(0xFF1E293B)
                                        : Colors.grey.shade500,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  'Supported: PDF, JPG, PNG (Max 10MB)',
                                  style: TextStyle(
                                    fontSize: 12,
                                    color: Colors.grey.shade600,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),

            // Footer buttons
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                border: Border(top: BorderSide(color: Colors.grey.shade300)),
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  TextButton(
                    onPressed: () => Navigator.of(context).pop(),
                    child: const Text('Cancel'),
                  ),
                  const SizedBox(width: 12),
                  ElevatedButton.icon(
                    onPressed:
                        (_selectedIdentificationType != null &&
                            _identificationNumber.isNotEmpty &&
                            _selectedFile != null)
                        ? () {
                            Navigator.of(context).pop({
                              'identificationType': _selectedIdentificationType,
                              'identificationNumber': _identificationNumber,
                              'file': _selectedFile!.name,
                              'filePath': _selectedFile!.path,
                              'fileSize': _selectedFile!.size,
                            });
                          }
                        : null,
                    icon: const Icon(Icons.check, size: 20),
                    label: const Text('Confirm'),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
