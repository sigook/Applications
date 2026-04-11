import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/feedback/error_state_widget.dart';
import '../../../../core/widgets/feedback/loading_indicator.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../../auth/presentation/pages/logout_webview_page.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../providers/cached_worker_profile_provider.dart';
import '../widgets/profile_header.dart';
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
    _tabController = TabController(length: 2, vsync: this);
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

  Future<void> _showLogoutDialog() async {
    final shouldLogout = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Logout'),
        content: const Text('Are you sure you want to logout?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.of(context).pop(true),
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.secondaryRed,
            ),
            child: const Text('Logout'),
          ),
        ],
      ),
    );

    if (shouldLogout != true || !mounted) return;

    final idToken = ref.read(authViewModelProvider).token?.idToken;
    final notifier = ref.read(authViewModelProvider.notifier);

    await Navigator.of(context).push(
      MaterialPageRoute<bool>(
        builder: (_) => LogoutWebviewPage(idToken: idToken),
      ),
    );

    await notifier.logout();
    if (mounted) context.go(AppRoutes.welcome);
  }

  @override
  Widget build(BuildContext context) {
    final profileAsync = ref.watch(cachedWorkerProfileProvider);

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.primaryBlue,
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
                preferredSize: const Size.fromHeight(48.0),
                child: Container(
                  color: Colors.white,
                  child: TabBar(
                    controller: _tabController,
                    labelColor: AppTheme.primaryBlue,
                    unselectedLabelColor: Colors.grey.shade600,
                    indicatorColor: AppTheme.primaryBlue,
                    indicatorWeight: 3,
                    labelStyle: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                    ),
                    unselectedLabelStyle: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w500,
                    ),
                    tabs: const [
                      Tab(text: 'Personal Details'),
                      Tab(text: 'Preferences'),
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
              PreferencesTab(
                onLogout: _showLogoutDialog,
                appVersion: _appVersion,
              ),
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
