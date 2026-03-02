import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../jobs/domain/entities/paginated_jobs.dart';

abstract class HistoryRepository {
  Future<Either<Failure, PaginatedJobs>> getHistory({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  });
}
