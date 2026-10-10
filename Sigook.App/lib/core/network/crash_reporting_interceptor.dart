import 'package:dio/dio.dart';

import '../services/crash_reporting_service.dart';

/// Dio interceptor that forwards server errors (5xx) and connection errors
/// to [CrashReportingService]. 4xx errors are intentionally ignored because
/// 401/404 are expected business-logic responses, not crashes.
class CrashReportingInterceptor extends Interceptor {
  final CrashReportingService _crashService;

  CrashReportingInterceptor(this._crashService);

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final statusCode = err.response?.statusCode;
    final isServerError = statusCode != null && statusCode >= 500;
    final isConnectionError =
        err.type == DioExceptionType.connectionError ||
        err.type == DioExceptionType.connectionTimeout ||
        err.type == DioExceptionType.receiveTimeout;

    if (isServerError || isConnectionError) {
      _crashService.recordError(
        err,
        err.stackTrace,
        fatal: false,
        information: {
          'url': err.requestOptions.uri.toString(),
          'method': err.requestOptions.method,
          if (statusCode != null) 'statusCode': statusCode.toString(),
          'errorType': err.type.name,
        },
      );
    }

    super.onError(err, handler);
  }
}
