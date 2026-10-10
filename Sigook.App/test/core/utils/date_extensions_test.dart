import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/core/utils/date_extensions.dart';

void main() {
  group('DateExtensions', () {
    test('formatDate returns correct format', () {
      final date = DateTime(2024, 12, 25, 14, 30);
      expect(date.formatDate(), '12/25/2024');
    });

    test('formatTime returns correct 12-hour format', () {
      final morning = DateTime(2024, 1, 1, 9, 30);
      final afternoon = DateTime(2024, 1, 1, 14, 30);
      final midnight = DateTime(2024, 1, 1, 0, 0);
      final noon = DateTime(2024, 1, 1, 12, 0);

      expect(morning.formatTime(), '9:30 AM');
      expect(afternoon.formatTime(), '2:30 PM');
      expect(midnight.formatTime(), '12:00 AM');
      expect(noon.formatTime(), '12:00 PM');
    });

    test('formatDateTime combines date and time', () {
      final dateTime = DateTime(2024, 12, 25, 14, 30);
      expect(dateTime.formatDateTime(), '12/25/2024 2:30 PM');
    });

    test('toRelativeString returns correct relative time', () {
      final now = DateTime.now();

      // Just now
      final justNow = now.subtract(const Duration(seconds: 30));
      expect(justNow.toRelativeString(), 'Just now');

      // Minutes ago
      final fiveMinutesAgo = now.subtract(const Duration(minutes: 5));
      expect(fiveMinutesAgo.toRelativeString(), '5 minutes ago');

      // Hours ago
      final twoHoursAgo = now.subtract(const Duration(hours: 2));
      expect(twoHoursAgo.toRelativeString(), '2 hours ago');

      // Yesterday
      final yesterday = now.subtract(const Duration(days: 1));
      expect(yesterday.toRelativeString(), 'Yesterday');

      // Days ago
      final threeDaysAgo = now.subtract(const Duration(days: 3));
      expect(threeDaysAgo.toRelativeString(), '3 days ago');
    });

    test('isToday returns true for today', () {
      final now = DateTime.now();
      expect(now.isToday, true);

      final yesterday = now.subtract(const Duration(days: 1));
      expect(yesterday.isToday, false);
    });

    test('isYesterday returns true for yesterday', () {
      final yesterday = DateTime.now().subtract(const Duration(days: 1));
      expect(yesterday.isYesterday, true);

      final today = DateTime.now();
      expect(today.isYesterday, false);
    });

    test('isSameDay returns true for same calendar day', () {
      final date1 = DateTime(2024, 12, 25, 10, 0);
      final date2 = DateTime(2024, 12, 25, 20, 0);
      final date3 = DateTime(2024, 12, 26, 10, 0);

      expect(date1.isSameDay(date2), true);
      expect(date1.isSameDay(date3), false);
    });

    test('daysBetween calculates correct day difference', () {
      final date1 = DateTime(2024, 1, 1);
      final date2 = DateTime(2024, 1, 10);

      expect(date1.daysBetween(date2), 9);
      expect(date2.daysBetween(date1), -9);
    });

    test('addDays adds correct number of days', () {
      final date = DateTime(2024, 1, 1);
      final result = date.addDays(10);

      expect(result.year, 2024);
      expect(result.month, 1);
      expect(result.day, 11);
    });

    test('startOfDay returns midnight', () {
      final dateTime = DateTime(2024, 12, 25, 14, 30, 45);
      final start = dateTime.startOfDay;

      expect(start.hour, 0);
      expect(start.minute, 0);
      expect(start.second, 0);
      expect(start.millisecond, 0);
    });

    test('endOfDay returns last moment of day', () {
      final dateTime = DateTime(2024, 12, 25, 14, 30);
      final end = dateTime.endOfDay;

      expect(end.hour, 23);
      expect(end.minute, 59);
      expect(end.second, 59);
      expect(end.millisecond, 999);
    });
  });
}
