import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/paginated_jobs.dart';
import '../repositories/jobs_repository.dart';

class GetJobs implements UseCase<PaginatedJobs, GetJobsParams> {
  final JobsRepository repository;

  GetJobs(this.repository);

  @override
  Future<Either<Failure, PaginatedJobs>> call(GetJobsParams params) {
    return repository.getJobs(
      sortBy: params.sortBy,
      isDescending: params.isDescending,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    );
  }
}

class GetJobsParams {
  final int sortBy;
  final bool isDescending;
  final int pageIndex;
  final int pageSize;

  const GetJobsParams({
    this.sortBy = 0,
    this.isDescending = false,
    this.pageIndex = 1,
    this.pageSize = 30,
  });
}
