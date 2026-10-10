import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../../../jobs/domain/entities/paginated_jobs.dart';
import '../repositories/history_repository.dart';

class GetHistoryParams {
  final int sortBy;
  final bool isDescending;
  final int pageIndex;
  final int pageSize;

  const GetHistoryParams({
    this.sortBy = 0,
    this.isDescending = false,
    this.pageIndex = 1,
    this.pageSize = 30,
  });
}

class GetHistory implements UseCase<PaginatedJobs, GetHistoryParams> {
  final HistoryRepository repository;

  GetHistory(this.repository);

  @override
  Future<Either<Failure, PaginatedJobs>> call(GetHistoryParams params) {
    return repository.getHistory(
      sortBy: params.sortBy,
      isDescending: params.isDescending,
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    );
  }
}
