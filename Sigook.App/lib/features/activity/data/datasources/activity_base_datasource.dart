import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/network/dio_error_interceptor.dart';

abstract class ActivityBaseDatasource {
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
}
