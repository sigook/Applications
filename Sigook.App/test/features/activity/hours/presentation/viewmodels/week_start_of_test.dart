import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/activity/hours/presentation/viewmodels/hours_viewmodel.dart';

void main() {
  group('weekStartOf', () {
    test('returns the previous Sunday at midnight', () {
      expect(weekStartOf(DateTime(2026, 10, 28, 15, 20)), DateTime(2026, 10, 25));
    });

    test('keeps Sunday as its own week start', () {
      expect(weekStartOf(DateTime(2026, 10, 25, 23)), DateTime(2026, 10, 25));
    });

    test('moves Saturday back six days', () {
      expect(weekStartOf(DateTime(2026, 10, 31)), DateTime(2026, 10, 25));
    });
  });
}
