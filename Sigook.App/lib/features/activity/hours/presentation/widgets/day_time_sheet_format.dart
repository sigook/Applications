import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../domain/entities/week_time_sheets.dart';

extension DayTimeSheetFormat on DayTimeSheet {
  String get statusLabel {
    final today = DateUtils.dateOnly(DateTime.now());
    if (status == 'Open' && DateUtils.dateOnly(date).isBefore(today)) return 'No clock-out';
    return status;
  }

  String get rangeLabel {
    if (clockIn == null && clockOut == null) return 'Manual entry';
    final time = DateFormat('HH:mm');
    final from = clockIn == null ? '—' : time.format(clockIn!);
    final to = clockOut == null ? '—' : time.format(clockOut!);
    return '$from – $to';
  }

  String get subtitle => [
        if (clientName.isNotEmpty) clientName,
        if (jobTitle.isNotEmpty) jobTitle,
      ].join(' · ');

  String get breakLabel {
    final minutes = breakDuration.inMinutes;
    if (minutes == 0) return '—';
    return breakIsPaid ? '$minutes min (paid)' : '$minutes min';
  }

  bool get hasAdjustments =>
      missingHours > Duration.zero ||
      missingOvertime > Duration.zero ||
      bonus > 0 ||
      deductions > 0 ||
      reimbursements > 0;
}

String durationHoursLabel(Duration duration) =>
    '${(duration.inMinutes / 60).toStringAsFixed(1)} h';
