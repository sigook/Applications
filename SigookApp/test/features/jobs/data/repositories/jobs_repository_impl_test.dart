import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/job_model.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/paginated_jobs_model.dart';
import 'package:sigook_app_flutter/features/jobs/data/repositories/jobs_repository_impl.dart';

import '../../../../helpers/mocks.dart';

final _tNow = DateTime(2024, 1, 1);

JobModel _makeJobModel(String id) => JobModel(
      id: id,
      jobTitle: 'Test Job',
      numberId: 1,
      workersQuantity: 5,
      isAsap: false,
      workerRate: 20.0,
      createdAt: _tNow,
      startAt: _tNow,
    );

final _tJobModels = [_makeJobModel('job-1'), _makeJobModel('job-2')];

final _tPaginatedModel = PaginatedJobsModel(
  items: _tJobModels,
  pageIndex: 1,
  totalPages: 3,
  totalItems: 90,
);

const _defaultParams = (
  sortBy: 0,
  isDescending: false,
  pageIndex: 1,
  pageSize: 30,
);

void main() {
  late MockJobsRemoteDataSource mockRemote;
  late MockJobsLocalDataSource mockLocal;
  late MockNetworkInfo mockNetwork;
  late JobsRepositoryImpl repository;

  setUp(() {
    mockRemote = MockJobsRemoteDataSource();
    mockLocal = MockJobsLocalDataSource();
    mockNetwork = MockNetworkInfo();
    repository = JobsRepositoryImpl(
      remoteDataSource: mockRemote,
      localDataSource: mockLocal,
      networkInfo: mockNetwork,
    );
  });

  // Helpers
  void whenOnline() =>
      when(() => mockNetwork.isConnected).thenAnswer((_) async => true);
  void whenOffline() =>
      when(() => mockNetwork.isConnected).thenAnswer((_) async => false);
  void whenRemoteReturns(PaginatedJobsModel model) =>
      when(() => mockRemote.getJobs(
            sortBy: any(named: 'sortBy'),
            isDescending: any(named: 'isDescending'),
            pageIndex: any(named: 'pageIndex'),
            pageSize: any(named: 'pageSize'),
          )).thenAnswer((_) async => model);
  void whenRemoteThrows(Exception e) =>
      when(() => mockRemote.getJobs(
            sortBy: any(named: 'sortBy'),
            isDescending: any(named: 'isDescending'),
            pageIndex: any(named: 'pageIndex'),
            pageSize: any(named: 'pageSize'),
          )).thenThrow(e);
  void whenCacheReturns(List<JobModel> jobs) =>
      when(() => mockLocal.getCachedJobs()).thenAnswer((_) async => jobs);
  void whenCacheEmpty() =>
      when(() => mockLocal.getCachedJobs()).thenAnswer((_) async => []);
  void stubCacheJobs() =>
      when(() => mockLocal.cacheJobs(any())).thenAnswer((_) async {});

  // ── Online success ──────────────────────────────────────────────────────────

  group('online — success', () {
    test('returns PaginatedJobs from remote', () async {
      whenOnline();
      whenRemoteReturns(_tPaginatedModel);
      stubCacheJobs();

      final result = await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: _defaultParams.pageIndex,
        pageSize: _defaultParams.pageSize,
      );

      expect(result.isRight(), true);
      result.fold((_) => fail('Expected Right'), (pj) {
        expect(pj.items.length, 2);
        expect(pj.totalPages, 3);
      });
    });

    test('caches jobs when pageIndex == 0 and items are non-empty', () async {
      whenOnline();
      final page0Model = PaginatedJobsModel(
        items: _tJobModels,
        pageIndex: 0,
        totalPages: 1,
        totalItems: 2,
      );
      when(() => mockRemote.getJobs(
            sortBy: any(named: 'sortBy'),
            isDescending: any(named: 'isDescending'),
            pageIndex: 0,
            pageSize: any(named: 'pageSize'),
          )).thenAnswer((_) async => page0Model);
      stubCacheJobs();

      await repository.getJobs(
        sortBy: 0,
        isDescending: false,
        pageIndex: 0,
        pageSize: 30,
      );

      verify(() => mockLocal.cacheJobs(_tJobModels)).called(1);
    });

    test('does NOT cache when pageIndex > 0', () async {
      whenOnline();
      whenRemoteReturns(_tPaginatedModel); // pageIndex == 1 in model
      stubCacheJobs();

      await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: 1,
        pageSize: _defaultParams.pageSize,
      );

      // pageIndex in the model is 1 (not 0), so no caching
      verifyNever(() => mockLocal.cacheJobs(any()));
    });
  });

  // ── Offline ─────────────────────────────────────────────────────────────────

  group('offline', () {
    test('returns cached jobs when cache is non-empty', () async {
      whenOffline();
      whenCacheReturns(_tJobModels);

      final result = await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: _defaultParams.pageIndex,
        pageSize: _defaultParams.pageSize,
      );

      expect(result.isRight(), true);
      result.fold((_) => fail('Expected Right'), (pj) {
        expect(pj.items.length, 2);
      });
      verifyNever(() => mockRemote.getJobs(
            sortBy: any(named: 'sortBy'),
            isDescending: any(named: 'isDescending'),
            pageIndex: any(named: 'pageIndex'),
            pageSize: any(named: 'pageSize'),
          ));
    });

    test('returns NetworkFailure when offline and cache is empty', () async {
      whenOffline();
      whenCacheEmpty();

      final result = await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: _defaultParams.pageIndex,
        pageSize: _defaultParams.pageSize,
      );

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });
  });

  // ── Server error with graceful degradation ──────────────────────────────────

  group('server error fallback', () {
    test('returns cached jobs when server throws 500 and cache is non-empty',
        () async {
      whenOnline();
      whenRemoteThrows(ServerException(message: 'server error', statusCode: 500));
      whenCacheReturns(_tJobModels);

      final result = await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: _defaultParams.pageIndex,
        pageSize: _defaultParams.pageSize,
      );

      expect(result.isRight(), true);
      result.fold((_) => fail('Expected Right'), (pj) {
        expect(pj.items.length, 2);
      });
    });

    test('returns ServerFailure when server throws 500 and cache is empty',
        () async {
      whenOnline();
      whenRemoteThrows(ServerException(message: 'server error', statusCode: 500));
      whenCacheEmpty();

      final result = await repository.getJobs(
        sortBy: _defaultParams.sortBy,
        isDescending: _defaultParams.isDescending,
        pageIndex: _defaultParams.pageIndex,
        pageSize: _defaultParams.pageSize,
      );

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<ServerFailure>()), (_) => fail(''));
    });
  });

  // ── getJobDetails ───────────────────────────────────────────────────────────

  group('getJobDetails', () {
    test('returns NetworkFailure when offline', () async {
      whenOffline();

      final result = await repository.getJobDetails('job-1');

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });

    test('returns ServerFailure on ServerException', () async {
      whenOnline();
      when(() => mockRemote.getJobDetails('job-1'))
          .thenThrow(ServerException(message: 'not found', statusCode: 404));

      final result = await repository.getJobDetails('job-1');

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<ServerFailure>()), (_) => fail(''));
    });
  });

  // ── applyToJob ──────────────────────────────────────────────────────────────

  group('applyToJob', () {
    test('returns NetworkFailure when offline', () async {
      whenOffline();

      final result = await repository.applyToJob('job-1');

      expect(result.isLeft(), true);
      result.fold((f) => expect(f, isA<NetworkFailure>()), (_) => fail(''));
    });

    test('returns Right(null) on success', () async {
      whenOnline();
      when(() => mockRemote.applyToJob('job-1')).thenAnswer((_) async {});

      final result = await repository.applyToJob('job-1');

      expect(result.isRight(), true);
    });
  });
}
