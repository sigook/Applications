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
import '../../../../domain/entities/worker_profile.dart';
import '../../../../domain/usecases/update_worker_profile.dart';
import '../../../viewmodels/profile_viewmodel.dart';
import '../../../widgets/license_card.dart';
import '../../../widgets/pending_file_row.dart';
import '../../../widgets/upload_action_row.dart';

class LicensesSectionCard extends ConsumerStatefulWidget {
  final WorkerProfile? profile;

  const LicensesSectionCard({super.key, required this.profile});

  @override
  ConsumerState<LicensesSectionCard> createState() =>
      _LicensesSectionCardState();
}

class _LicensesSectionCardState extends ConsumerState<LicensesSectionCard> {
  PickedFileData? _pendingFile;
  bool _isUploading = false;
  final _licenseNumberController = TextEditingController();
  DateTime? _issuedDate;
  DateTime? _expiresDate;

  @override
  void dispose() {
    _licenseNumberController.dispose();
    super.dispose();
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
    setState(() => _isUploading = true);

    final error = await ref.read(profileViewModelProvider.notifier).uploadFile(
      ProfileSection.licenses,
      {
        'licenseNumber': _licenseNumberController.text,
        'licenseIssued': _issuedDate!.toUtc().toIso8601String(),
        'licenseExpires': _expiresDate!.toUtc().toIso8601String(),
      },
      {'licenseFile': _pendingFile!.path},
    );

    if (!mounted) return;
    setState(() => _isUploading = false);

    if (error != null) {
      showProfileError(context, 'Failed to upload license: $error');
    } else {
      setState(() {
        _pendingFile = null;
        _licenseNumberController.clear();
        _issuedDate = null;
        _expiresDate = null;
      });
      showProfileSuccess(context, 'License uploaded successfully!');
    }
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
    final profile = widget.profile;

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
            isUploading: _isUploading,
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
