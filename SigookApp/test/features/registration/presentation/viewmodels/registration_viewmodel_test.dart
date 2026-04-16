import 'package:dartz/dartz.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/providers/analytics_providers.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/basic_info.dart';
import 'package:sigook_app_flutter/features/registration/domain/entities/registration_form.dart';
import 'package:sigook_app_flutter/features/registration/domain/repositories/registration_repository.dart';
import 'package:sigook_app_flutter/features/registration/domain/usecases/submit_registration.dart';
import 'package:sigook_app_flutter/features/registration/presentation/providers/registration_providers.dart';

import '../../../../helpers/mocks.dart';
import '../../../../helpers/riverpod_test_helpers.dart';

// ── Additional mocks ─────────────────────────────────────────────────────────
class MockRegistrationRepository extends Mock
    implements RegistrationRepository {}

class MockSubmitRegistration extends Mock implements SubmitRegistration {}

void main() {
  setUpAll(() {
    registerFallbackValue(RegistrationForm.empty());
  });

  late MockRegistrationRepository mockRepo;
  late MockSubmitRegistration mockSubmit;
  late MockAnalyticsService mockAnalytics;

  setUp(() {
    mockRepo = MockRegistrationRepository();
    mockSubmit = MockSubmitRegistration();
    mockAnalytics = MockAnalyticsService();

    // build() calls _loadDraft() — stub it
    when(() => mockRepo.getDraft())
        .thenAnswer((_) async => const Right(RegistrationForm()));

    // saveDraft always succeeds
    when(() => mockRepo.saveDraft(any()))
        .thenAnswer((_) async => const Right(null));

    // Analytics stubs
    when(() => mockAnalytics.logEvent(
          name: any(named: 'name'),
          parameters: any(named: 'parameters'),
        )).thenAnswer((_) async {});
  });

  buildTestContainer() => buildContainer(ProviderContainer(overrides: [
        registrationRepositoryProvider.overrideWithValue(mockRepo),
        submitRegistrationUseCaseProvider.overrideWithValue(mockSubmit),
        analyticsServiceProvider.overrideWithValue(mockAnalytics),
      ]));

  // ── updateBasicInfo (step 0) ───────────────────────────────────────────────

  group('updateBasicInfo', () {
    test('fires registration_step_completed analytics for step 0', () async {
      final container = buildTestContainer();
      await Future<void>.delayed(Duration.zero); // let _loadDraft complete

      container.read(registrationViewModelProvider.notifier).updateBasicInfo(
            BasicInfo.empty(),
          );

      verify(() => mockAnalytics.logEvent(
            name: 'registration_step_completed',
            parameters: any(named: 'parameters'),
          )).called(1);
    });
  });

  // ── submitRegistration ─────────────────────────────────────────────────────

  group('submitRegistration', () {
    test('fires registration_completed and returns Success on success',
        () async {
      final container = buildTestContainer();
      await Future<void>.delayed(Duration.zero);

      when(() => mockSubmit.call(any()))
          .thenAnswer((_) async => const Right(null));

      final result = await container
          .read(registrationViewModelProvider.notifier)
          .submitRegistration();

      expect(result, 'Success');
      verify(() => mockAnalytics.logEvent(
            name: 'registration_completed',
            parameters: any(named: 'parameters'),
          )).called(1);
    });

    test('fires registration_failed and returns error message on failure',
        () async {
      final container = buildTestContainer();
      await Future<void>.delayed(Duration.zero);

      when(() => mockSubmit.call(any())).thenAnswer(
        (_) async =>
            const Left(ServerFailure(message: 'submission failed')),
      );

      final result = await container
          .read(registrationViewModelProvider.notifier)
          .submitRegistration();

      expect(result, 'submission failed');
      verify(() => mockAnalytics.logEvent(
            name: 'registration_failed',
            parameters: any(named: 'parameters'),
          )).called(1);
    });
  });

  // ── clearForm ──────────────────────────────────────────────────────────────

  group('clearForm', () {
    test('resets form to empty', () async {
      final container = buildTestContainer();
      await Future<void>.delayed(Duration.zero);

      when(() => mockRepo.clearDraft())
          .thenAnswer((_) async => const Right(null));

      await container
          .read(registrationViewModelProvider.notifier)
          .clearForm();

      final form = container.read(registrationViewModelProvider);
      expect(form, RegistrationForm.empty());
    });
  });
}
