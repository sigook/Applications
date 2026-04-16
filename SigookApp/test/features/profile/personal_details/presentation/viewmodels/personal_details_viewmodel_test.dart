import 'package:dartz/dartz.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/core/providers/analytics_providers.dart';
import 'package:sigook_app_flutter/features/profile/personal_details/domain/usecases/update_personal_details.dart';
import 'package:sigook_app_flutter/features/profile/personal_details/presentation/providers/personal_details_providers.dart';
import 'package:sigook_app_flutter/features/profile/personal_details/presentation/viewmodels/personal_details_viewmodel.dart';
import 'package:sigook_app_flutter/features/profile/presentation/providers/cached_worker_profile_provider.dart';
import 'package:sigook_app_flutter/features/profile/presentation/providers/profile_providers.dart';

import '../../../../../helpers/mocks.dart';
import '../../../../../helpers/riverpod_test_helpers.dart';

class MockUpdatePersonalDetails extends Mock implements UpdatePersonalDetails {}

void main() {
  late MockUpdatePersonalDetails mockUseCase;
  late MockAnalyticsService mockAnalytics;
  late MockProfileRepository mockProfileRepo;

  const tFields = {'firstName': 'John', 'lastName': 'Doe'};

  setUp(() {
    mockUseCase = MockUpdatePersonalDetails();
    mockAnalytics = MockAnalyticsService();
    mockProfileRepo = MockProfileRepository();

    // Analytics stub
    when(() => mockAnalytics.logEvent(
          name: any(named: 'name'),
          parameters: any(named: 'parameters'),
        )).thenAnswer((_) async {});
  });

  buildTestContainer() => buildContainer(ProviderContainer(overrides: [
        updatePersonalDetailsUseCaseProvider.overrideWithValue(mockUseCase),
        analyticsServiceProvider.overrideWithValue(mockAnalytics),
        // Override profile repo so cachedWorkerProfileProvider doesn't hit network
        profileRepositoryProvider.overrideWithValue(mockProfileRepo),
      ]));

  // ── save() — success ───────────────────────────────────────────────────────

  group('save() success', () {
    setUp(() {
      when(() => mockUseCase.call(any()))
          .thenAnswer((_) async => const Right(null));
      when(() => mockProfileRepo.getWorkerBasicInfo())
          .thenAnswer((_) async => const Left(NetworkFailure()));
    });

    test('sets justSaved=true and isSaving=false', () async {
      final container = buildTestContainer();

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      final state = container.read(personalDetailsViewModelProvider);
      expect(state.justSaved, true);
      expect(state.isSaving, false);
      expect(state.saveError, isNull);
    });

    test('sets isEditing=false after save', () async {
      final container = buildTestContainer();
      container
          .read(personalDetailsViewModelProvider.notifier)
          .startEditing();

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      expect(
        container.read(personalDetailsViewModelProvider).isEditing,
        false,
      );
    });

    test('fires profile_section_saved analytics event', () async {
      final container = buildTestContainer();

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      verify(() => mockAnalytics.logEvent(
            name: 'profile_section_saved',
            parameters: any(named: 'parameters'),
          )).called(1);
    });

    test('invalidates cachedWorkerProfileProvider after save', () async {
      final container = buildTestContainer();

      // Listen to cachedWorkerProfileProvider to confirm it gets rebuilt
      var rebuildCount = 0;
      container.listen(cachedWorkerProfileProvider, (prev, next) {
        rebuildCount++;
      });

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      expect(rebuildCount, greaterThan(0));
    });
  });

  // ── save() — failure ───────────────────────────────────────────────────────

  group('save() failure', () {
    test('sets saveError and isSaving=false on failure', () async {
      final container = buildTestContainer();
      when(() => mockUseCase.call(any())).thenAnswer(
        (_) async =>
            const Left(ServerFailure(message: 'network error')),
      );

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      final state = container.read(personalDetailsViewModelProvider);
      expect(state.isSaving, false);
      expect(state.saveError, isNotNull);
      expect(state.saveError, contains('network error'));
      expect(state.justSaved, false);
    });

    test('does NOT fire analytics event on failure', () async {
      final container = buildTestContainer();
      when(() => mockUseCase.call(any())).thenAnswer(
        (_) async => const Left(ServerFailure(message: 'error')),
      );

      await container
          .read(personalDetailsViewModelProvider.notifier)
          .save(tFields);

      verifyNever(() => mockAnalytics.logEvent(
            name: 'profile_section_saved',
            parameters: any(named: 'parameters'),
          ));
    });
  });

  // ── startEditing / cancelEditing ───────────────────────────────────────────

  group('editing state', () {
    test('startEditing sets isEditing=true', () {
      final container = buildTestContainer();

      container
          .read(personalDetailsViewModelProvider.notifier)
          .startEditing();

      expect(
        container.read(personalDetailsViewModelProvider).isEditing,
        true,
      );
    });

    test('cancelEditing sets isEditing=false', () {
      final container = buildTestContainer();

      container
          .read(personalDetailsViewModelProvider.notifier)
          .startEditing();
      container
          .read(personalDetailsViewModelProvider.notifier)
          .cancelEditing();

      expect(
        container.read(personalDetailsViewModelProvider).isEditing,
        false,
      );
    });
  });
}
