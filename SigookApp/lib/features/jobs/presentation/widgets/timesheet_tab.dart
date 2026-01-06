import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_theme.dart';
import '../../domain/entities/timesheet_entry.dart';
import '../viewmodels/timesheet_viewmodel.dart';

class TimesheetTab extends ConsumerWidget {
  final String jobId;

  const TimesheetTab({super.key, required this.jobId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    debugPrint('\n🟪 [TIMESHEET TAB] ===== BUILD METHOD CALLED =====');
    debugPrint('🟪 [TIMESHEET TAB] JobId: $jobId');
    debugPrint(
      '🟪 [TIMESHEET TAB] Watching timesheetViewModelProvider($jobId)...',
    );

    final timesheetState = ref.watch(timesheetViewModelProvider(jobId));

    debugPrint('🟪 [TIMESHEET TAB] State received:');
    debugPrint('  - isLoading: ${timesheetState.isLoading}');
    debugPrint('  - entries.length: ${timesheetState.entries.length}');
    debugPrint('  - error: ${timesheetState.error}');

    if (timesheetState.isLoading && timesheetState.entries.isEmpty) {
      return const Center(
        child: CircularProgressIndicator(color: AppTheme.primaryBlue),
      );
    }

    if (timesheetState.error != null && timesheetState.entries.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.error_outline, size: 80, color: Colors.grey.shade400),
            const SizedBox(height: 16),
            Text(
              'Failed to Load Timesheet',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w600,
                color: Colors.grey.shade600,
              ),
            ),
            const SizedBox(height: 8),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 32),
              child: Text(
                timesheetState.error!,
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 14, color: Colors.grey.shade500),
              ),
            ),
            const SizedBox(height: 16),
            ElevatedButton.icon(
              onPressed: () {
                ref
                    .read(timesheetViewModelProvider(jobId).notifier)
                    .loadTimesheetEntries(refresh: true);
              },
              icon: const Icon(Icons.refresh),
              label: const Text('Retry'),
              style: ElevatedButton.styleFrom(
                backgroundColor: AppTheme.primaryBlue,
                foregroundColor: Colors.white,
              ),
            ),
          ],
        ),
      );
    }

    if (timesheetState.entries.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.assignment_outlined,
              size: 80,
              color: Colors.grey.shade400,
            ),
            const SizedBox(height: 16),
            Text(
              'No Timesheet Records',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.w600,
                color: Colors.grey.shade600,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Your completed shifts will appear here',
              style: TextStyle(fontSize: 14, color: Colors.grey.shade500),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: () => ref
          .read(timesheetViewModelProvider(jobId).notifier)
          .loadTimesheetEntries(refresh: true),
      color: AppTheme.primaryBlue,
      child: ListView.separated(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        itemCount:
            timesheetState.entries.length +
            (timesheetState.isLoadingMore ? 1 : 0),
        separatorBuilder: (context, index) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          if (index == timesheetState.entries.length) {
            return const Center(
              child: Padding(
                padding: EdgeInsets.all(16.0),
                child: CircularProgressIndicator(color: AppTheme.primaryBlue),
              ),
            );
          }
          return _buildShiftCard(timesheetState.entries[index]);
        },
      ),
    );
  }

  Widget _buildShiftCard(TimesheetEntry entry) {
    final dateFormat = DateFormat('EEE, MMM dd, yyyy');
    final timeFormat = DateFormat('h:mm a');

    return Card(
      elevation: 1,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(
          color: entry.wasApproved
              ? Colors.green.withOpacity(0.3)
              : Colors.grey.withOpacity(0.2),
          width: 1,
        ),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header: Date and Status
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Row(
                    children: [
                      Icon(
                        Icons.calendar_today,
                        size: 16,
                        color: AppTheme.primaryBlue,
                      ),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          dateFormat.format(entry.day),
                          style: const TextStyle(
                            fontSize: 15,
                            fontWeight: FontWeight.w600,
                            color: AppTheme.textDark,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 4,
                  ),
                  decoration: BoxDecoration(
                    color: entry.wasApproved
                        ? Colors.green.withOpacity(0.1)
                        : Colors.orange.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(
                      color: entry.wasApproved ? Colors.green : Colors.orange,
                      width: 1,
                    ),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(
                        entry.wasApproved ? Icons.check_circle : Icons.pending,
                        size: 14,
                        color: entry.wasApproved ? Colors.green : Colors.orange,
                      ),
                      const SizedBox(width: 4),
                      Text(
                        entry.wasApproved ? 'Approved' : 'Pending',
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.w600,
                          color: entry.wasApproved
                              ? Colors.green
                              : Colors.orange,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            // Clock In/Out Times
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.grey.shade50,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.all(6),
                          decoration: BoxDecoration(
                            color: Colors.green.withOpacity(0.1),
                            borderRadius: BorderRadius.circular(6),
                          ),
                          child: Icon(
                            Icons.login,
                            size: 18,
                            color: Colors.green.shade700,
                          ),
                        ),
                        const SizedBox(width: 8),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Clock In',
                              style: TextStyle(
                                fontSize: 11,
                                color: Colors.grey.shade600,
                              ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              entry.clockIn != null
                                  ? timeFormat.format(entry.clockIn!)
                                  : 'N/A',
                              style: const TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w600,
                                color: AppTheme.textDark,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                  Container(width: 1, height: 40, color: Colors.grey.shade300),
                  Expanded(
                    child: Row(
                      children: [
                        const SizedBox(width: 12),
                        Container(
                          padding: const EdgeInsets.all(6),
                          decoration: BoxDecoration(
                            color: Colors.red.withOpacity(0.1),
                            borderRadius: BorderRadius.circular(6),
                          ),
                          child: Icon(
                            Icons.logout,
                            size: 18,
                            color: Colors.red.shade700,
                          ),
                        ),
                        const SizedBox(width: 8),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'Clock Out',
                              style: TextStyle(
                                fontSize: 11,
                                color: Colors.grey.shade600,
                              ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              entry.clockOut != null
                                  ? timeFormat.format(entry.clockOut!)
                                  : 'In Progress',
                              style: TextStyle(
                                fontSize: 14,
                                fontWeight: FontWeight.w600,
                                color: entry.clockOut != null
                                    ? AppTheme.textDark
                                    : Colors.orange,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 12),

            // Hours Information
            Row(
              children: [
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: AppTheme.primaryBlue.withOpacity(0.08),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Column(
                      children: [
                        Text(
                          'Total Hours',
                          style: TextStyle(
                            fontSize: 11,
                            color: Colors.grey.shade700,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          entry.totalHours > 0
                              ? '${entry.totalHours.toStringAsFixed(1)} hrs'
                              : '0.0 hrs',
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.primaryBlue,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: Colors.green.withOpacity(0.08),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Column(
                      children: [
                        Text(
                          'Approved',
                          style: TextStyle(
                            fontSize: 11,
                            color: Colors.grey.shade700,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          entry.totalHoursApproved > 0
                              ? '${entry.totalHoursApproved.toStringAsFixed(1)} hrs'
                              : '0.0 hrs',
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.bold,
                            color: Colors.green,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            ),

            // Comment (if exists)
            if (entry.comment != null && entry.comment!.isNotEmpty) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: Colors.blue.shade50,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Icon(Icons.comment, size: 16, color: Colors.blue.shade700),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        entry.comment!,
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey.shade800,
                          height: 1.4,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
