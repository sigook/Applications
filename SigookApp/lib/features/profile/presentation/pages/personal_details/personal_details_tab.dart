import 'package:flutter/material.dart';
import 'sections/basic_info_section.dart';
import 'sections/certificates_section.dart';
import 'sections/contact_info_section.dart';
import 'sections/documents_section.dart';
import 'sections/licenses_section.dart';
import 'sections/resume_section.dart';
import 'sections/sin_section.dart';

class PersonalDetailsTab extends StatelessWidget {
  const PersonalDetailsTab({super.key});

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: EdgeInsets.zero,
      children: [
        SizedBox(height: 16),
        BasicInfoSectionCard(),
        SizedBox(height: 12),
        ContactInfoSectionCard(),
        SizedBox(height: 12),
        SinSectionCard(),
        SizedBox(height: 12),
        DocumentsSectionCard(),
        SizedBox(height: 12),
        ResumeSectionCard(),
        SizedBox(height: 12),
        LicensesSectionCard(),
        SizedBox(height: 12),
        CertificatesSectionCard(),
        SizedBox(height: 24),
      ],
    );
  }
}
