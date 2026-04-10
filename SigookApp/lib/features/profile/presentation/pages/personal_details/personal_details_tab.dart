import 'package:flutter/material.dart';
import '../../../../../core/services/file_picker_service.dart';
import '../../../domain/entities/worker_profile.dart';
import '../../../domain/usecases/update_worker_profile.dart';
import 'sections/basic_info_section.dart';
import 'sections/certificates_section.dart';
import 'sections/contact_info_section.dart';
import 'sections/documents_section.dart';
import 'sections/licenses_section.dart';
import 'sections/resume_section.dart';
import 'sections/sin_section.dart';

class PersonalDetailsTab extends StatelessWidget {
  final WorkerProfile? profile;
  final ProfileSection? editingSection;
  final bool isSaving;
  final void Function(ProfileSection section) onEdit;
  final Future<void> Function(
    ProfileSection section,
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) onSave;
  final VoidCallback onCancel;
  final void Function(String url, String title) onPreviewDocument;
  final Future<PickedFileData?> Function() onPickFile;

  const PersonalDetailsTab({
    super.key,
    required this.profile,
    required this.editingSection,
    required this.isSaving,
    required this.onEdit,
    required this.onSave,
    required this.onCancel,
    required this.onPreviewDocument,
    required this.onPickFile,
  });

  Future<void> _saveSimple(ProfileSection section, Map<String, String> fields) =>
      onSave(section, fields);

  Future<void> _saveWithFiles(
    ProfileSection section,
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) =>
      onSave(section, fields, filePaths: filePaths);

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        const SizedBox(height: 16),
        BasicInfoSectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.personal)
              : null,
          onSave: _saveSimple,
          onCancel: onCancel,
        ),
        const SizedBox(height: 12),
        ContactInfoSectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.contact)
              : null,
          onSave: _saveSimple,
          onCancel: onCancel,
        ),
        const SizedBox(height: 12),
        SinSectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.sin)
              : null,
          onSave: _saveWithFiles,
          onCancel: onCancel,
          onPreviewDocument: onPreviewDocument,
          onPickFile: onPickFile,
        ),
        const SizedBox(height: 12),
        DocumentsSectionCard(
          profile: profile,
          editingSection: editingSection,
          isSaving: isSaving,
          onEdit: editingSection == null
              ? () => onEdit(ProfileSection.documents)
              : null,
          onSave: _saveWithFiles,
          onCancel: onCancel,
          onPreviewDocument: onPreviewDocument,
          onPickFile: onPickFile,
        ),
        const SizedBox(height: 12),
        ResumeSectionCard(profile: profile),
        const SizedBox(height: 12),
        LicensesSectionCard(profile: profile),
        const SizedBox(height: 12),
        CertificatesSectionCard(profile: profile),
        const SizedBox(height: 24),
      ],
    );
  }
}
