import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../presentation/widgets/activity_status_badge.dart';
import '../../domain/entities/assignment.dart';

class AssignmentCard extends StatelessWidget {
  final Assignment assignment;

  const AssignmentCard({super.key, required this.assignment});

  String get _dates {
    final format = DateFormat('MMM d, y');
    final start = assignment.startedAt != null
        ? format.format(assignment.startedAt!)
        : '—';
    final end = assignment.endedAt != null
        ? format.format(assignment.endedAt!)
        : 'now';
    return '$start – $end';
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      elevation: 1,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
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
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: AppTheme.textDark,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        '${assignment.clientName} · #${assignment.numberId}',
                        style: const TextStyle(
                          fontSize: 13,
                          color: AppTheme.textMedium,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 8),
                ActivityStatusBadge(status: assignment.status),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: Text(
                    _dates,
                    style: const TextStyle(
                      fontSize: 13,
                      color: AppTheme.textMedium,
                    ),
                  ),
                ),
                Text(
                  '${assignment.hoursWorked.toStringAsFixed(1)} h',
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
    );
  }
}
