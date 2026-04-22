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
  DateTime? _issuedDate;
  DateTime? _expiresDate;

  @override
  void dispose() {
    _licenseNumberController.dispose();
    super.dispose();
  }

  Future<void> _confirmDelete(String licenseId) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete License'),
        content: const Text('Are you sure you want to delete this license? This action cannot be undone.'),
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
        .pickFile(allowedExtensions: ['pdf', 'docx', 'jpg', 'jpeg', 'png']);
    if (!result.isSuccess || result.file == null) return;
    setState(() => _pendingFile = result.file);
  }

  Future<void> _pickDate({required bool isIssued}) async {
    final now = DateTime.now();
    final picked = await showDatePicker(
      context: context,
      initialDate: isIssued ? (_issuedDate ?? now) : (_expiresDate ?? now),
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
      builder: (context, child) => Theme(
        data: Theme.of(context).copyWith(
          colorScheme:
              const ColorScheme.light(primary: AppTheme.primaryBlue),
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
      }
    });
  }

  Future<void> _upload() async {
    if (_pendingFile == null) return;
    if (_licenseNumberController.text.isEmpty ||
        _issuedDate == null ||
        _expiresDate == null) {
      showProfileError(context, 'Please fill in all license fields');
      return;
    }
    await ref.read(licensesViewModelProvider.notifier).upload(
      filePath: _pendingFile!.path,
      number: _licenseNumberController.text,
      issued: _issuedDate!.toUtc().toIso8601String(),
      expires: _expiresDate!.toUtc().toIso8601String(),
    );
  }

  void _cancel() {
    setState(() {
      _pendingFile = null;
      _licenseNumberController.clear();
      _issuedDate = null;
      _expiresDate = null;
    });
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(licensesViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen<LicensesState>(licensesViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justUploaded && !(prev?.justUploaded ?? false)) {
        setState(() {
          _pendingFile = null;
          _licenseNumberController.clear();
          _issuedDate = null;
          _expiresDate = null;
        });
        showProfileSuccess(context, 'License uploaded successfully!');
      }
      if (next.uploadError != null && next.uploadError != prev?.uploadError) {
        showProfileError(context, 'Failed to upload license: ${next.uploadError}');
      }
      if (next.justDeleted && !(prev?.justDeleted ?? false)) {
        showProfileSuccess(context, 'License deleted successfully!');
      }
      if (next.deleteError != null && next.deleteError != prev?.deleteError) {
        showProfileError(context, 'Failed to delete license: ${next.deleteError}');
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
                      license.fileUrl!, license.description ?? 'License')
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
          Row(
            children: [
              Expanded(
                child: DatePickerField(
                  label: 'Issued Date',
                  value: _issuedDate,
                  onTap: () => _pickDate(isIssued: true),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: DatePickerField(
                  label: 'Expires Date',
                  value: _expiresDate,
                  onTap: () => _pickDate(isIssued: false),
                ),
              ),
            ],
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
