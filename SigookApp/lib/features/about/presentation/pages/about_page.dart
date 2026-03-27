import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../../jobs/presentation/widgets/app_drawer.dart';
import '../widgets/hero_section.dart';
import '../widgets/about_section.dart';
import '../widgets/services_section.dart';
import '../widgets/industries_section.dart';
import '../widgets/job_categories_section.dart';
import '../widgets/legal_section.dart';

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
