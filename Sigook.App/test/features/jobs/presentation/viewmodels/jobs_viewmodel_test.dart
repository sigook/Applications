import 'package:dartz/dartz.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/job.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/paginated_jobs.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_jobs.dart';
import 'package:sigook_app_flutter/features/jobs/presentation/providers/jobs_providers.dart';
import 'package:sigook_app_flutter/features/jobs/presentation/viewmodels/jobs_viewmodel.dart';

import '../../../../helpers/riverpod_test_helpers.dart';

class MockGetJobs extends Mock implements GetJobs {}

final _tNow = DateTime(2024, 1, 1);

Job _makeJob(String id) => Job(
      id: id,
      jobTitle: 'Job $id',
      numberId: 1,
      workersQuantity: 3,
      isAsap: false,
      workerRate: 18.0,
      createdAt: _tNow,
      startAt: _tNow,
    );

PaginatedJobs _makePage({
  required List<Job> items,
  required int pageIndex,
  required int totalPages,
}) =>
    PaginatedJobs(
      items: items,
      pageIndex: pageIndex,
      totalPages: totalPages,
      totalItems: items.length,
    );

ProviderContainer _makeContainer(MockGetJobs mock) => buildContainer(
      ProviderContainer(
        overrides: [getJobsUseCaseProvider.overrideWithValue(mock)],
      ),
    );

void main() {
  setUpAll(() {
    registerFallbackValue(const GetJobsParams());
  });

  late MockGetJobs mockGetJobs;

  setUp(() {
    mockGetJobs = MockGetJobs();
  });

  void stubPage(PaginatedJobs page) {
    when(() => mockGetJobs.call(any()))
        .thenAnswer((_) async => Right(page));
  }

  void stubFailure(Failure failure) {
    when(() => mockGetJobs.call(any()))
        .thenAnswer((_) async => Left(failure));
  }

  // ── loadJobs ───────────────────────────────────────────────────────────────

  group('loadJobs', () {
    test('populates jobs and sets isLoading=false on success', () async {
      final container = _makeContainer(mockGetJobs);
      stubPage(_makePage(
        items: [_makeJob('a'), _makeJob('b')],
        pageIndex: 1,
        totalPages: 3,
      ));

      await container.read(jobsViewModelProvider.notifier).loadJobs();

      final state = container.read(jobsViewModelProvider);
      expect(state.isLoading, false);
      expect(state.jobs.length, 2);
      expect(state.hasMore, true); // pageIndex(1) < totalPages(3)
      expect(state.error, isNull);
    });

    test('sets hasMore=false when on last page', () async {
      final container = _makeContainer(mockGetJobs);
      stubPage(_makePage(items: [_makeJob('a')], pageIndex: 2, totalPages: 2));

      await container.read(jobsViewModelProvider.notifier).loadJobs();

      expect(container.read(jobsViewModelProvider).hasMore, false);
    });

    test('sets error on failure and clears jobs', () async {
      final container = _makeContainer(mockGetJobs);
      stubFailure(const ServerFailure(message: 'server error'));

      await container.read(jobsViewModelProvider.notifier).loadJobs();

      final state = container.read(jobsViewModelProvider);
      expect(state.isLoading, false);
      expect(state.jobs, isEmpty);
      expect(state.error, isNotNull);
    });
  });

  // ── loadMore ───────────────────────────────────────────────────────────────

  group('loadMore — guards', () {
    test('is no-op when hasMore=false', () async {
      final container = _makeContainer(mockGetJobs);

      stubPage(_makePage(items: [_makeJob('a')], pageIndex: 2, totalPages: 2));
      await container.read(jobsViewModelProvider.notifier).loadJobs();
      expect(container.read(jobsViewModelProvider).hasMore, false);

      clearInteractions(mockGetJobs);

      await container.read(jobsViewModelProvider.notifier).loadMore();

      verifyNever(() => mockGetJobs.call(any()));
    });

    test('appends jobs and increments page on success', () async {
      final container = _makeContainer(mockGetJobs);

      // Page 1
      when(() => mockGetJobs.call(any())).thenAnswer((_) async => Right(
            _makePage(
              items: [_makeJob('a'), _makeJob('b')],
              pageIndex: 1,
              totalPages: 3,
            ),
          ));
      await container.read(jobsViewModelProvider.notifier).loadJobs();
      expect(container.read(jobsViewModelProvider).jobs.length, 2);

      // Page 2
      when(() => mockGetJobs.call(any())).thenAnswer((_) async => Right(
            _makePage(items: [_makeJob('c')], pageIndex: 2, totalPages: 3),
          ));
      await container.read(jobsViewModelProvider.notifier).loadMore();

      final state = container.read(jobsViewModelProvider);
      expect(state.jobs.length, 3); // 2 + 1
      expect(state.currentPage, 2);
      expect(state.isLoadingMore, false);
    });
  });

  // ── refresh ────────────────────────────────────────────────────────────────

  group('refresh', () {
    test('resets state and reloads from page 1', () async {
      final container = _makeContainer(mockGetJobs);

      stubPage(_makePage(
        items: [_makeJob('a'), _makeJob('b')],
        pageIndex: 1,
        totalPages: 2,
      ));
      await container.read(jobsViewModelProvider.notifier).loadJobs();
      expect(container.read(jobsViewModelProvider).jobs.length, 2);

      stubPage(_makePage(items: [_makeJob('x')], pageIndex: 1, totalPages: 1));
      await container.read(jobsViewModelProvider.notifier).refresh();

      final state = container.read(jobsViewModelProvider);
      expect(state.jobs.length, 1);
      expect(state.jobs.first.id, 'x');
    });
  });
}
