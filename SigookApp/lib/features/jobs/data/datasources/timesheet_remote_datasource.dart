import 'package:dio/dio.dart';
import 'package:sigook_app_flutter/core/error/exceptions.dart';
import 'package:sigook_app_flutter/core/network/api_client.dart';
import 'package:sigook_app_flutter/features/jobs/data/models/clock_type_model.dart';
import 'package:sigook_app_flutter/features/jobs/domain/entities/clock_type.dart';

abstract class TimesheetRemoteDatasource {
  Future<ClockType> getClockType(DateTime date, String requestId);
}

class TimeSheetRemoteDataSourceImpl implements TimesheetRemoteDatasource {
  final ApiClient apiClient;
  TimeSheetRemoteDataSourceImpl({required this.apiClient});

  @override
  Future<ClockType> getClockType(DateTime date, String requestId) async {
    try {
      final response = await apiClient.dio.get(
        'WorkerRequest/$requestId/TimeSheet/clock-type?date=$date',
      );
      if (response.statusCode == 200) {
        return clockTypeFromInt(response.data);
      } else {
        throw ServerException(
          message: 'Failed to load data: ${response.statusCode}',
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
      throw ServerException(message: 'Failed to load data: $e');
    }
  }
}
