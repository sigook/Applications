import 'dart:convert';
import 'package:dio/dio.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/network/api_client.dart';
import '../models/worker_registration_request.dart';

/// Remote data source for registration operations
abstract class RegistrationRemoteDataSource {
  /// Submit worker registration to API
  /// Endpoint: POST https://staging.api.sigook.ca/api/WorkerProfile
  Future<void> registerWorker(WorkerRegistrationRequest request);
}

class RegistrationRemoteDataSourceImpl implements RegistrationRemoteDataSource {
  final ApiClient apiClient;

  RegistrationRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<void> registerWorker(WorkerRegistrationRequest request) async {
    try {
      final jsonData = request.toJson();
      
      // Debug: Print the actual JSON being sent
      print('╔═══ WORKER REGISTRATION REQUEST ═══');
      print('║ JSON Data:');
      const encoder = JsonEncoder.withIndent('  ');
      final prettyJson = encoder.convert(jsonData);
      print(prettyJson);
      print('╚═══════════════════════════════════');
      
      final response = await apiClient.post(
        '/WorkerProfile',
        data: jsonData,
      );

      if (response.statusCode == 200 || response.statusCode == 201) {
        // Registration successful
        return;
      } else {
        throw ServerException(
          message: 'Failed to register worker',
          statusCode: response.statusCode,
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException(
          'Request timeout. Please check your internet connection.',
        );
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException(
          'Cannot connect to server. Please check your internet connection.',
        );
      } else if (e.error?.toString().contains('SocketException') == true) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      } else if (e.response != null) {
        // Extract error message from API response
        final errorMessage = e.response!.data is Map
            ? e.response!.data['message'] ?? 'Registration failed'
            : 'Registration failed';
        throw ServerException(
          message: errorMessage,
          statusCode: e.response!.statusCode,
        );
      } else {
        throw ServerException(
          message: e.message ?? 'Server error occurred',
          statusCode: null,
        );
      }
    } on NetworkException {
      rethrow;
    } on ServerException {
      rethrow;
    } catch (e) {
      if (e.toString().contains('SocketException')) {
        throw NetworkException(
          'Cannot reach server. Please check your network connection or VPN.',
        );
      }
      throw ServerException(
        message: 'An unexpected error occurred: ${e.toString()}',
        statusCode: null,
      );
    }
  }
}
