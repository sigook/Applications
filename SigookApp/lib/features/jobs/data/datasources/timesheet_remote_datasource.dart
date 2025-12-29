import 'package:dio/dio.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/network/api_client.dart';
import 'package:sigook_app_flutter/core/constants/enums.dart';
import 'package:sigook_app_flutter/core/utils/enum_converters.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/timesheet_response_model.dart';

abstract class TimesheetRemoteDatasource {
  Future<ClockType> getClockType(DateTime date, String requestId);
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
  Future<ClockType> getClockType(DateTime date, String requestId) async {
    try {
      final response = await apiClient.dio.get(
        '/WorkerRequest/$requestId/TimeSheet/clock-type?date=${date.toIso8601String()}',
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
        throw ServerException(
          message: 'Server error: ${e.response?.statusCode}',
        );
      } else {
        throw NetworkException('Network error: ${e.message}');
      }
    } catch (e) {
      throw ServerException(message: 'Failed to submit timesheet: $e');
    }
  }
}
