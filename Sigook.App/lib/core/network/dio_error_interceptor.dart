import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import '../constants/error_messages.dart';
import '../error/exceptions.dart';

class DioErrorInterceptor extends Interceptor {
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    if (kDebugMode) {
      debugPrint(
        '╔══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ DIO ERROR');
      debugPrint(
        '╠══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ URL: ${err.requestOptions.uri}');
      debugPrint('║ Method: ${err.requestOptions.method}');
      debugPrint('║ Status Code: ${err.response?.statusCode}');
      debugPrint('║ Error Type: ${err.type}');
      debugPrint('║ Error Message: ${err.message}');
      if (err.response?.data != null) {
        debugPrint('║ Response Data: ${err.response?.data}');
      }
      debugPrint(
        '╚══════════════════════════════════════════════════════════════',
      );
    }

    super.onError(err, handler);
  }

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    if (kDebugMode) {
      debugPrint(
        '╔══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ API REQUEST');
      debugPrint(
        '╠══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ URL: ${options.uri}');
      debugPrint('║ Method: ${options.method}');
      if (options.data != null) {
        debugPrint('║ Data: ${options.data}');
      }
      debugPrint(
        '╚══════════════════════════════════════════════════════════════',
      );
    }
    super.onRequest(options, handler);
  }

  @override
  void onResponse(Response response, ResponseInterceptorHandler handler) {
    if (kDebugMode) {
      debugPrint(
        '╔══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ API RESPONSE');
      debugPrint(
        '╠══════════════════════════════════════════════════════════════',
      );
      debugPrint('║ URL: ${response.requestOptions.uri}');
      debugPrint('║ Status Code: ${response.statusCode}');
      debugPrint('║ Data: ${response.data}');
      debugPrint(
        '╚══════════════════════════════════════════════════════════════',
      );
    }
    super.onResponse(response, handler);
  }
}

/// Translates a [DioException] into a domain [NetworkException] or [ServerException].
/// Use this inside datasource catch blocks instead of repeating the mapping inline.
Never handleDioException(DioException e) {
  if (e.type == DioExceptionType.connectionTimeout ||
      e.type == DioExceptionType.sendTimeout ||
      e.type == DioExceptionType.receiveTimeout) {
    throw NetworkException('Connection timeout');
  } else if (e.type == DioExceptionType.connectionError) {
    throw NetworkException('No internet connection');
  } else if (e.response != null) {
    throw ServerException(
      message: 'Server error: ${e.response?.statusCode}',
      statusCode: e.response?.statusCode,
    );
  } else {
    throw NetworkException('Network error: ${e.message}');
  }
}

extension DioExceptionMessage on DioException {
  String get userMessage {
    switch (type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.transformTimeout:
        return ErrorMessages.connectionTimeout;
      case DioExceptionType.connectionError:
        return ErrorMessages.networkError;
      case DioExceptionType.badResponse:
        return ErrorMessages.serverError;
      case DioExceptionType.cancel:
        return ErrorMessages.userCancelled;
      case DioExceptionType.badCertificate:
        return ErrorMessages.serverError;
      case DioExceptionType.unknown:
        return ErrorMessages.unknownError;
    }
  }
}
