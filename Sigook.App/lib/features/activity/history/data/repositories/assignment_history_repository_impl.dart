import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/activity_repository_helpers.dart';
import '../../domain/entities/assignment.dart';
import '../../domain/repositories/assignment_history_repository.dart';
import '../datasources/assignment_history_remote_datasource.dart';

class AssignmentHistoryRepositoryImpl implements AssignmentHistoryRepository {
  final AssignmentHistoryRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  AssignmentHistoryRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, AssignmentPage>> getHistory({
    String? status,
    required int pageIndex,
    required int pageSize,
  }) =>
      guardedActivityCall(networkInfo, () async {
        final response = await datasource.getHistory(
          status: status,
          pageIndex: pageIndex,
          pageSize: pageSize,
        );
        return AssignmentPage(
          items: response.items.map((m) => m.toEntity()).toList(),
          pageIndex: response.pageIndex,
          hasMore: response.pageIndex < response.totalPages,
        );
      });
}
