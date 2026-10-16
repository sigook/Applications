import 'package:dartz/dartz.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/job.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/paginated_jobs.dart';
import 'package:sigook_app_flutter/features/jobs/domain/usecases/get_jobs.dart';

import '../../../../helpers/mocks.dart';

final _tNow = DateTime(2024, 1, 1);

Job _makeJob(String id) => Job(
      id: id,
      jobTitle: 'Test Job',
      numberId: 1,
      workersQuantity: 5,
      isAsap: false,
      workerRate: 20.0,
      createdAt: _tNow,
      startAt: _tNow,
    );

void main() {
  late MockJobsRepository mockRepo;
  late GetJobs usecase;

  final tPaginatedJobs = PaginatedJobs(
    items: [_makeJob('job-1'), _makeJob('job-2')],
    pageIndex: 1,
    totalPages: 3,
    totalItems: 90,
  );

  setUp(() {
    mockRepo = MockJobsRepository();
    usecase = GetJobs(mockRepo);
  });

  test('returns PaginatedJobs on success', () async {
    when(() => mockRepo.getJobs(
          sortBy: any(named: 'sortBy'),
          isDescending: any(named: 'isDescending'),
          pageIndex: any(named: 'pageIndex'),
          pageSize: any(named: 'pageSize'),
        )).thenAnswer((_) async => Right(tPaginatedJobs));

    final result = await usecase(const GetJobsParams());

    expect(result, Right(tPaginatedJobs));
  });

  test('passes all params to repository', () async {
    when(() => mockRepo.getJobs(
          sortBy: 1,
          isDescending: true,
          pageIndex: 2,
          pageSize: 10,
        )).thenAnswer((_) async => Right(tPaginatedJobs));

    await usecase(const GetJobsParams(
      sortBy: 1,
      isDescending: true,
      pageIndex: 2,
      pageSize: 10,
    ));

    verify(() => mockRepo.getJobs(
          sortBy: 1,
          isDescending: true,
          pageIndex: 2,
          pageSize: 10,
        )).called(1);
  });

  test('returns NetworkFailure when repository fails', () async {
    const failure = NetworkFailure(message: 'offline');
    when(() => mockRepo.getJobs(
          sortBy: any(named: 'sortBy'),
          isDescending: any(named: 'isDescending'),
          pageIndex: any(named: 'pageIndex'),
          pageSize: any(named: 'pageSize'),
        )).thenAnswer((_) async => const Left(failure));

    final result = await usecase(const GetJobsParams());

    expect(result, const Left(failure));
  });
}
