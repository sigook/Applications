import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../presentation/widgets/activity_detail_row.dart';
import '../../../presentation/widgets/activity_status_badge.dart';
import '../../domain/entities/week_time_sheets.dart';
import 'day_time_sheet_format.dart';

class DayTimeSheetDetailSheet extends StatelessWidget {
  final DayTimeSheet day;

  const DayTimeSheetDetailSheet({super.key, required this.day});

  static Future<void> show(BuildContext context, DayTimeSheet day) => showModalBottomSheet(
        context: context,
        isScrollControlled: true,
        showDragHandle: true,
        backgroundColor: Colors.white,
        builder: (_) => DayTimeSheetDetailSheet(day: day),
      );

  @override
  Widget build(BuildContext context) {
    final money = NumberFormat.simpleCurrency(name: 'USD');

    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(20, 0, 20, 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        DateFormat('EEE, MMM d, yyyy').format(day.date),
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w700,
                          color: AppTheme.textDark,
                        ),
                      ),
                      if (day.subtitle.isNotEmpty)
                        Text(
                          day.subtitle,
                          style: const TextStyle(fontSize: 13, color: AppTheme.textMedium),
                        ),
                    ],
                  ),
                ),
                const SizedBox(width: 8),
                ActivityStatusBadge(status: day.statusLabel),
              ],
            ),
            const _SectionTitle('Hours'),
            ActivityDetailRow(label: 'In – Out', value: day.rangeLabel),
            ActivityDetailRow(label: 'Break', value: day.breakLabel),
            ActivityDetailRow(label: 'Regular', value: day.regularHours.toStringAsFixed(1)),
            ActivityDetailRow(label: 'Overtime', value: day.overtimeHours.toStringAsFixed(1)),
            ActivityDetailRow(label: 'Total', value: day.totalHours.toStringAsFixed(1), isBold: true),
            if (day.hasAdjustments) ...[
              const _SectionTitle('Adjustments'),
              if (day.missingHours > Duration.zero)
                ActivityDetailRow(label: 'Missing hours', value: durationHoursLabel(day.missingHours)),
              if (day.missingOvertime > Duration.zero)
                ActivityDetailRow(label: 'Missing overtime', value: durationHoursLabel(day.missingOvertime)),
              if (day.bonus > 0)
                ActivityDetailRow(label: 'Bonus / others', value: money.format(day.bonus), hint: day.bonusDescription),
              if (day.deductions > 0)
                ActivityDetailRow(
                  label: 'Deductions',
                  value: '−${money.format(day.deductions)}',
                  hint: day.deductionsDescription,
                ),
              if (day.reimbursements > 0)
                ActivityDetailRow(
                  label: 'Reimbursements',
                  value: money.format(day.reimbursements),
                  hint: day.reimbursementsDescription,
                ),
            ],
          ],
        ),
      ),
    );
  }
}

class _SectionTitle extends StatelessWidget {
  final String text;

  const _SectionTitle(this.text);

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 16, bottom: 4),
      child: Text(
        text,
        style: const TextStyle(fontSize: 14, fontWeight: FontWeight.w700, color: AppTheme.textDark),
      ),
    );
  }
}
