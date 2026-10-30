import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/activity/hours/data/models/week_time_sheets_model.dart';

void main() {
  group('WeekTimeSheetsModel', () {
    test('maps the week response to the entity', () {
      final model = WeekTimeSheetsModel.fromJson({
        'weekStart': '2026-10-25T00:00:00',
        'weekEnd': '2026-10-31T00:00:00',
        'hours': {'total': 46.0, 'regular': 44.0, 'overtime': 2.0, 'holiday': 0.0, 'pendingApproval': 8.0},
        'items': [
          {
            'id': '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            'requestId': '3fa85f64-5717-4562-b3fc-2c963f66afa7',
            'date': '2026-10-26T00:00:00',
            'clientName': 'Acme',
            'jobTitle': 'Picker',
            'clockIn': '2026-10-26T07:00:00',
            'clockOut': '2026-10-26T15:30:00',
            'timeIn': '0001-01-01T07:00:00',
            'durationBreak': '00:30:00',
            'breakIsPaid': false,
            'regularHours': 8.0,
            'overtimeHours': 0.0,
            'holidayHours': 0.0,
            'totalHours': 8.0,
            'status': 3,
          },
        ],
      });

      final week = model.toEntity();

      expect(week.weekStart, DateTime(2026, 10, 25));
      expect(week.totalHours, 46.0);
      expect(week.overtimeHours, 2.0);
      expect(week.pendingApprovalHours, 8.0);
      expect(week.days, hasLength(1));
      final day = week.days.single;
      expect(day.clientName, 'Acme');
      expect(day.clockIn, DateTime(2026, 10, 26, 7));
      expect(day.breakDuration, const Duration(minutes: 30));
      expect(day.status, 'Approved');
    });

    test('defaults missing values', () {
      final week = WeekTimeSheetsModel.fromJson({'weekStart': '2026-10-25T00:00:00'}).toEntity();

      expect(week.totalHours, 0);
      expect(week.days, isEmpty);
    });

    test('maps unknown break and status to safe defaults', () {
      final day = DayTimeSheetModel.fromJson({
        'id': 'a',
        'date': '2026-10-26T00:00:00',
        'durationBreak': null,
        'status': 9,
      }).toEntity();

      expect(day.breakDuration, Duration.zero);
      expect(day.status, 'Open');
    });
  });
}
