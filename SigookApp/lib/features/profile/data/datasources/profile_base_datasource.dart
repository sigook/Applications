import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/dio_error_interceptor.dart';
import '../models/worker_profile_model.dart';

/// Base class for all profile section datasources.
/// Provides [execute], [getWorkerProfile], and [basenameOf] shared helpers.
/// Subclasses must declare [apiClient].
abstract class ProfileBaseDatasource {
  ApiClient get apiClient;

  Future<T> execute<T>(Future<T> Function() call) async {
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

  /// Fetches the authenticated worker's full profile via GET /WorkerProfile/me.
  /// Use [profile.id] wherever a workerId is needed for section endpoints.
  Future<WorkerProfileModel> getWorkerProfile() =>
      execute(() async {
        final response = await apiClient.dio.get('/WorkerProfile/me');
        return WorkerProfileModel.fromJson(
          response.data as Map<String, dynamic>,
        );
      });

  static String basenameOf(String path) =>
      path.split(RegExp(r'[/\\]')).last;
}
