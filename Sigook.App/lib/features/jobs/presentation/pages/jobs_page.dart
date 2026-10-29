import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../../auth/presentation/widgets/logout_confirmation_dialog.dart';
import '../../../profile/presentation/providers/cached_worker_profile_provider.dart';
import '../viewmodels/jobs_viewmodel.dart';
import '../../../activity/history/presentation/providers/history_providers.dart';
import '../widgets/current_jobs_section.dart';
import '../widgets/job_card.dart';
import '../widgets/filter_sort_bottom_sheet.dart';
import '../widgets/profile_warning_banner.dart';

class JobsPage extends ConsumerStatefulWidget {
  const JobsPage({super.key});

  @override
  ConsumerState<JobsPage> createState() => _JobsPageState();
}

class _JobsPageState extends ConsumerState<JobsPage> {
  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) {
        ref.read(jobsViewModelProvider.notifier).loadJobs();
      }
    });
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _handleSignOut() async {
    final shouldLogout = await showLogoutConfirmationDialog(context);
    if (!shouldLogout || !mounted) return;
    await ref.read(authViewModelProvider.notifier).logout();
    if (mounted) context.go(AppRoutes.welcome);
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent * 0.8) {
      ref.read(jobsViewModelProvider.notifier).loadMore();
    }
  }

  Future<void> _onRefresh() async {
    await Future.wait([
      ref.refresh(cachedWorkerProfileProvider.future),
      ref.refresh(activeAssignmentsProvider.future),
      ref.read(jobsViewModelProvider.notifier).refresh(),
    ]);
  }

  void _showFilterModal() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => FilterSortBottomSheet(
        currentFilter: ref.read(jobsViewModelProvider).filter,
        onApply: (filter) {
          ref.read(jobsViewModelProvider.notifier).applyFilter(filter);
          ref
              .read(analyticsServiceProvider)
              .logEvent(
                name: 'jobs_filter_applied',
                parameters: {
                  'sort': filter.sortBy.name,
                  'search': filter.hasSearch,
                },
              );
        },
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(jobsViewModelProvider);
    final activeAsync = ref.watch(activeAssignmentsProvider);
    final hasActiveJobs = activeAsync.value?.isNotEmpty ?? false;
    final jobs = state.jobs;
    final hasSearch = state.filter.hasSearch;

    return Scaffold(
      backgroundColor: AppTheme.surfaceGrey,
      appBar: AppBar(
        backgroundColor: AppTheme.secondaryRed,
        foregroundColor: Colors.white,
        elevation: 0,
        automaticallyImplyLeading: false,
        leading: hasActiveJobs
            ? null
            : IconButton(
                tooltip: 'Filter jobs',
                icon: const Icon(Icons.filter_list),
                onPressed: () {
                  notifyLogoFlash();
                  _showFilterModal();
                },
              ),
        title: const NavbarLogo(),
      ),
      body:
          (state.isLoading && state.jobs.isEmpty) ||
              (activeAsync.isLoading && !activeAsync.hasValue)
          ? const Center(
              child: CircularProgressIndicator(color: AppTheme.primaryBlue),
            )
          : hasActiveJobs
          ? RefreshIndicator(
              onRefresh: _onRefresh,
              color: AppTheme.primaryBlue,
              child: ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.only(top: 8, bottom: 16),
                children: const [ProfileWarningBanner(), CurrentJobsSection()],
              ),
            )
          : state.error != null && state.jobs.isEmpty
          ? Center(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 32),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Container(
                      padding: const EdgeInsets.all(20),
                      decoration: BoxDecoration(
                        color: AppTheme.errorRed.withValues(alpha: 0.1),
                        shape: BoxShape.circle,
                      ),
                      child: Icon(
                        Icons.cloud_off_outlined,
                        size: 64,
                        color: AppTheme.errorRed,
                      ),
                    ),
                    const SizedBox(height: 24),
                    Text(
                      'Unable to Load Jobs',
                      style: TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: Colors.grey.shade800,
                      ),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 12),
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: Colors.grey.shade100,
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: Colors.grey.shade300),
                      ),
                      child: Text(
                        state.error!,
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey.shade700,
                          height: 1.5,
                        ),
                        textAlign: TextAlign.center,
                      ),
                    ),
                    const SizedBox(height: 24),
                    ElevatedButton.icon(
                      onPressed: () {
                        ref.read(jobsViewModelProvider.notifier).loadJobs();
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.primaryBlue,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(
                          horizontal: 32,
                          vertical: 16,
                        ),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                      icon: const Icon(Icons.refresh, size: 22),
                      label: const Text(
                        'Try Again',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    TextButton.icon(
                      onPressed: _handleSignOut,
                      icon: Icon(
                        Icons.logout,
                        size: 18,
                        color: Colors.grey.shade600,
                      ),
                      label: Text(
                        'Sign Out',
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey.shade600,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            )
          : jobs.isEmpty
          ? RefreshIndicator(
              onRefresh: _onRefresh,
              color: AppTheme.primaryBlue,
              child: LayoutBuilder(
                builder: (context, constraints) => SingleChildScrollView(
                  physics: const AlwaysScrollableScrollPhysics(),
                  child: ConstrainedBox(
                    constraints: BoxConstraints(
                      minHeight: constraints.maxHeight,
                    ),
                    child: Column(
                      children: [
                        const ProfileWarningBanner(),
                        Padding(
                          padding: const EdgeInsets.symmetric(vertical: 48),
                          child: Center(
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Icon(
                                  Icons.work_outline,
                                  size: 64,
                                  color: Colors.grey.shade400,
                                ),
                                const SizedBox(height: 16),
                                Text(
                                  hasSearch
                                      ? 'No jobs match your search'
                                      : 'No jobs available',
                                  style: TextStyle(
                                    fontSize: 18,
                                    fontWeight: FontWeight.w600,
                                    color: Colors.grey.shade700,
                                  ),
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  hasSearch
                                      ? 'Try a different position or request number'
                                      : 'Check back later for new opportunities',
                                  style: TextStyle(
                                    fontSize: 14,
                                    color: Colors.grey.shade600,
                                  ),
                                ),
                                if (hasSearch) ...[
                                  const SizedBox(height: 16),
                                  TextButton(
                                    onPressed: () => ref
                                        .read(jobsViewModelProvider.notifier)
                                        .clearSearch(),
                                    child: const Text('Clear Search'),
                                  ),
                                ],
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            )
          : RefreshIndicator(
              onRefresh: _onRefresh,
              color: AppTheme.primaryBlue,
              child: ListView.builder(
                controller: _scrollController,
                physics: const AlwaysScrollableScrollPhysics(),
                padding: const EdgeInsets.only(top: 8, bottom: 16),
                itemCount: jobs.length + 1 + (state.isLoadingMore ? 1 : 0),
                itemBuilder: (context, index) {
                  if (index == 0) return const ProfileWarningBanner();
                  final jobIndex = index - 1;
                  if (jobIndex == jobs.length) {
                    return const Padding(
                      padding: EdgeInsets.all(16),
                      child: Center(
                        child: CircularProgressIndicator(
                          color: AppTheme.primaryBlue,
                        ),
                      ),
                    );
                  }
                  return JobCard(job: jobs[jobIndex]);
                },
              ),
            ),
    );
  }
}
