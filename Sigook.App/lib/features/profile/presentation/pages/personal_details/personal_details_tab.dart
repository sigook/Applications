import 'package:flutter/material.dart';
import '../../../domain/entities/worker_requirement.dart';
import 'sections/basic_info_section.dart';
import 'sections/certificates_section.dart';
import 'sections/contact_info_section.dart';
import 'sections/documents_section.dart';
import 'sections/licenses_section.dart';
import 'sections/other_documents_section.dart';
import 'sections/resume_section.dart';
import 'sections/sin_section.dart';

class PersonalDetailsTab extends StatefulWidget {
  final ProfileSection? scrollTo;

  const PersonalDetailsTab({super.key, this.scrollTo});

  @override
  State<PersonalDetailsTab> createState() => _PersonalDetailsTabState();
}

class _PersonalDetailsTabState extends State<PersonalDetailsTab> {
  final _sinKey = GlobalKey();
  final _documentsKey = GlobalKey();

  @override
  void initState() {
    super.initState();
    final target = switch (widget.scrollTo) {
      ProfileSection.sin => _sinKey,
      ProfileSection.documents => _documentsKey,
      _ => null,
    };
    if (target == null) return;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final context = target.currentContext;
      if (context == null || !mounted) return;
      Scrollable.ensureVisible(
        context,
        duration: const Duration(milliseconds: 400),
        curve: Curves.easeInOut,
        alignment: 0.05,
      );
    });
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      physics: const AlwaysScrollableScrollPhysics(),
      children: [
        const SizedBox(height: 16),
        const BasicInfoSectionCard(),
        const SizedBox(height: 12),
        const ContactInfoSectionCard(),
        const SizedBox(height: 12),
        SinSectionCard(key: _sinKey),
        const SizedBox(height: 12),
        DocumentsSectionCard(key: _documentsKey),
        const SizedBox(height: 12),
        const ResumeSectionCard(),
        const SizedBox(height: 12),
        const LicensesSectionCard(),
        const SizedBox(height: 12),
        const CertificatesSectionCard(),
        const SizedBox(height: 12),
        const OtherDocumentsSectionCard(),
        const SizedBox(height: 24),
      ],
    );
  }
}
