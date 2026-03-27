import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../../jobs/presentation/widgets/app_drawer.dart';

class AboutPage extends StatefulWidget {
  const AboutPage({super.key});

  @override
  State<AboutPage> createState() => _AboutPageState();
}

class _AboutPageState extends State<AboutPage> {
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _scaffoldKey,
      backgroundColor: AppTheme.surfaceGrey,
      endDrawer: const AppDrawer(currentRoute: AppRoutes.about),
      appBar: AppBar(
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
        iconTheme: const IconThemeData(color: Colors.white),
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () {
            notifyLogoFlash();
            if (context.canPop()) {
              context.pop();
            } else {
              context.go(AppRoutes.jobs);
            }
          },
        ),
        title: const NavbarLogo(),
        actions: [
          IconButton(
            icon: const Icon(Icons.menu),
            onPressed: () {
              notifyLogoFlash();
              _scaffoldKey.currentState?.openEndDrawer();
            },
          ),
        ],
      ),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            _HeroSection(),
            const SizedBox(height: 24),
            _AboutSection(),
            const SizedBox(height: 24),
            _ServicesSection(),
            const SizedBox(height: 24),
            _IndustriesSection(),
            const SizedBox(height: 24),
            _JobCategoriesSection(),
            const SizedBox(height: 24),
            _LegalSection(),
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }
}

class _HeroSection extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.white,
      padding: const EdgeInsets.symmetric(vertical: 32, horizontal: 24),
      child: Column(
        children: [
          Image.asset(
            'assets/images/logo/sigook-logo.png',
            width: 160,
          ),
          const SizedBox(height: 16),
          Text(
            'SIGOOK® Work Factory',
            style: AppTheme.heading2.copyWith(
              color: AppTheme.primaryBlue,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            'Connecting workers with opportunities',
            style: AppTheme.bodyMedium.copyWith(color: AppTheme.textLight),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}

class _AboutSection extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.06),
              blurRadius: 12,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppTheme.primaryBlue.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: const Icon(
                    Icons.info_outline_rounded,
                    color: AppTheme.primaryBlue,
                    size: 20,
                  ),
                ),
                const SizedBox(width: 12),
                Text(
                  'Who We Are',
                  style: AppTheme.heading3.copyWith(
                    color: AppTheme.textDark,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Text(
              'We are SIGOOK® Work Factory, a staffing agency in Florida '
              'dedicated to connecting skilled workers with the right job '
              'opportunities. We specialize in temporary, permanent, and '
              'contract placements across multiple industries.',
              style: AppTheme.bodyMedium.copyWith(
                color: AppTheme.textMedium,
                height: 1.6,
              ),
            ),
            const SizedBox(height: 12),
            Text(
              'Our mission is to simplify hiring for businesses and make '
              'finding meaningful work easier for every worker — whether '
              'you\'re looking for your first job or your next career move.',
              style: AppTheme.bodyMedium.copyWith(
                color: AppTheme.textMedium,
                height: 1.6,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ServicesSection extends StatelessWidget {
  static const _services = [
    _ServiceItem(
      icon: Icons.access_time_rounded,
      title: 'Temporary',
      description: 'Short-term placements to cover seasonal or project-based needs.',
      color: Color(0xFF1565C0),
    ),
    _ServiceItem(
      icon: Icons.sync_alt_rounded,
      title: 'Temp-to-Hire',
      description: 'Start temporary and transition to a permanent role.',
      color: Color(0xFF0277BD),
    ),
    _ServiceItem(
      icon: Icons.verified_user_rounded,
      title: 'Permanent',
      description: 'Direct permanent placements for long-term positions.',
      color: Color(0xFF43A047),
    ),
    _ServiceItem(
      icon: Icons.assignment_rounded,
      title: 'Contract',
      description: 'Fixed-term contracts for specialized project work.',
      color: Color(0xFFE53935),
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.only(left: 4, bottom: 12),
            child: Text(
              'Our Services',
              style: AppTheme.heading3.copyWith(
                color: AppTheme.textDark,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            crossAxisSpacing: 12,
            mainAxisSpacing: 12,
            childAspectRatio: 1.1,
            children: _services
                .map((s) => _ServiceCard(service: s))
                .toList(),
          ),
        ],
      ),
    );
  }
}

class _ServiceItem {
  final IconData icon;
  final String title;
  final String description;
  final Color color;

  const _ServiceItem({
    required this.icon,
    required this.title,
    required this.description,
    required this.color,
  });
}

class _ServiceCard extends StatelessWidget {
  final _ServiceItem service;

  const _ServiceCard({required this.service});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.06),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: service.color.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Icon(service.icon, color: service.color, size: 22),
          ),
          const SizedBox(height: 12),
          Text(
            service.title,
            style: AppTheme.bodyMedium.copyWith(
              fontWeight: FontWeight.bold,
              color: AppTheme.textDark,
            ),
          ),
          const SizedBox(height: 4),
          Expanded(
            child: Text(
              service.description,
              style: AppTheme.bodySmall.copyWith(
                color: AppTheme.textLight,
                height: 1.4,
              ),
              maxLines: 3,
              overflow: TextOverflow.ellipsis,
            ),
          ),
        ],
      ),
    );
  }
}

class _IndustriesSection extends StatelessWidget {
  static const _industries = [
    _IndustryItem(icon: Icons.factory_rounded, label: 'Light Industry'),
    _IndustryItem(icon: Icons.business_center_rounded, label: 'Clerical'),
    _IndustryItem(icon: Icons.construction_rounded, label: 'Skilled Trades'),
    _IndustryItem(icon: Icons.computer_rounded, label: 'Information Technology'),
  ];

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.06),
              blurRadius: 12,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Industries We Serve',
              style: AppTheme.heading3.copyWith(
                color: AppTheme.textDark,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Wrap(
              spacing: 12,
              runSpacing: 12,
              children: _industries
                  .map((i) => _IndustryChip(item: i))
                  .toList(),
            ),
          ],
        ),
      ),
    );
  }
}

class _IndustryItem {
  final IconData icon;
  final String label;

  const _IndustryItem({required this.icon, required this.label});
}

class _IndustryChip extends StatelessWidget {
  final _IndustryItem item;

  const _IndustryChip({required this.item});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      decoration: BoxDecoration(
        color: AppTheme.primaryBlue.withValues(alpha: 0.07),
        borderRadius: BorderRadius.circular(32),
        border: Border.all(
          color: AppTheme.primaryBlue.withValues(alpha: 0.2),
        ),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(item.icon, color: AppTheme.primaryBlue, size: 16),
          const SizedBox(width: 8),
          Text(
            item.label,
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

class _JobCategoriesSection extends StatelessWidget {
  static const _categories = [
    _CategoryItem(
      icon: Icons.admin_panel_settings_rounded,
      title: 'Administrative',
      subtitle: 'Office support, data entry, coordination',
    ),
    _CategoryItem(
      icon: Icons.headset_mic_rounded,
      title: 'Customer Service',
      subtitle: 'Call center, support, client relations',
    ),
    _CategoryItem(
      icon: Icons.warehouse_rounded,
      title: 'Warehouse & Supply Chain',
      subtitle: 'Logistics, forklift, inventory management',
    ),
    _CategoryItem(
      icon: Icons.precision_manufacturing_rounded,
      title: 'Industrial & Manufacturing',
      subtitle: 'Assembly, production, quality control',
    ),
    _CategoryItem(
      icon: Icons.code_rounded,
      title: 'Technology & IT',
      subtitle: 'Software, networking, technical support',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Container(
        padding: const EdgeInsets.all(20),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.06),
              blurRadius: 12,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Job Categories',
              style: AppTheme.heading3.copyWith(
                color: AppTheme.textDark,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ...List.generate(_categories.length, (index) {
              final cat = _categories[index];
              return Column(
                children: [
                  _CategoryRow(item: cat),
                  if (index < _categories.length - 1)
                    Divider(
                      height: 24,
                      color: Colors.grey.shade100,
                    ),
                ],
              );
            }),
          ],
        ),
      ),
    );
  }
}

class _CategoryItem {
  final IconData icon;
  final String title;
  final String subtitle;

  const _CategoryItem({
    required this.icon,
    required this.title,
    required this.subtitle,
  });
}

class _CategoryRow extends StatelessWidget {
  final _CategoryItem item;

  const _CategoryRow({required this.item});

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            color: AppTheme.primaryBlue.withValues(alpha: 0.08),
            borderRadius: BorderRadius.circular(10),
          ),
          child: Icon(item.icon, color: AppTheme.primaryBlue, size: 20),
        ),
        const SizedBox(width: 14),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                item.title,
                style: AppTheme.bodyMedium.copyWith(
                  fontWeight: FontWeight.w600,
                  color: AppTheme.textDark,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                item.subtitle,
                style: AppTheme.bodySmall.copyWith(color: AppTheme.textLight),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _LegalSection extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16),
      child: Column(
        children: [
          const Divider(height: 1),
          const SizedBox(height: 16),
          Text(
            'Legal',
            style: AppTheme.bodySmall.copyWith(
              color: AppTheme.textLight,
              fontWeight: FontWeight.w600,
              letterSpacing: 1.2,
            ),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () => context.push(AppRoutes.privacyPolicy),
                  icon: const Icon(Icons.shield_outlined, size: 16),
                  label: const Text('Privacy Policy'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppTheme.primaryBlue,
                    side: BorderSide(
                      color: AppTheme.primaryBlue.withValues(alpha: 0.4),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    textStyle: AppTheme.bodySmall.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: OutlinedButton.icon(
                  onPressed: () => context.push(AppRoutes.terms),
                  icon: const Icon(Icons.gavel_rounded, size: 16),
                  label: const Text('Terms & Conditions'),
                  style: OutlinedButton.styleFrom(
                    foregroundColor: AppTheme.primaryBlue,
                    side: BorderSide(
                      color: AppTheme.primaryBlue.withValues(alpha: 0.4),
                    ),
                    padding: const EdgeInsets.symmetric(vertical: 12),
                    textStyle: AppTheme.bodySmall.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 8),
          Text(
            '© ${DateTime.now().year} SIGOOK® Work Factory. All rights reserved.',
            style: AppTheme.bodySmall.copyWith(color: AppTheme.textLight),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }
}
