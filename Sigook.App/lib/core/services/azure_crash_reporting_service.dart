import 'package:flutter/foundation.dart';

import 'azure_app_insights_client.dart';
import 'crash_reporting_service.dart';

/// Implements [CrashReportingService] by sending telemetry to Azure Application Insights.
class AzureCrashReportingService implements CrashReportingService {
  final AzureAppInsightsClient _client;
  final List<String> _logs = [];

  AzureCrashReportingService(this._client);

  @override
  Future<void> recordError(
    dynamic exception,
    StackTrace? stackTrace, {
    dynamic reason,
    bool fatal = false,
    Map<String, dynamic>? information,
  }) async {
    final props = <String, String>{};
    if (reason != null) props['reason'] = reason.toString();
    if (information != null) {
      information.forEach((k, v) => props[k] = v?.toString() ?? '');
    }
    if (_logs.isNotEmpty) props['breadcrumbs'] = _logs.join(' | ');

    final envelope = await _client.buildEnvelope(
      baseType: 'ExceptionData',
      name: 'Exception',
      baseData: {
        'exceptions': [
          {
            'typeName': exception.runtimeType.toString(),
            'message': exception.toString(),
            'hasFullStack': stackTrace != null,
            if (stackTrace != null) 'stack': stackTrace.toString(),
          }
        ],
        'severityLevel': fatal ? 4 : 3,
      },
      properties: props,
    );
    _client.track([envelope]);
  }

  @override
  Future<void> recordFlutterError(FlutterErrorDetails details) async {
    await recordError(
      details.exception,
      details.stack,
      reason: details.context?.toString(),
      fatal: true,
      information: {
        'library': details.library ?? 'unknown',
        'context': details.context?.toString() ?? 'none',
      },
    );
  }

  @override
  Future<void> setCustomKey(String key, dynamic value) async {
    _client.setCustomProperty(key, value?.toString() ?? '');
  }

  @override
  Future<void> setUserId(String userId) async {
    _client.setUserId(userId);
  }

  @override
  Future<void> log(String message) async {
    _logs.add(message);
    if (_logs.length > 50) _logs.removeAt(0);

    final envelope = await _client.buildEnvelope(
      baseType: 'MessageData',
      name: message,
      baseData: {'message': message, 'severityLevel': 0},
    );
    _client.track([envelope]);
  }
}
