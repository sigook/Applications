import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/worker_comment_model.dart';

abstract class CommentsRemoteDataSource {
  Future<List<WorkerCommentModel>> getComments(
    String workerId, {
    int pageSize = 10,
    int pageIndex = 1,
  });
}

class CommentsRemoteDataSourceImpl implements CommentsRemoteDataSource {
  final ApiClient apiClient;

  CommentsRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<List<WorkerCommentModel>> getComments(
    String workerId, {
    int pageSize = 10,
    int pageIndex = 1,
  }) async {
    try {
      final response = await apiClient.dio.get(
        '/worker/$workerId/comment',
        queryParameters: {'PageSize': pageSize, 'PageIndex': pageIndex},
      );

      if (response.statusCode == 200) {
        final data = response.data as Map<String, dynamic>;
        final items = (data['items'] as List<dynamic>? ?? []);
        return items
            .map((e) => WorkerCommentModel.fromJson(e as Map<String, dynamic>))
            .toList();
      } else {
        throw ServerException(
          message: 'Failed to load comments: ${response.statusCode}',
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
      throw ServerException(message: 'Failed to load comments: $e');
    }
  }
}
