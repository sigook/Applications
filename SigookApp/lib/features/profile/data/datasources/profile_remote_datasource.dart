import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/dio_error_interceptor.dart';
import '../models/worker_profile_model.dart';

abstract class ProfileRemoteDataSource {
  Future<WorkerProfileModel> getWorkerProfile();
}

class ProfileRemoteDataSourceImpl implements ProfileRemoteDataSource {
  final ApiClient apiClient;

  ProfileRemoteDataSourceImpl({required this.apiClient});

  Future<T> _execute<T>(Future<T> Function() call) async {
    try {
      return await call();
    } on DioException catch (e) {
      handleDioException(e);
    } on ServerException {
      rethrow;
    } on NetworkException {
      rethrow;
    } catch (e) {
      throw ServerException(message: 'Unexpected error: $e');
    }
  }

  @override
  Future<WorkerProfileModel> getWorkerProfile() =>
      _execute(() async {
        final response = await apiClient.dio.get('/WorkerProfile/me');
        return WorkerProfileModel.fromJson(
          response.data as Map<String, dynamic>,
        );
      });
}
