import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../jobs/data/models/paginated_jobs_model.dart';

abstract class HistoryRemoteDataSource {
  Future<PaginatedJobsModel> getHistory({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  });
}

class HistoryRemoteDataSourceImpl implements HistoryRemoteDataSource {
  final ApiClient apiClient;

  HistoryRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PaginatedJobsModel> getHistory({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  }) async {
    try {
      final response = await apiClient.dio.get(
        '/WorkerRequestHistory',
        queryParameters: {
          'sortBy': sortBy,
          'isDescending': isDescending,
          'pageIndex': pageIndex,
          'pageSize': pageSize,
        },
      );

      if (response.statusCode == 200) {
        final data = Map<String, dynamic>.from(
          response.data as Map<String, dynamic>,
        );

        // WorkerRequestHistory returns workerApprovedToWork as a string ("True"/"False").
        // JobModel expects bool?, so we convert before parsing.
        final rawItems = (data['items'] as List<dynamic>? ?? []);
        final items = rawItems.map((item) {
          final map = Map<String, dynamic>.from(item as Map);
          if (map['workerApprovedToWork'] is String) {
            map['workerApprovedToWork'] =
                (map['workerApprovedToWork'] as String).toLowerCase() == 'true';
          }
          map['jobTitle'] ??= '';
          map['agencyFullName'] ??= '';
          return map;
        }).toList();

        return PaginatedJobsModel.fromJson({...data, 'items': items});
      } else {
        throw ServerException(
          message: 'Failed to load history: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      if (e is ServerException || e is NetworkException) rethrow;
      throw ServerException(message: 'Failed to load history: $e');
    }
  }
}
