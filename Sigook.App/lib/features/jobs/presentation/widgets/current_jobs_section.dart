import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';
import '../../../../core/routing/app_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../core/widgets/buttons/open_punch_card_button.dart';
import '../../../activity/history/domain/entities/assignment.dart';
import '../../../activity/history/presentation/providers/history_providers.dart';
import '../../../activity/presentation/widgets/activity_status_badge.dart';

class CurrentJobsSection extends ConsumerWidget {
  const CurrentJobsSection({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final assignments = ref.watch(activeAssignmentsProvider).value ?? [];
    if (assignments.isEmpty) return const SizedBox.shrink();

    return Column(
      children: [
        for (final assignment in assignments)
          _CurrentJobCard(assignment: assignment),
      ],
    );
  }
}

class _CurrentJobCard extends StatelessWidget {
  final Assignment assignment;

  const _CurrentJobCard({required this.assignment});

  @override
  Widget build(BuildContext context) {
    final subtitle = [
      assignment.clientName,
      if (assignment.location != null && assignment.location!.isNotEmpty)
        assignment.location!,
    ].join(' · ');

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      elevation: 1,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(16),
        side: const BorderSide(color: AppTheme.successGreen, width: 2),
      ),
      child: Semantics(
        button: true,
        child: InkWell(
          borderRadius: BorderRadius.circular(16),
          onTap: () => context.push(AppRoutes.jobDetails, extra: assignment.id),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            assignment.jobTitle,
                            style: const TextStyle(
                              fontSize: 17,
                              fontWeight: FontWeight.w700,
                              color: AppTheme.textDark,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            subtitle,
                            style: const TextStyle(
                              fontSize: 13,
                              color: AppTheme.textMedium,
                            ),
                          ),
                          if (assignment.startedAt != null) ...[
                            const SizedBox(height: 2),
                            Text(
                              'Since ${DateFormat('MMM d, y').format(assignment.startedAt!)} · #${assignment.numberId}',
                              style: const TextStyle(
                                fontSize: 13,
                                color: AppTheme.textMedium,
                              ),
                            ),
                          ],
                        ],
                      ),
                    ),
                    const SizedBox(width: 8),
                    ActivityStatusBadge(status: assignment.status),
                  ],
                ),
                const SizedBox(height: 12),
                OpenPunchCardButton(jobId: assignment.id),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
