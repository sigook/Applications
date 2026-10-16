import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/features/app_update/domain/entities/app_update_check.dart';
import 'package:sigook_app_flutter/features/app_update/domain/entities/app_version_requirement.dart';

void main() {
  AppVersionRequirement requirement(String minimum) => AppVersionRequirement(
        minimumVersion: minimum,
        androidStoreUrl: 'https://play.google.com/store/apps/details?id=x',
        iosStoreUrl: 'https://apps.apple.com/ca/app/id1',
      );

  group('AppVersionRequirement.compareVersions', () {
    test('compares segments numerically, not lexically', () {
      expect(AppVersionRequirement.compareVersions('2026.10.9', '2026.9.23'),
          greaterThan(0));
      expect(AppVersionRequirement.compareVersions('2026.9.23', '2026.10.9'),
          lessThan(0));
    });

    test('treats missing segments as zero', () {
      expect(AppVersionRequirement.compareVersions('2026.10', '2026.10.0'), 0);
      expect(AppVersionRequirement.compareVersions('2026.10.1', '2026.10'),
          greaterThan(0));
    });

    test('ignores build metadata and pre-release suffixes', () {
      expect(AppVersionRequirement.compareVersions('2026.10.9+42', '2026.10.9'),
          0);
      expect(AppVersionRequirement.compareVersions('2026.10.9-beta', '2026.10.9'),
          0);
    });

    test('non-numeric segments count as zero', () {
      expect(AppVersionRequirement.compareVersions('abc', '0.0.0'), 0);
    });
  });

  group('AppVersionRequirement.isSatisfiedBy', () {
    test('is satisfied when current equals or exceeds minimum', () {
      expect(requirement('2026.10.9').isSatisfiedBy('2026.10.9'), isTrue);
      expect(requirement('2026.10.9').isSatisfiedBy('2026.10.10'), isTrue);
      expect(requirement('2026.10.9').isSatisfiedBy('2027.1.1'), isTrue);
    });

    test('is not satisfied when current is older', () {
      expect(requirement('2026.10.9').isSatisfiedBy('2026.9.23'), isFalse);
      expect(requirement('2026.10.9').isSatisfiedBy('2026.10.8'), isFalse);
    });

    test('empty minimum never forces an update', () {
      expect(requirement('').isSatisfiedBy('1.0.0'), isTrue);
      expect(requirement('   ').isSatisfiedBy('1.0.0'), isTrue);
    });
  });

  group('AppUpdateCheck', () {
    test('updateRequired mirrors the requirement', () {
      final outdated = AppUpdateCheck(
        currentVersion: '2026.9.23',
        requirement: requirement('2026.10.9'),
      );
      final current = AppUpdateCheck(
        currentVersion: '2026.10.9',
        requirement: requirement('2026.10.9'),
      );
      expect(outdated.updateRequired, isTrue);
      expect(current.updateRequired, isFalse);
    });
  });
}
