import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../certificates/presentation/viewmodels/certificates_viewmodel.dart';
import '../../../../presentation/providers/cached_worker_profile_provider.dart';
import '../../../../certificates/presentation/widgets/certificate_card.dart';
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class CertificatesSectionCard extends ConsumerStatefulWidget {
  const CertificatesSectionCard({super.key});

  @override
  ConsumerState<CertificatesSectionCard> createState() =>
      _CertificatesSectionCardState();
}

class _CertificatesSectionCardState
    extends ConsumerState<CertificatesSectionCard> {
  PickedFileData? _pendingFile;

  Future<void> _confirmDelete(String certificateId) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete Certificate'),
        content: const Text('Are you sure you want to delete this certificate? This action cannot be undone.'),
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
      ref.read(certificatesViewModelProvider.notifier).delete(certificateId);
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

  Future<void> _upload() async {
    if (_pendingFile == null) return;
    await ref
        .read(certificatesViewModelProvider.notifier)
        .upload(_pendingFile!.path);
  }

  @override
  Widget build(BuildContext context) {
    final vm = ref.watch(certificatesViewModelProvider);
    final profile = ref.watch(cachedWorkerProfileProvider).asData?.value;

    ref.listen<CertificatesState>(certificatesViewModelProvider, (prev, next) {
      if (!mounted) return;
      if (next.justUploaded && !(prev?.justUploaded ?? false)) {
        setState(() => _pendingFile = null);
        showProfileSuccess(context, 'Certificate uploaded successfully!');
      }
      if (next.uploadError != null && next.uploadError != prev?.uploadError) {
        showProfileError(context, 'Failed to upload certificate: ${next.uploadError}');
      }
      if (next.justDeleted && !(prev?.justDeleted ?? false)) {
        showProfileSuccess(context, 'Certificate deleted successfully!');
      }
      if (next.deleteError != null && next.deleteError != prev?.deleteError) {
        showProfileError(context, 'Failed to delete certificate: ${next.deleteError}');
      }
    });

    return ProfileSectionCard(
      title: 'Certificates',
      icon: Icons.workspace_premium_outlined,
      iconGradient: const [Color(0xFFE65100), Color(0xFFFF9800)],
      children: [
        if (profile != null && profile.certificates.isNotEmpty) ...[
          ...profile.certificates.map(
            (cert) => CertificateCard(
              certificate: cert,
              onPreview: cert.fileUrl != null
                  ? () => _previewDocument(
                      cert.fileUrl!, cert.description ?? 'Certificate')
                  : null,
              onDelete: cert.id != null
                  ? () => _confirmDelete(cert.id!)
                  : null,
            ),
          ),
          const SizedBox(height: 4),
        ],
        if (_pendingFile != null) ...[
          Padding(
            padding: const EdgeInsets.only(bottom: 12),
            child: PendingFileRow(
              fileName: profile?.fullName.isNotEmpty == true
                  ? "${profile!.fullName}'s certificate"
                  : 'Certificate file',
            ),
          ),
          UploadActionRow(
            isUploading: vm.isUploading,
            label: 'Upload Certificate',
            onUpload: _upload,
            onCancel: () => setState(() => _pendingFile = null),
          ),
        ] else
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: _pickFile,
              icon: const Icon(Icons.upload_file, size: 18),
              label: const Text('Add Certificate'),
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
