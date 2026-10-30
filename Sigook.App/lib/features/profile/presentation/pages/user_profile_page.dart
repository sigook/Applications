import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_cropper/image_cropper.dart';
import 'package:image_picker/image_picker.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/feedback/error_state_widget.dart';
import '../../../../core/widgets/feedback/loading_indicator.dart';
import '../../../../core/widgets/navigation/app_tab_bar.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../domain/entities/worker_requirement.dart';
import '../../profile_image/presentation/viewmodels/profile_image_viewmodel.dart';
import '../providers/cached_worker_profile_provider.dart';
import 'profile_header.dart';
import 'account_settings_tab.dart';
import 'comments_tab.dart';
import 'job_experience_tab.dart';
import 'personal_details/personal_details_tab.dart';
import 'preferences/preferences_tab.dart';

class UserProfilePage extends ConsumerStatefulWidget {
  final ProfileSection? section;

  const UserProfilePage({super.key, this.section});

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

  Future<void> _handlePhotoTap() async {
    final source = await showModalBottomSheet<ImageSource>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const SizedBox(height: 8),
            Container(
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: Colors.grey.shade300,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const SizedBox(height: 16),
            ListTile(
              leading: const Icon(Icons.camera_alt_outlined),
              title: const Text('Take a photo'),
              onTap: () => Navigator.of(ctx).pop(ImageSource.camera),
            ),
            ListTile(
              leading: const Icon(Icons.photo_library_outlined),
              title: const Text('Choose from gallery'),
              onTap: () => Navigator.of(ctx).pop(ImageSource.gallery),
            ),
            const SizedBox(height: 8),
          ],
        ),
      ),
    );

    if (source == null || !mounted) return;

    final picker = ImagePicker();
    final picked = await picker.pickImage(source: source, imageQuality: 90);
    if (picked == null || !mounted) return;

    final cropped = await ImageCropper().cropImage(
      sourcePath: picked.path,
      aspectRatio: const CropAspectRatio(ratioX: 1, ratioY: 1),
      uiSettings: [
        AndroidUiSettings(
          toolbarTitle: 'Crop Photo',
          toolbarColor: AppTheme.secondaryRed,
          toolbarWidgetColor: Colors.white,
          lockAspectRatio: true,
          cropStyle: CropStyle.circle,
        ),
        IOSUiSettings(
          title: 'Crop Photo',
          aspectRatioLockEnabled: true,
          resetAspectRatioEnabled: false,
          cropStyle: CropStyle.circle,
        ),
      ],
    );
    if (cropped == null || !mounted) return;

    ref.read(profileImageViewModelProvider.notifier).upload(cropped.path);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final profileAsync = ref.watch(cachedWorkerProfileProvider);

    ref.listen<ProfileImageState>(profileImageViewModelProvider, (_, next) {
      if (next.justUploaded) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Profile photo updated successfully')),
        );
      } else if (next.uploadError != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to upload photo: ${next.uploadError}'),
          ),
        );
      }
    });

    final isUploadingPhoto = ref
        .watch(profileImageViewModelProvider)
        .isUploading;

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.secondaryRed,
        foregroundColor: Colors.white,
        elevation: 0,
        automaticallyImplyLeading: false,
        title: const NavbarLogo(),
      ),
      body: profileAsync.when(
        data: (profile) => RefreshIndicator(
          onRefresh: () => ref.refresh(cachedWorkerProfileProvider.future),
          color: AppTheme.primaryBlue,
          child: NestedScrollView(
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
                        isEditing: true,
                        isUploading: isUploadingPhoto,
                        onPhotoTap: isUploadingPhoto ? null : _handlePhotoTap,
                      );
                    },
                  ),
                ),

                bottom: AppTabBar(
                  controller: _tabController,
                  tabs: const [
                    AppTabItem(icon: Icons.person_outline, label: 'PERSONAL'),
                    AppTabItem(
                      icon: Icons.work_history_outlined,
                      label: 'EXPERIENCE',
                    ),
                    AppTabItem(icon: Icons.tune_outlined, label: 'PREFERENCES'),
                    AppTabItem(
                      icon: Icons.rate_review_outlined,
                      label: 'COMMENTS',
                    ),
                    AppTabItem(
                      icon: Icons.manage_accounts_outlined,
                      label: 'ACCOUNT',
                    ),
                  ],
                ),
              ),
            ],
            body: TabBarView(
              controller: _tabController,
              children: [
                PersonalDetailsTab(scrollTo: widget.section),
                const WorkExperienceTab(),
                const PreferencesTab(),
                const CommentsTab(),
                AccountSettingsTab(appVersion: _appVersion),
              ],
            ),
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
