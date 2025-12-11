import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/paginated_jobs_model.dart';

abstract class JobsRemoteDataSource {
  Future<PaginatedJobsModel> getJobs({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  });
}

class JobsRemoteDataSourceImpl implements JobsRemoteDataSource {
  final ApiClient apiClient;

  JobsRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PaginatedJobsModel> getJobs({
    required int sortBy,
    required bool isDescending,
    required int pageIndex,
    required int pageSize,
  }) async {
    try {
      final response = await apiClient.dio.get(
        '/WorkerRequest',
        queryParameters: {
          'sortBy': sortBy,
          'isDescending': isDescending,
          'pageIndex': pageIndex,
          'pageSize': pageSize,
        },
      );

      if (response.statusCode == 200) {
        return PaginatedJobsModel.fromJson(
          response.data as Map<String, dynamic>,
        );
      } else {
        throw ServerException(
          message: 'Failed to load jobs: ${response.statusCode}',
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
      throw ServerException(message: 'Failed to load jobs: $e');
    }
  }
}
