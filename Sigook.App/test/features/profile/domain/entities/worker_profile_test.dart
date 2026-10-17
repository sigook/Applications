import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/profile/data/models/worker_profile_model.dart';
import 'package:sigook_app_flutter/features/profile/domain/entities/worker_profile.dart';
import 'package:sigook_app_flutter/features/profile/domain/entities/worker_requirement.dart';

void main() {
  group('WorkerRequirement', () {
    test('maps every backend code', () {
      for (final value in WorkerRequirement.values) {
        expect(WorkerRequirement.fromCode(value.code), value);
      }
    });

    test('unknown code returns null', () {
      expect(WorkerRequirement.fromCode('Something'), isNull);
    });
  });

  group('WorkerProfile missing requirements', () {
    test('empty list means nothing is missing', () {
      const profile = WorkerProfile(id: 'worker-001');
      expect(profile.hasMissingRequirements, isFalse);
    });

    test('model maps codes to requirements and skips unknown ones', () {
      final model = WorkerProfileModel.fromJson({
        'id': 'worker-001',
        'missingRequirements': ['Identification', 'ProfileImage', 'Unknown'],
      });

      final profile = model.toEntity();

      expect(profile.missingRequirements, [
        WorkerRequirement.identification,
        WorkerRequirement.profileImage,
      ]);
      expect(profile.hasMissingRequirements, isTrue);
    });

    test('model without the field defaults to empty', () {
      final profile = WorkerProfileModel.fromJson({'id': 'worker-001'}).toEntity();
      expect(profile.missingRequirements, isEmpty);
    });
  });
}
