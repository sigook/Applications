import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/app_version_requirement_model.dart';

abstract class AppUpdateRemoteDataSource {
  Future<AppVersionRequirementModel> getVersionRequirement();
}

class AppUpdateRemoteDataSourceImpl implements AppUpdateRemoteDataSource {
  static const String endpoint = '/mobileapp/version';

  final ApiClient apiClient;

  AppUpdateRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<AppVersionRequirementModel> getVersionRequirement() async {
    try {
      final response = await apiClient.get(endpoint);
      if (response.statusCode != 200 || response.data is! Map<String, dynamic>) {
        throw ServerException(
          message: 'Failed to load app version requirement',
          statusCode: response.statusCode,
        );
      }
      return AppVersionRequirementModel.fromJson(
        response.data as Map<String, dynamic>,
      );
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout ||
          e.type == DioExceptionType.connectionError) {
        throw NetworkException(
          'Cannot connect to server. Please check your internet connection.',
        );
      }
      throw ServerException(
        message: e.message ?? 'Server error occurred',
        statusCode: e.response?.statusCode,
      );
    } on ServerException {
      rethrow;
    } catch (e) {
      throw ParseException('Failed to parse app version requirement: $e');
    }
  }
}
