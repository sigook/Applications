import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/usecases/get_job_details.dart';
import '../providers/jobs_providers.dart';
import '../widgets/job_details_tab.dart';
import '../widgets/punch_card_tab.dart';
import '../widgets/timesheet_tab.dart';
import '../widgets/app_drawer.dart';

class JobPage extends ConsumerStatefulWidget {
  final String jobId;

  const JobPage({super.key, required this.jobId});

  @override
  ConsumerState<JobPage> createState() => _JobPageState();
}

class _JobPageState extends ConsumerState<JobPage> {
  JobDetails? _jobDetails;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadJobDetails();
  }

  Future<void> _loadJobDetails() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final useCase = ref.read(getJobDetailsUseCaseProvider);
    final result = await useCase(GetJobDetailsParams(jobId: widget.jobId));

    if (!mounted) return;

    result.fold(
      (failure) {
        setState(() {
          _error = failure.message;
          _isLoading = false;
        });
      },
      (jobDetails) {
        setState(() {
          _jobDetails = jobDetails;
          _isLoading = false;
        });
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          backgroundColor: AppTheme.primaryBlue,
          title: const Text('Job Details'),
        ),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null || _jobDetails == null) {
      return Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          backgroundColor: AppTheme.primaryBlue,
          title: const Text('Job Details'),
        ),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, size: 64, color: Colors.grey),
              const SizedBox(height: 16),
              Text(
                _error ?? 'Failed to load job details',
                style: const TextStyle(fontSize: 16),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 16),
              ElevatedButton(
                onPressed: _loadJobDetails,
                child: const Text('Retry'),
              ),
            ],
          ),
        ),
      );
    }

    final jobDetails = _jobDetails!;
    final showTimesheetPunchcard = jobDetails.shouldShowTimesheetAndPunchcard;
    final tabCount = showTimesheetPunchcard ? 3 : 1;

    return DefaultTabController(
      length: tabCount,
      child: Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        body: NestedScrollView(
          headerSliverBuilder: (context, innerBoxIsScrolled) {
            return [
              SliverAppBar(
                expandedHeight: 200,
                pinned: true,
                backgroundColor: AppTheme.primaryBlue,
                leading: IconButton(
                  icon: const Icon(Icons.arrow_back, color: Colors.white),
                  onPressed: () => Navigator.of(context).pop(),
                ),
                actions: [
                  Builder(
                    builder: (context) => IconButton(
                      icon: const Icon(Icons.menu, color: Colors.white),
                      onPressed: () => Scaffold.of(context).openEndDrawer(),
                    ),
                  ),
                ],
                flexibleSpace: FlexibleSpaceBar(
                  title: Text(
                    jobDetails.jobTitle,
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.w600,
                      fontSize: 16,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  titlePadding: const EdgeInsets.only(
                    left: 56,
                    right: 56,
                    bottom: 16,
                  ),
                  background: Container(
                    decoration: const BoxDecoration(
                      gradient: LinearGradient(
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                        colors: [AppTheme.primaryBlue, AppTheme.tertiaryBlue],
                      ),
                    ),
                    child: SafeArea(
                      child: Padding(
                        padding: const EdgeInsets.fromLTRB(16, 60, 16, 80),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            Text(
                              jobDetails.jobTitle,
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 20,
                                fontWeight: FontWeight.bold,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                            const SizedBox(height: 4),
                            Text(
                              jobDetails.agencyFullName,
                              style: TextStyle(
                                color: Colors.white.withValues(alpha: 0.9),
                                fontSize: 14,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ),
                ),
                bottom: PreferredSize(
                  preferredSize: const Size.fromHeight(60),
                  child: Container(
                    color: Colors.white,
                    child: TabBar(
                      indicator: UnderlineTabIndicator(
                        borderSide: const BorderSide(
                          color: AppTheme.primaryBlue,
                          width: 3,
                        ),
                        insets: const EdgeInsets.symmetric(horizontal: 16),
                      ),
                      labelColor: AppTheme.primaryBlue,
                      unselectedLabelColor: Colors.grey.shade600,
                      labelStyle: const TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        letterSpacing: 0.8,
                      ),
                      unselectedLabelStyle: const TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                        letterSpacing: 0.5,
                      ),
                      tabs: [
                        Tab(
                          height: 60,
                          child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                              Icon(Icons.work_outline, size: 24),
                              const SizedBox(height: 4),
                              const Text('JOB'),
                            ],
                          ),
                        ),
                        if (showTimesheetPunchcard) ...[
                          Tab(
                            height: 60,
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Icon(Icons.access_time, size: 24),
                                const SizedBox(height: 4),
                                const Text('PUNCH'),
                              ],
                            ),
                          ),
                          Tab(
                            height: 60,
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Icon(Icons.receipt_long, size: 24),
                                const SizedBox(height: 4),
                                const Text('TIMESHEET'),
                              ],
                            ),
                          ),
                        ],
                      ],
                    ),
                  ),
                ),
              ),
            ];
          },
          body: TabBarView(
            children: [
              JobDetailsTab(jobDetails: jobDetails),
              if (showTimesheetPunchcard) ...[
                const PunchCardTab(),
                const TimesheetTab(),
              ],
            ],
          ),
        ),
        endDrawer: const AppDrawer(currentRoute: AppRoutes.jobs),
      ),
    );
  }
}
