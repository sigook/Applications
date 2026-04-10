import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';

class LegalSection {
  final String title;
  final List<String> paragraphs;

  const LegalSection({required this.title, required this.paragraphs});
}

class LegalPage extends StatelessWidget {
  final String title;
  final String lastUpdated;
  final List<LegalSection> sections;

  const LegalPage({
    super.key,
    required this.title,
    required this.lastUpdated,
    required this.sections,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        title: const NavbarLogo(),
        backgroundColor: AppTheme.primaryBlue,
        elevation: 0,
        surfaceTintColor: Colors.transparent,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () => context.pop(),
        ),
      ),
      body: ListView.builder(
        padding: const EdgeInsets.symmetric(vertical: 12, horizontal: 16),
        itemCount: sections.length + 1,
        itemBuilder: (context, index) {
          if (index == 0) {
            return _LastUpdatedBanner(lastUpdated: lastUpdated);
          }
          return _SectionTile(section: sections[index - 1]);
        },
      ),
    );
  }
}

class _LastUpdatedBanner extends StatelessWidget {
  final String lastUpdated;
  const _LastUpdatedBanner({required this.lastUpdated});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      decoration: BoxDecoration(
        color: AppTheme.primaryBlue.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppTheme.primaryBlue.withValues(alpha: 0.2)),
      ),
      child: Row(
        children: [
          const Icon(
            Icons.calendar_today_outlined,
            size: 16,
            color: AppTheme.primaryBlue,
          ),
          const SizedBox(width: 8),
          Text(
            'Last updated: $lastUpdated',
            style: AppTheme.bodySmall.copyWith(
              color: AppTheme.primaryBlue,
              fontWeight: FontWeight.w600,
            ),
          ),
        ],
      ),
    );
  }
}

class _SectionTile extends StatelessWidget {
  final LegalSection section;
  const _SectionTile({required this.section});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Theme(
        data: Theme.of(context).copyWith(dividerColor: Colors.transparent),
        child: ExpansionTile(
          tilePadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
          childrenPadding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
          leading: Container(
            width: 4,
            height: 28,
            decoration: BoxDecoration(
              color: AppTheme.primaryBlue,
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          title: Text(
            section.title,
            style: AppTheme.bodyMedium.copyWith(
              fontWeight: FontWeight.w600,
              color: AppTheme.textDark,
            ),
          ),
          iconColor: AppTheme.primaryBlue,
          collapsedIconColor: AppTheme.textMedium,
          children: section.paragraphs.map((p) => _ParagraphItem(text: p)).toList(),
        ),
      ),
    );
  }
}

class _ParagraphItem extends StatelessWidget {
  final String text;
  const _ParagraphItem({required this.text});

  @override
  Widget build(BuildContext context) {
    final isBullet = text.startsWith('• ');
    return Padding(
      padding: EdgeInsets.only(
        left: isBullet ? 8 : 0,
        bottom: 8,
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (isBullet) ...[
            Padding(
              padding: const EdgeInsets.only(top: 5, right: 8),
              child: Container(
                width: 5,
                height: 5,
                decoration: const BoxDecoration(
                  color: AppTheme.primaryBlue,
                  shape: BoxShape.circle,
                ),
              ),
            ),
            Expanded(
              child: Text(
                text.substring(2),
                style: AppTheme.bodySmall.copyWith(
                  color: AppTheme.textMedium,
                  height: 1.5,
                ),
              ),
            ),
          ] else
            Expanded(
              child: Text(
                text,
                style: AppTheme.bodySmall.copyWith(
                  color: AppTheme.textMedium,
                  height: 1.6,
                ),
              ),
            ),
        ],
      ),
    );
  }
}
