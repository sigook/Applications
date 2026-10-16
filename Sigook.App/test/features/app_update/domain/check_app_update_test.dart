import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/app_update/domain/entities/app_version_requirement.dart';
import 'package:sigook_app_flutter/features/app_update/domain/repositories/app_update_repository.dart';
import 'package:sigook_app_flutter/features/app_update/domain/usecases/check_app_update.dart';

class MockAppUpdateRepository extends Mock implements AppUpdateRepository {}

void main() {
  late MockAppUpdateRepository repository;
  late CheckAppUpdate useCase;

  const requirement = AppVersionRequirement(
    minimumVersion: '2026.10.9',
    androidStoreUrl: 'https://play.google.com/store/apps/details?id=x',
    iosStoreUrl: 'https://apps.apple.com/ca/app/id1',
  );

  setUp(() {
    repository = MockAppUpdateRepository();
    useCase = CheckAppUpdate(repository);
  });

  test('flags an outdated build', () async {
    when(() => repository.getVersionRequirement())
        .thenAnswer((_) async => const Right(requirement));

    final result = await useCase('2026.9.23');

    expect(result.isRight(), isTrue);
    result.fold(
      (_) => fail('expected a check'),
      (check) {
        expect(check.updateRequired, isTrue);
        expect(check.requirement, requirement);
        expect(check.currentVersion, '2026.9.23');
      },
    );
  });

  test('accepts a build at or above the minimum', () async {
    when(() => repository.getVersionRequirement())
        .thenAnswer((_) async => const Right(requirement));

    final result = await useCase('2026.10.9');

    result.fold(
      (_) => fail('expected a check'),
      (check) => expect(check.updateRequired, isFalse),
    );
  });

  test('propagates repository failures', () async {
    when(() => repository.getVersionRequirement())
        .thenAnswer((_) async => const Left(NetworkFailure()));

    final result = await useCase('2026.10.9');

    expect(result.isLeft(), isTrue);
    verify(() => repository.getVersionRequirement()).called(1);
  });
}
