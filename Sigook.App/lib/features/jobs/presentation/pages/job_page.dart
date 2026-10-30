import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/providers/analytics_providers.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/services/analytics_service.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/navigation/app_tab_bar.dart';
import '../../../../core/widgets/navigation/navbar_logo.dart';
import '../../domain/entities/job_details.dart';
import '../../domain/usecases/get_job_details.dart';
import '../providers/jobs_providers.dart';
import '../widgets/job_details_tab.dart';
import '../widgets/punch_card_tab.dart';

class JobPage extends ConsumerStatefulWidget {
  final String jobId;
  final JobPageTab initialTab;

  const JobPage({
    super.key,
    required this.jobId,
    this.initialTab = JobPageTab.detail,
  });

  @override
  ConsumerState<JobPage> createState() => _JobPageState();
}

class _JobPageState extends ConsumerState<JobPage>
    with WidgetsBindingObserver {
  JobDetails? _jobDetails;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _loadJobDetails();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed && _jobDetails != null) {
      _refreshJobDetails();
    }
  }

  Future<void> _refreshJobDetails() async {
    final useCase = ref.read(getJobDetailsUseCaseProvider);
    final result = await useCase(GetJobDetailsParams(jobId: widget.jobId));
    if (!mounted) return;
    result.fold((_) {}, (jobDetails) {
      setState(() {
        _jobDetails = jobDetails;
      });
    });
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
        JobAnalyticsEvents.logJobViewed(
          ref.read(analyticsServiceProvider),
          jobId: widget.jobId,
          jobTitle: jobDetails.jobTitle,
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          backgroundColor: AppTheme.secondaryRed,
          title: const NavbarLogo(),
        ),
        body: const Center(child: CircularProgressIndicator()),
      );
    }

    if (_error != null || _jobDetails == null) {
      return Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          backgroundColor: AppTheme.secondaryRed,
          title: const NavbarLogo(),
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
    final tabCount = showTimesheetPunchcard ? 2 : 1;

    return DefaultTabController(
      length: tabCount,
      initialIndex:
          widget.initialTab == JobPageTab.punchCard && showTimesheetPunchcard
          ? 1
          : 0,
      child: Scaffold(
        backgroundColor: AppTheme.surfaceGrey,
        appBar: AppBar(
          title: const NavbarLogo(),
          backgroundColor: AppTheme.secondaryRed,
          leading: IconButton(
            tooltip: 'Back',
            icon: const Icon(Icons.arrow_back, color: Colors.white),
            onPressed: () {
              notifyLogoFlash();
              Navigator.of(context).pop();
            },
          ),
          bottom: AppTabBar(
            tabs: [
              const AppTabItem(icon: Icons.work_outline, label: 'DETAIL'),
              if (showTimesheetPunchcard)
                const AppTabItem(icon: Icons.access_time, label: 'PUNCH CARD'),
            ],
          ),
        ),
        body: TabBarView(
          children: [
            JobDetailsTab(
              jobDetails: jobDetails,
              onApplySuccess: () {
                context.go(AppRoutes.jobs);
              },
            ),
            if (showTimesheetPunchcard) ...[
              PunchCardTab(
                jobDetails: jobDetails,
                onRefreshCheckpoint: _refreshJobDetails,
              ),
            ],
          ],
        ),
      ),
    );
  }
}
