import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/assignment_history_remote_datasource.dart';
import '../../data/repositories/assignment_history_repository_impl.dart';
import '../../domain/entities/assignment.dart';
import '../../domain/repositories/assignment_history_repository.dart';
import '../../domain/usecases/get_assignment_history.dart';
import '../../../../auth/presentation/viewmodels/auth_viewmodel.dart';

final assignmentHistoryDatasourceProvider =
    Provider<AssignmentHistoryRemoteDataSource>((ref) {
  return AssignmentHistoryRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final assignmentHistoryRepositoryProvider =
    Provider<AssignmentHistoryRepository>((ref) {
  return AssignmentHistoryRepositoryImpl(
    datasource: ref.read(assignmentHistoryDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getAssignmentHistoryUseCaseProvider =
    Provider<GetAssignmentHistory>((ref) {
  return GetAssignmentHistory(ref.read(assignmentHistoryRepositoryProvider));
});

final activeAssignmentsProvider = FutureProvider<List<Assignment>>((ref) async {
  ref.watch(sessionUserIdProvider);
  final result = await ref.read(getAssignmentHistoryUseCaseProvider)(
    status: 'Active',
    pageSize: 10,
  );
  return result.fold((f) => throw Exception(f.message), (page) => page.items);
});
