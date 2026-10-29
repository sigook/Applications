import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/activity_base_datasource.dart';
import '../models/assignment_model.dart';

class AssignmentHistoryResponse {
  final List<AssignmentModel> items;
  final int pageIndex;
  final int totalPages;

  const AssignmentHistoryResponse({
    required this.items,
    required this.pageIndex,
    required this.totalPages,
  });
}

abstract class AssignmentHistoryRemoteDataSource {
  Future<AssignmentHistoryResponse> getHistory({
    String? status,
    required int pageIndex,
    required int pageSize,
  });
}

class AssignmentHistoryRemoteDataSourceImpl extends ActivityBaseDatasource
    implements AssignmentHistoryRemoteDataSource {
  @override
  final ApiClient apiClient;

  AssignmentHistoryRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<AssignmentHistoryResponse> getHistory({
    String? status,
    required int pageIndex,
    required int pageSize,
  }) =>
      execute(() async {
        final response = await apiClient.dio.get(
          '/worker/history',
          queryParameters: {
            'pageIndex': pageIndex,
            'pageSize': pageSize,
            'status': ?status,
          },
        );
        final data = response.data as Map<String, dynamic>;
        return AssignmentHistoryResponse(
          items: (data['items'] as List<dynamic>? ?? [])
              .map((e) => AssignmentModel.fromJson(e as Map<String, dynamic>))
              .toList(),
          pageIndex: (data['pageIndex'] as num?)?.toInt() ?? pageIndex,
          totalPages: (data['totalPages'] as num?)?.toInt() ?? 1,
        );
      });
}
