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
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class ResumeSectionCard extends ConsumerStatefulWidget {
  final WorkerProfile? profile;

  const ResumeSectionCard({super.key, required this.profile});

  @override
  ConsumerState<ResumeSectionCard> createState() => _ResumeSectionCardState();
}

class _ResumeSectionCardState extends ConsumerState<ResumeSectionCard> {
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
      ProfileSection.resume,
      {},
      {'resumeFile': _pendingFile!.path},
    );

    if (!mounted) return;
    setState(() => _isUploading = false);

    if (error != null) {
      showProfileError(context, 'Failed to upload resume: $error');
    } else {
      setState(() => _pendingFile = null);
      showProfileSuccess(context, 'Resume uploaded successfully!');
    }
  }

  @override
  Widget build(BuildContext context) {
    final profile = widget.profile;
    final hasResume = profile?.hasResume == true;
    final hasUrl =
        profile?.resumeFileUrl != null && profile!.resumeFileUrl!.isNotEmpty;

    return ProfileSectionCard(
      title: 'Resume',
      icon: Icons.description_outlined,
      iconGradient: const [Color(0xFF00897B), Color(0xFF4DB6AC)],
      children: [
        Padding(
          padding: const EdgeInsets.only(bottom: 12),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Icon(
                Icons.attach_file_outlined,
                size: 20,
                color: AppTheme.primaryBlue,
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Resume File',
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.grey.shade600,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    const SizedBox(height: 4),
                    if (_pendingFile != null)
                      PendingFileRow(
                        fileName: profile?.fullName.isNotEmpty == true
                            ? "${profile!.fullName}'s resume"
                            : _pendingFile!.name,
                      )
                    else
                      Text(
                        hasResume
                            ? "${profile?.fullName}'s resume"
                            : 'Not uploaded',
                        style: TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: hasResume ? AppTheme.textDark : Colors.grey,
                        ),
                      ),
                  ],
                ),
              ),
              if (hasUrl && _pendingFile == null)
                IconButton(
                  onPressed: () =>
                      _previewDocument(profile.resumeFileUrl!, 'Resume'),
                  icon: const Icon(Icons.visibility_outlined, size: 20),
                  color: AppTheme.primaryBlue,
                  tooltip: 'Preview',
                  padding: EdgeInsets.zero,
                  constraints:
                      const BoxConstraints(minWidth: 32, minHeight: 32),
                ),
            ],
          ),
        ),
        if (_pendingFile != null)
          UploadActionRow(
            isUploading: _isUploading,
            label: 'Upload Resume',
            onUpload: _upload,
            onCancel: () => setState(() => _pendingFile = null),
          )
        else
          SizedBox(
            width: double.infinity,
            child: OutlinedButton.icon(
              onPressed: _pickFile,
              icon: Icon(
                hasResume ? Icons.swap_horiz : Icons.upload_file,
                size: 18,
              ),
              label: Text(hasResume ? 'Replace Resume' : 'Upload Resume'),
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
