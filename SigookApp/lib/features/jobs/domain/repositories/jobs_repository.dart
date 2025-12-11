import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/paginated_jobs.dart';

abstract class JobsRepository {
  Future<Either<Failure, PaginatedJobs>> getJobs({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  });
}
