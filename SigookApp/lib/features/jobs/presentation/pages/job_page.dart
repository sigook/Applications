import 'package:flutter/material.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../domain/entities/job.dart';
import '../widgets/job_details_tab.dart';
import '../widgets/punch_card_tab.dart';
import '../widgets/timesheet_tab.dart';
import '../widgets/app_drawer.dart';

class JobPage extends StatelessWidget {
  final Job job;

  const JobPage({super.key, required this.job});

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 3,
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
                    'Job #${job.numberId}',
                    style: const TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.w600,
                      fontSize: 18,
                    ),
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
                              job.jobTitle,
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
                              job.agencyFullName,
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
                    ),
                  ),
                ),
              ),
            ];
          },
          body: TabBarView(
            children: [
              JobDetailsTab(job: job),
              const PunchCardTab(),
              const TimesheetTab(),
            ],
          ),
        ),
        endDrawer: const AppDrawer(currentRoute: AppRoutes.jobs),
      ),
    );
  }
}
