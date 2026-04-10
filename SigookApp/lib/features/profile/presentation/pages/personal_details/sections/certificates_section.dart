import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../../core/providers/file_picker_provider.dart';
import '../../../../../../core/services/file_picker_service.dart';
import '../../../../../../core/theme/app_theme.dart';
import '../../../../../../core/widgets/cards/profile_section_card.dart';
import '../../../../../../core/widgets/feedback/profile_snack_bar.dart';
import '../../../../../../core/widgets/navigation/document_preview_page.dart';
import '../../../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../viewmodels/profile_viewmodel.dart';
import '../../../widgets/certificate_card.dart';
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class CertificatesSectionCard extends ConsumerStatefulWidget {
  final WorkerProfile? profile;

  const CertificatesSectionCard({super.key, required this.profile});

  @override
  ConsumerState<CertificatesSectionCard> createState() =>
      _CertificatesSectionCardState();
}

class _CertificatesSectionCardState
    extends ConsumerState<CertificatesSectionCard> {
  PickedFileData? _pendingFile;
  bool _isUploading = false;

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
    setState(() => _isUploading = true);

    final error = await ref.read(profileViewModelProvider.notifier).uploadFile(
      ProfileSection.certificates,
      {},
      {'certificateFile': _pendingFile!.path},
    );

    if (!mounted) return;
    setState(() => _isUploading = false);

    if (error != null) {
      showProfileError(context, 'Failed to upload certificate: $error');
    } else {
      setState(() => _pendingFile = null);
      showProfileSuccess(context, 'Certificate uploaded successfully!');
    }
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;

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
            isUploading: _isUploading,
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
