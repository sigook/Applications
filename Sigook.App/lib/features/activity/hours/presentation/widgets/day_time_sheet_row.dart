import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../../../../core/theme/app_theme.dart';
import '../../../presentation/widgets/activity_detail_row.dart';
import '../../../presentation/widgets/activity_status_badge.dart';
import '../../domain/entities/week_time_sheets.dart';
import 'day_time_sheet_detail_sheet.dart';
import 'day_time_sheet_format.dart';

class DayTimeSheetRow extends StatelessWidget {
  final DayTimeSheet day;

  const DayTimeSheetRow({super.key, required this.day});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      elevation: 0,
      color: Colors.white,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
        side: const BorderSide(color: AppTheme.borderLight),
      ),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: () => DayTimeSheetDetailSheet.show(context, day),
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            children: [
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          DateFormat('EEE, MMM d').format(day.date),
                          style: const TextStyle(
                            fontSize: 15,
                            fontWeight: FontWeight.w600,
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
              const SizedBox(height: 8),
              ActivityDetailRow(label: 'In – Out', value: day.rangeLabel),
              ActivityDetailRow(
                label: 'Regular · OT',
                value: '${day.regularHours.toStringAsFixed(1)} · ${day.overtimeHours.toStringAsFixed(1)}',
              ),
              ActivityDetailRow(label: 'Total', value: day.totalHours.toStringAsFixed(1), isBold: true),
            ],
          ),
        ),
      ),
    );
  }
}
