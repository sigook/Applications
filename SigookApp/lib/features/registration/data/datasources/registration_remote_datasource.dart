import 'dart:convert';
import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:retry/retry.dart';
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
    final jsonData = request.toJson();

    // Debug: Log the actual JSON being sent
    debugPrint('╔═══ WORKER REGISTRATION REQUEST ═══');
    debugPrint('║ JSON Data:');
    const encoder = JsonEncoder.withIndent('  ');
    final prettyJson = encoder.convert(jsonData);
    debugPrint(prettyJson);
    debugPrint('╚═══════════════════════════════════');

    // Retry configuration with exponential backoff
    const retryOptions = RetryOptions(
      maxAttempts: 3,
      delayFactor: Duration(seconds: 2), // 2s, 4s, 8s
      randomizationFactor: 0.25, // Add jitter to prevent thundering herd
      maxDelay: Duration(seconds: 10),
    );

    try {
      final response = await retryOptions.retry(
        () async {
          return await apiClient.post('/WorkerProfile', data: jsonData);
        },
        // Only retry on network/timeout errors, not on server errors (4xx/5xx)
        retryIf: (e) {
          if (e is DioException) {
            return e.type == DioExceptionType.connectionTimeout ||
                e.type == DioExceptionType.receiveTimeout ||
                e.type == DioExceptionType.sendTimeout ||
                e.type == DioExceptionType.connectionError;
          }
          // Retry on socket exceptions
          return e.toString().contains('SocketException');
        },
        onRetry: (e) {
          debugPrint(
            '⚠️ Retrying registration request due to: ${e.toString()}',
          );
        },
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
