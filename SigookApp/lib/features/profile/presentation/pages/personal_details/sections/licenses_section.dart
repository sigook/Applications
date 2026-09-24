import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/inputs/date_picker_field.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../licenses/presentation/viewmodels/licenses_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../../licenses/presentation/widgets/license_card.dart';
import '../../../widgets/document_description_field.dart';
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class LicensesSectionCard extends ConsumerStatefulWidget {
  const LicensesSectionCard({super.key});

  @override
  ConsumerState<LicensesSectionCard> createState() =>
      _LicensesSectionCardState();
}

class _LicensesSectionCardState extends ConsumerState<LicensesSectionCard> {
  PickedFileData? _pendingFile;
  final _licenseNumberController = TextEditingController();
  final _descriptionController = TextEditingController();
  String? _descriptionError;
  DateTime? _issuedDate;
  DateTime? _expiresDate;
  bool _expires = true;
  String? _expiresError;

  @override
  void dispose() {
    _licenseNumberController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _confirmDelete(String licenseId) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete License'),
        content: const Text(
          'Are you sure you want to delete this license? This action cannot be undone.',
        ),
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
      ref.read(licensesViewModelProvider.notifier).delete(licenseId);
    }
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

  Future<void> _pickFile() async {
    final result = await ref
        .read(filePickerServiceProvider)
        .pickFile(allowedExtensions: FilePickerService.documentExtensions);
    if (!result.isSuccess || result.file == null) return;
    setState(() => _pendingFile = result.file);
  }

  Future<void> _pickDate({required bool isIssued}) async {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final picked = await showDatePicker(
      context: context,
      initialDate: isIssued ? (_issuedDate ?? today) : (_expiresDate ?? today),
      firstDate: isIssued ? DateTime(2000) : today,
      lastDate: isIssued ? today : DateTime(2100),
      builder: (context, child) => Theme(
        data: Theme.of(context).copyWith(
          colorScheme: const ColorScheme.light(primary: AppTheme.primaryBlue),
        ),
        child: child!,
      ),
    );
    if (picked == null) return;
    setState(() {
      if (isIssued) {
        _issuedDate = picked;
      } else {
        _expiresDate = picked;
        _expiresError = null;
      }
    });
  }

  Future<void> _upload() async {
    if (_pendingFile == null) return;
    setState(() {
      _descriptionError = DocumentDescriptionField.validate(
        _descriptionController.text,
      );
      _expiresError = _expires && _expiresDate == null
          ? 'Expiration date is required'
          : null;
    });
    if (_descriptionError != null || _expiresError != null) return;
    final number = _licenseNumberController.text.trim();
    await ref
        .read(licensesViewModelProvider.notifier)
        .upload(
          filePath: _pendingFile!.path,
          description: _descriptionController.text.trim(),
          number: number.isEmpty ? null : number,
          issued: _issuedDate?.toUtc().toIso8601String(),
          expires: _expires ? _expiresDate!.toUtc().toIso8601String() : null,
        );
  }

  void _cancel() {
    setState(() {
      _pendingFile = null;
      _licenseNumberController.clear();
      _descriptionController.clear();
      _descriptionError = null;
      _issuedDate = null;
      _expiresDate = null;
      _expires = true;
      _expiresError = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(licensesViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen<LicensesState>(licensesViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justUploaded && !(prev?.justUploaded ?? false)) {
        _cancel();
        showProfileSuccess(context, 'License uploaded successfully!');
      }
      if (next.uploadError != null && next.uploadError != prev?.uploadError) {
        showProfileError(
          context,
          'Failed to upload license: ${next.uploadError}',
        );
      }
      if (next.justDeleted && !(prev?.justDeleted ?? false)) {
        showProfileSuccess(context, 'License deleted successfully!');
      }
      if (next.deleteError != null && next.deleteError != prev?.deleteError) {
        showProfileError(
          context,
          'Failed to delete license: ${next.deleteError}',
        );
      }
    });

    return ProfileSectionCard(
      title: 'Licenses',
      icon: Icons.card_membership_outlined,
      iconGradient: const [Color(0xFF7B1FA2), Color(0xFFBA68C8)],
      children: [
        if (profile != null && profile.licenses.isNotEmpty) ...[
          ...profile.licenses.map(
            (license) => LicenseCard(
              license: license,
              onPreview: license.fileUrl != null
                  ? () => _previewDocument(
                      license.fileUrl!,
                      license.description ?? 'License',
                    )
                  : null,
              onDelete: license.id != null
                  ? () => _confirmDelete(license.id!)
                  : null,
            ),
          ),
          const SizedBox(height: 4),
        ],
        if (_pendingFile != null) ...[
          const Divider(height: 24),
          Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: Text(
              'New License',
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey.shade600,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.only(bottom: 8),
            child: PendingFileRow(
              fileName: profile?.fullName.isNotEmpty == true
                  ? "${profile!.fullName}'s license"
                  : 'License file',
            ),
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
          TextField(
            controller: _licenseNumberController,
            decoration: InputDecoration(
              labelText: 'License Number',
              prefixIcon: const Icon(Icons.numbers_outlined, size: 20),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(12),
              ),
              contentPadding: const EdgeInsets.symmetric(
                horizontal: 16,
                vertical: 12,
              ),
              isDense: true,
            ),
          ),
          const SizedBox(height: 12),
          DatePickerField(
            label: 'Issued Date',
            value: _issuedDate,
            onTap: () => _pickDate(isIssued: true),
          ),
          SwitchListTile(
            contentPadding: const EdgeInsets.symmetric(horizontal: 4),
            title: const Text('Expires', style: TextStyle(fontSize: 14)),
            value: _expires,
            activeThumbColor: AppTheme.primaryBlue,
            onChanged: (v) => setState(() {
              _expires = v;
              if (!v) {
                _expiresDate = null;
                _expiresError = null;
              }
            }),
          ),
          if (_expires)
            DatePickerField(
              label: 'Expiration Date *',
              value: _expiresDate,
              onTap: () => _pickDate(isIssued: false),
              errorText: _expiresError,
            ),
          const SizedBox(height: 12),
          UploadActionRow(
            isUploading: vm.isUploading,
            label: 'Upload License',
            onUpload: _upload,
            onCancel: _cancel,
          ),
        ] else
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: _pickFile,
              icon: const Icon(Icons.upload_file, size: 18),
              label: const Text('Add License'),
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
