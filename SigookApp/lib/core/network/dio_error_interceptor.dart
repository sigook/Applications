import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import '../constants/error_messages.dart';

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

extension DioExceptionMessage on DioException {
  String get userMessage {
    switch (type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
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
