import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/activity/history/data/models/assignment_model.dart';

void main() {
  group('AssignmentModel', () {
    test('prefers the start working date over the request start', () {
      final assignment = AssignmentModel.fromJson({
        'id': 'a',
        'numberId': 1671,
        'jobTitle': 'Picker',
        'clientName': 'Acme',
        'status': 'Completed',
        'startAt': '2026-10-01T00:00:00',
        'startWorking': '2026-10-05T00:00:00',
        'endedAt': '2026-10-20T00:00:00',
        'hoursWorked': 72.5,
      }).toEntity();

      expect(assignment.startedAt, DateTime(2026, 10, 5));
      expect(assignment.status, 'Completed');
      expect(assignment.hoursWorked, 72.5);
    });

    test('falls back to Ended and empty texts', () {
      final assignment = AssignmentModel.fromJson({'id': 'a', 'numberId': 1}).toEntity();

      expect(assignment.status, 'Ended');
      expect(assignment.jobTitle, '');
      expect(assignment.startedAt, isNull);
    });
  });
}
