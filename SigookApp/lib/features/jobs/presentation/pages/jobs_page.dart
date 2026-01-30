import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/routing/app_router.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../viewmodels/jobs_viewmodel.dart';
import '../widgets/job_card.dart';
import '../widgets/app_drawer.dart';
import '../widgets/filter_sort_bottom_sheet.dart';
import '../../domain/entities/job.dart';

class JobsPage extends ConsumerStatefulWidget {
  const JobsPage({super.key});

  @override
  ConsumerState<JobsPage> createState() => _JobsPageState();
}

class _JobsPageState extends ConsumerState<JobsPage> {
  final ScrollController _scrollController = ScrollController();
  final GlobalKey<ScaffoldState> _scaffoldKey = GlobalKey<ScaffoldState>();
  SortOption _currentSort = SortOption.dateNewest;
  FilterStatus _currentFilter = FilterStatus.all;

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

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent * 0.8) {
      ref.read(jobsViewModelProvider.notifier).loadMore();
    }
  }

  Future<void> _onRefresh() async {
    await ref.read(jobsViewModelProvider.notifier).refresh();
  }

  void _showFilterModal() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => FilterSortBottomSheet(
        currentSort: _currentSort,
        currentFilter: _currentFilter,
        onApply: (sort, filter) {
          setState(() {
            _currentSort = sort;
            _currentFilter = filter;
          });
        },
      ),
    );
  }

  List<Job> _getFilteredAndSortedJobs(List<Job> jobs) {
    var filtered = List<Job>.from(jobs);

    if (_currentFilter != FilterStatus.all) {
      filtered = filtered.where((job) {
        switch (_currentFilter) {
          case FilterStatus.open:
            return job.status?.toLowerCase() == 'open';
          case FilterStatus.booked:
            return job.status?.toLowerCase() == 'booked';
          case FilterStatus.pending:
            return job.status?.toLowerCase() == 'pending';
          case FilterStatus.cancelled:
            return job.status?.toLowerCase() == 'cancelled';
          default:
            return true;
        }
      }).toList();
    }

    filtered.sort((a, b) {
      switch (_currentSort) {
        case SortOption.dateNewest:
          return b.createdAt.compareTo(a.createdAt);
        case SortOption.dateOldest:
          return a.createdAt.compareTo(b.createdAt);
        case SortOption.rateHighest:
          return b.workerRate.compareTo(a.workerRate);
        case SortOption.rateLowest:
          return a.workerRate.compareTo(b.workerRate);
        case SortOption.workersHighest:
          return b.workersQuantity.compareTo(a.workersQuantity);
        case SortOption.workersLowest:
          return a.workersQuantity.compareTo(b.workersQuantity);
      }
    });

    return filtered;
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(jobsViewModelProvider);
    final filteredJobs = _getFilteredAndSortedJobs(state.jobs);

    return Scaffold(
      key: _scaffoldKey,
      backgroundColor: AppTheme.surfaceGrey,
      endDrawer: const AppDrawer(currentRoute: AppRoutes.jobs),
      appBar: AppBar(
        backgroundColor: AppTheme.primaryBlue,
        foregroundColor: Colors.white,
        elevation: 0,
        leading: IconButton(
          icon: const Icon(Icons.filter_list),
          onPressed: _showFilterModal,
        ),
        title: const Text(
          'Jobs',
          style: TextStyle(fontSize: 20, fontWeight: FontWeight.w600),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.menu),
            onPressed: () {
              _scaffoldKey.currentState?.openEndDrawer();
            },
          ),
        ],
      ),
      body: state.isLoading && state.jobs.isEmpty
          ? const Center(
              child: CircularProgressIndicator(color: AppTheme.primaryBlue),
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
                      onPressed: () {
                        ref.read(authViewModelProvider.notifier).logout();
                      },
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
          : filteredJobs.isEmpty
          ? Center(
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
                    _currentFilter != FilterStatus.all
                        ? 'No jobs match the filter'
                        : 'No jobs available',
                    style: TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.w600,
                      color: Colors.grey.shade700,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    _currentFilter != FilterStatus.all
                        ? 'Try adjusting your filters'
                        : 'Check back later for new opportunities',
                    style: TextStyle(fontSize: 14, color: Colors.grey.shade600),
                  ),
                  if (_currentFilter != FilterStatus.all) ...[
                    const SizedBox(height: 16),
                    TextButton(
                      onPressed: () {
                        setState(() {
                          _currentFilter = FilterStatus.all;
                        });
                      },
                      child: const Text('Clear Filter'),
                    ),
                  ],
                ],
              ),
            )
          : RefreshIndicator(
              onRefresh: _onRefresh,
              color: AppTheme.primaryBlue,
              child: ListView.builder(
                controller: _scrollController,
                padding: const EdgeInsets.only(top: 8, bottom: 16),
                itemCount: filteredJobs.length + (state.isLoadingMore ? 1 : 0),
                itemBuilder: (context, index) {
                  if (index == filteredJobs.length) {
                    return const Padding(
                      padding: EdgeInsets.all(16),
                      child: Center(
                        child: CircularProgressIndicator(
                          color: AppTheme.primaryBlue,
                        ),
                      ),
                    );
                  }
                  return JobCard(job: filteredJobs[index]);
                },
              ),
            ),
    );
  }
}
