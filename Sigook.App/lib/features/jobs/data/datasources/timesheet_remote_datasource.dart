import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/network/api_client.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/punch_status_model.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/timesheet_response_model.dart';

abstract class TimesheetRemoteDatasource {
  Future<PunchStatusModel> getPunchStatus(
    String requestId,
    double latitude,
    double longitude,
  );
  Future<TimesheetResponseModel> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  });
}

class TimesheetRemoteDataSourceImpl implements TimesheetRemoteDatasource {
  final ApiClient apiClient;
  TimesheetRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<PunchStatusModel> getPunchStatus(
    String requestId,
    double latitude,
    double longitude,
  ) async {
    try {
      final response = await apiClient.dio.get(
        '/worker/requests/$requestId/TimeSheet/punch-status/$latitude/$longitude',
      );
      if (response.statusCode == 200) {
        return PunchStatusModel.fromJson(response.data as Map<String, dynamic>);
      } else {
        throw ServerException(
          message: 'Failed to load punch status: ${response.statusCode}',
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
      throw ServerException(message: 'Failed to load punch status: $e');
    }
  }

  @override
  Future<TimesheetResponseModel> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  }) async {
    try {
      final response = await apiClient.dio.post(
        '/worker/requests/$jobId/TimeSheet',
        data: {'latitude': latitude, 'longitude': longitude},
      );
      if (response.statusCode == 200) {
        return TimesheetResponseModel.fromJson(response.data);
      } else {
        throw ServerException(
          message: 'Failed to submit timesheet: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.receiveTimeout) {
        throw NetworkException('Connection timeout');
      } else if (e.type == DioExceptionType.connectionError) {
        throw NetworkException('No internet connection');
      } else if (e.response != null) {
        // Extract error message from response body
        final errorMessage = _extractErrorMessage(e.response?.data);
        throw ServerException(
          message: errorMessage ?? 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      throw ServerException(message: 'Failed to submit timesheet: $e');
    }
  }

  /// Extracts error message from API response body.
  /// Handles various response formats like:
  /// - {"": ["Error message"]}
  /// - {"error": "Error message"}
  /// - {"message": "Error message"}
  /// - {"errors": {"field": ["Error message"]}}
  String? _extractErrorMessage(dynamic responseData) {
    if (responseData == null) return null;

    try {
      if (responseData is Map<String, dynamic>) {
        // Check for empty key with array (ASP.NET validation format)
        if (responseData.containsKey('') && responseData[''] is List) {
          final errors = responseData[''] as List;
          if (errors.isNotEmpty) {
            return errors.first.toString();
          }
        }

        // Check for common error message keys
        for (final key in [
          'error',
          'message',
          'Message',
          'Error',
          'title',
          'Title',
        ]) {
          if (responseData.containsKey(key) && responseData[key] != null) {
            return responseData[key].toString();
          }
        }

        // Check for errors object (ASP.NET ModelState format)
        if (responseData.containsKey('errors') &&
            responseData['errors'] is Map) {
          final errors = responseData['errors'] as Map;
          for (final value in errors.values) {
            if (value is List && value.isNotEmpty) {
              return value.first.toString();
            }
          }
        }

        // If the response has any key with an array value, try to get the first error
        for (final value in responseData.values) {
          if (value is List && value.isNotEmpty) {
            return value.first.toString();
          }
        }
      } else if (responseData is String) {
        return responseData;
      }
    } catch (e) {
      debugPrint('Error extracting error message: $e');
    }

    return null;
  }
}
