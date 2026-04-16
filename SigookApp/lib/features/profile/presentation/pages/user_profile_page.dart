import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/feedback/error_state_widget.dart';
import '../../../../core/widgets/feedback/loading_indicator.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../providers/cached_worker_profile_provider.dart';
import 'profile_header.dart';
import 'account_settings_tab.dart';
import 'comments_tab.dart';
import 'job_experience_tab.dart';
import 'personal_details/personal_details_tab.dart';
import 'preferences/preferences_tab.dart';

class UserProfilePage extends ConsumerStatefulWidget {
  const UserProfilePage({super.key});

  @override
  ConsumerState<UserProfilePage> createState() => _UserProfilePageState();
}

class _UserProfilePageState extends ConsumerState<UserProfilePage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  String _appVersion = '';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 5, vsync: this);
    _loadAppVersion();
  }

  Future<void> _loadAppVersion() async {
    final info = await PackageInfo.fromPlatform();
    if (mounted) {
      setState(() => _appVersion = 'v${info.version} (${info.buildNumber})');
    }
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profileAsync = ref.watch(cachedWorkerProfileProvider);

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.secondaryRed,
        foregroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () {
            notifyLogoFlash();
            context.go(AppRoutes.jobs);
          },
        ),
        title: const NavbarLogo(),
      ),
      body: profileAsync.when(
        data: (profile) => NestedScrollView(
          headerSliverBuilder: (context, innerBoxIsScrolled) => [
            SliverAppBar(
              expandedHeight: 270.0,
              toolbarHeight: 1.0,
              pinned: true,
              floating: false,
              automaticallyImplyLeading: false,
              backgroundColor: Colors.white,
              elevation: 2,
              shadowColor: Colors.black.withValues(alpha: 0.08),
              flexibleSpace: FlexibleSpaceBar(
                collapseMode: CollapseMode.pin,
                background: Builder(
                  builder: (context) {
                    final settings = context
                        .dependOnInheritedWidgetOfExactType<
                          FlexibleSpaceBarSettings
                        >();
                    final t = settings != null
                        ? ((settings.currentExtent - settings.minExtent) /
                                  (settings.maxExtent - settings.minExtent))
                              .clamp(0.0, 1.0)
                        : 1.0;
                    return ProfileHeader(
                      name: profile?.fullName ?? 'User',
                      email: profile?.email ?? '',
                      photoUrl: profile?.profilePhotoUrl,
                      collapseRatio: t,
                    );
                  },
                ),
              ),

              bottom: PreferredSize(
                preferredSize: const Size.fromHeight(74.0),
                child: Container(
                  color: Colors.white,
                  child: TabBar(
                    controller: _tabController,
                    labelColor: AppTheme.primaryBlue,
                    unselectedLabelColor: Colors.grey.shade600,
                    indicatorColor: AppTheme.primaryBlue,
                    indicatorWeight: 3,
                    isScrollable: false,
                    tabAlignment: TabAlignment.fill,
                    labelStyle: const TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.w600,
                    ),
                    unselectedLabelStyle: const TextStyle(
                      fontSize: 11,
                      fontWeight: FontWeight.w500,
                    ),
                    tabs: const [
                      Tab(
                        icon: Icon(Icons.person_outline, size: 20),
                        text: 'Personal',
                      ),
                      Tab(
                        icon: Icon(Icons.work_history_outlined, size: 20),
                        text: 'Experience',
                      ),
                      Tab(
                        icon: Icon(Icons.tune_outlined, size: 20),
                        text: 'Preferences',
                      ),
                      Tab(
                        icon: Icon(Icons.rate_review_outlined, size: 20),
                        text: 'Comments',
                      ),
                      Tab(
                        icon: Icon(Icons.manage_accounts_outlined, size: 20),
                        text: 'Account',
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
          body: TabBarView(
            controller: _tabController,
            children: [
              const PersonalDetailsTab(),
              const WorkExperienceTab(),
              PreferencesTab(appVersion: _appVersion),
              const CommentsTab(),
              const AccountSettingsTab(),
            ],
          ),
        ),
        loading: () => const LoadingIndicator(message: 'Loading profile...'),
        error: (_, _) => ErrorStateWidget(
          title: 'Failed to load profile',
          message: 'Unable to retrieve your profile information',
          onRetry: () => ref.refresh(cachedWorkerProfileProvider),
        ),
      ),
    );
  }
}
