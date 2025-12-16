import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../core/theme/app_theme.dart';

class TimesheetTab extends StatelessWidget {
  const TimesheetTab({super.key});

  @override
  Widget build(BuildContext context) {
    final mockShifts = _getMockShifts();

    return mockShifts.isEmpty
        ? Center(
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
          )
        : ListView.separated(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            itemCount: mockShifts.length,
            separatorBuilder: (context, index) => const SizedBox(height: 12),
            itemBuilder: (context, index) {
              return _buildShiftCard(mockShifts[index]);
            },
          );
  }

  Widget _buildShiftCard(ShiftRecord shift) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: AppTheme.primaryBlue.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: const Icon(
                    Icons.calendar_today,
                    color: AppTheme.primaryBlue,
                    size: 20,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        DateFormat('EEEE, MMMM dd, yyyy').format(shift.date),
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.textDark,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        shift.jobTitle,
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey.shade600,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 6,
                  ),
                  decoration: BoxDecoration(
                    color: _getStatusColor(shift.status).withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(6),
                    border: Border.all(
                      color: _getStatusColor(shift.status),
                      width: 1,
                    ),
                  ),
                  child: Text(
                    shift.status,
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: _getStatusColor(shift.status),
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: Colors.grey.shade50,
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: _buildInfoColumn(
                      'Start Time',
                      DateFormat('hh:mm a').format(shift.startTime),
                      Icons.login,
                      AppTheme.successGreen,
                    ),
                  ),
                  Container(width: 1, height: 40, color: Colors.grey.shade300),
                  Expanded(
                    child: _buildInfoColumn(
                      'End Time',
                      DateFormat('hh:mm a').format(shift.endTime),
                      Icons.logout,
                      AppTheme.errorRed,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: _buildMetricCard(
                    'Worked Hours',
                    '${shift.workedHours.toStringAsFixed(1)} hrs',
                    Icons.access_time,
                    AppTheme.primaryBlue,
                  ),
                ),
                const SizedBox(width: 8),
                Expanded(
                  child: _buildMetricCard(
                    'Approved Hours',
                    '${shift.approvedHours.toStringAsFixed(1)} hrs',
                    Icons.check_circle_outline,
                    AppTheme.successGreen,
                  ),
                ),
              ],
            ),
            if (shift.notes != null) ...[
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.amber.shade50,
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: Colors.amber.shade200),
                ),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Icon(
                      Icons.info_outline,
                      size: 18,
                      color: Colors.amber.shade800,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        shift.notes!,
                        style: TextStyle(
                          fontSize: 13,
                          color: Colors.amber.shade900,
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

  Widget _buildInfoColumn(
    String label,
    String value,
    IconData icon,
    Color color,
  ) {
    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(icon, size: 16, color: color),
            const SizedBox(width: 6),
            Text(
              label,
              style: TextStyle(
                fontSize: 12,
                color: Colors.grey.shade600,
                fontWeight: FontWeight.w500,
              ),
            ),
          ],
        ),
        const SizedBox(height: 6),
        Text(
          value,
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.bold,
            color: AppTheme.textDark,
          ),
        ),
      ],
    );
  }

  Widget _buildMetricCard(
    String label,
    String value,
    IconData icon,
    Color color,
  ) {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Column(
        children: [
          Icon(icon, size: 20, color: color),
          const SizedBox(height: 6),
          Text(
            label,
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 11,
              color: Colors.grey.shade700,
              fontWeight: FontWeight.w500,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            value,
            style: TextStyle(
              fontSize: 15,
              fontWeight: FontWeight.bold,
              color: color,
            ),
          ),
        ],
      ),
    );
  }

  Color _getStatusColor(String status) {
    switch (status.toLowerCase()) {
      case 'approved':
        return AppTheme.successGreen;
      case 'pending':
        return Colors.orange;
      case 'rejected':
        return AppTheme.errorRed;
      default:
        return Colors.grey;
    }
  }

  List<ShiftRecord> _getMockShifts() {
    final now = DateTime.now();
    return [
      ShiftRecord(
        date: now.subtract(const Duration(days: 1)),
        jobTitle: 'General Labor - Valdosta GA',
        startTime: DateTime(now.year, now.month, now.day - 1, 7, 0),
        endTime: DateTime(now.year, now.month, now.day - 1, 16, 0),
        workedHours: 8.5,
        approvedHours: 8.0,
        status: 'Approved',
      ),
      ShiftRecord(
        date: now.subtract(const Duration(days: 2)),
        jobTitle: 'General Labor - Valdosta GA',
        startTime: DateTime(now.year, now.month, now.day - 2, 7, 0),
        endTime: DateTime(now.year, now.month, now.day - 2, 15, 30),
        workedHours: 8.0,
        approvedHours: 8.0,
        status: 'Approved',
      ),
      ShiftRecord(
        date: now.subtract(const Duration(days: 3)),
        jobTitle: 'General Labor - Valdosta GA',
        startTime: DateTime(now.year, now.month, now.day - 3, 7, 0),
        endTime: DateTime(now.year, now.month, now.day - 3, 16, 15),
        workedHours: 9.0,
        approvedHours: 8.5,
        status: 'Approved',
        notes: 'Overtime approved by supervisor',
      ),
      ShiftRecord(
        date: now.subtract(const Duration(days: 4)),
        jobTitle: 'General Labor - Valdosta GA',
        startTime: DateTime(now.year, now.month, now.day - 4, 7, 0),
        endTime: DateTime(now.year, now.month, now.day - 4, 16, 0),
        workedHours: 8.5,
        approvedHours: 8.5,
        status: 'Pending',
        notes: 'Awaiting supervisor approval',
      ),
    ];
  }
}

class ShiftRecord {
  final DateTime date;
  final String jobTitle;
  final DateTime startTime;
  final DateTime endTime;
  final double workedHours;
  final double approvedHours;
  final String status;
  final String? notes;

  ShiftRecord({
    required this.date,
    required this.jobTitle,
    required this.startTime,
    required this.endTime,
    required this.workedHours,
    required this.approvedHours,
    required this.status,
    this.notes,
  });
}
