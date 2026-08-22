import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/network/api_client.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/utils/enum_converters.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/timesheet_response_model.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/paginated_timesheet_model.dart';
import 'dart:convert';

abstract class TimesheetRemoteDatasource {
  Future<ClockType> getClockType(
    DateTime date,
    String requestId,
    double latitude,
    double longitude,
  );
  Future<TimesheetResponseModel> submitTimesheet({
    required String jobId,
    required double latitude,
    required double longitude,
  });
  Future<PaginatedTimesheetModel> getTimesheetEntries({
    required String jobId,
    int pageIndex = 1,
    int pageSize = 5,
    bool isDescending = false,
  });
}

class TimesheetRemoteDataSourceImpl implements TimesheetRemoteDatasource {
  final ApiClient apiClient;
  TimesheetRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<ClockType> getClockType(
    DateTime date,
    String requestId,
    double latitude,
    double longitude,
  ) async {
    try {
      final response = await apiClient.dio.get(
        '/WorkerRequest/$requestId/TimeSheet/clock-type/$latitude/$longitude?date=${date.toIso8601String()}',
      );
      if (response.statusCode == 200) {
        return clockTypeFromInt(response.data);
      } else {
        throw ServerException(
          message: 'Failed to load clock type: ${response.statusCode}',
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
      throw ServerException(message: 'Failed to load clock type: $e');
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
        '/WorkerRequest/$jobId/TimeSheet',
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

  @override
  Future<PaginatedTimesheetModel> getTimesheetEntries({
    required String jobId,
    int pageIndex = 1,
    int pageSize = 5,
    bool isDescending = false,
  }) async {
    final endpoint = '/WorkerRequest/$jobId/TimeSheet';
    final queryParams = {
      'IsDescending': isDescending,
      'PageIndex': pageIndex,
      'PageSize': pageSize,
    };

    debugPrint('🟠 [DATASOURCE] ===== API REQUEST =====');
    debugPrint('🟠 [DATASOURCE] Endpoint: $endpoint');
    debugPrint('🟠 [DATASOURCE] Query Params: $queryParams');
    debugPrint(
      '🟠 [DATASOURCE] Full URL: ${apiClient.dio.options.baseUrl}$endpoint?IsDescending=$isDescending&PageIndex=$pageIndex&PageSize=$pageSize',
    );

    try {
      final response = await apiClient.dio.get(
        endpoint,
        queryParameters: queryParams,
      );

      debugPrint('🟠 [DATASOURCE] ===== API RESPONSE =====');
      debugPrint('🟠 [DATASOURCE] Status Code: ${response.statusCode}');
      debugPrint('🟠 [DATASOURCE] Response Headers: ${response.headers}');
      debugPrint(
        '🟠 [DATASOURCE] Response Data Type: ${response.data.runtimeType}',
      );
      debugPrint('🟠 [DATASOURCE] Response Data: ${jsonEncode(response.data)}');

      if (response.statusCode == 200) {
        debugPrint('🟠 [DATASOURCE] Parsing response...');

        try {
          final model = PaginatedTimesheetModel.fromJson(response.data);
          debugPrint('🟠 [DATASOURCE] ✅ Parsing successful!');
          debugPrint(
            '🟠 [DATASOURCE] Parsed items count: ${model.items.length}',
          );
          debugPrint('🟠 [DATASOURCE] Total items: ${model.totalItems}');
          debugPrint('🟠 [DATASOURCE] Total pages: ${model.totalPages}');
          return model;
        } catch (parseError, stackTrace) {
          debugPrint('❌ [DATASOURCE] Parsing failed!');
          debugPrint('❌ [DATASOURCE] Parse Error: $parseError');
          debugPrint('❌ [DATASOURCE] Stack trace: $stackTrace');
          rethrow;
        }
      } else {
        debugPrint(
          '❌ [DATASOURCE] Non-200 status code: ${response.statusCode}',
        );
        throw ServerException(
          message: 'Failed to load timesheet: ${response.statusCode}',
        );
      }
    } on DioException catch (e) {
      debugPrint('❌ [DATASOURCE] DioException caught!');
      debugPrint('❌ [DATASOURCE] Error Type: ${e.type}');
      debugPrint('❌ [DATASOURCE] Error Message: ${e.message}');
      debugPrint('❌ [DATASOURCE] Response: ${e.response?.data}');
      debugPrint('❌ [DATASOURCE] Status Code: ${e.response?.statusCode}');

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
    } catch (e, stackTrace) {
      debugPrint('❌ [DATASOURCE] Unexpected exception!');
      debugPrint('❌ [DATASOURCE] Error: $e');
      debugPrint('❌ [DATASOURCE] Stack trace: $stackTrace');
      throw ServerException(message: 'Failed to load timesheet: $e');
    }
  }
}
