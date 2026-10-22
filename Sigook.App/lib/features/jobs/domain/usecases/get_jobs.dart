import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/jobs_filter.dart';
import '../entities/paginated_jobs.dart';
import '../repositories/jobs_repository.dart';

class GetJobs implements UseCase<PaginatedJobs, GetJobsParams> {
  final JobsRepository repository;

  GetJobs(this.repository);

  @override
  Future<Either<Failure, PaginatedJobs>> call(GetJobsParams params) {
    return repository.getJobs(
      filter: params.filter,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    );
  }
}

class GetJobsParams {
  final JobsFilter filter;
  final int pageIndex;
  final int pageSize;

  const GetJobsParams({
    this.filter = JobsFilter.defaults,
    this.pageIndex = 1,
    this.pageSize = 30,
  });
}
