import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../widgets/hero_section.dart';
import '../widgets/about_section.dart';
import '../widgets/services_section.dart';
import '../widgets/industries_section.dart';
import '../widgets/job_categories_section.dart';
import '../widgets/legal_section.dart';

class AboutPage extends StatelessWidget {
  const AboutPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.secondaryRed,
        foregroundColor: Colors.white,
        iconTheme: const IconThemeData(color: Colors.white),
        elevation: 0,
        leading: IconButton(
          tooltip: 'Back',
          icon: const Icon(Icons.arrow_back),
          onPressed: () {
            notifyLogoFlash();
            if (context.canPop()) {
              context.pop();
            } else {
              context.go(AppRoutes.profile);
            }
          },
        ),
        title: const NavbarLogo(),
      ),
      body: const SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            HeroSection(),
            SizedBox(height: 24),
            AboutSection(),
            SizedBox(height: 24),
            ServicesSection(),
            SizedBox(height: 24),
            IndustriesSection(),
            SizedBox(height: 24),
            JobCategoriesSection(),
            SizedBox(height: 24),
            LegalSection(),
            SizedBox(height: 32),
          ],
        ),
      ),
    );
  }
}
