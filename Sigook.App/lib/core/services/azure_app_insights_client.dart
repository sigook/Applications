import 'dart:async';
import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:uuid/uuid.dart';

/// Shared HTTP client for Azure Application Insights telemetry.
///
/// All services (analytics, crash reporting) share one instance so that
/// userId and customProperties are consistent across all event types.
class AzureAppInsightsClient {
  final String _instrumentationKey;
  final String _ingestionEndpoint;
  final String sessionId;

  String? _userId;
  final Map<String, String> _customProperties = {};

  PackageInfo? _packageInfo;
  late final Dio _dio;

  AzureAppInsightsClient({required String connectionString})
      : _instrumentationKey = _parseKey(connectionString, 'InstrumentationKey'),
        _ingestionEndpoint = _parseEndpoint(connectionString),
        sessionId = const Uuid().v4() {
    _dio = Dio(BaseOptions(
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
    ));
  }

  // ---------------------------------------------------------------------------
  // Public API
  // ---------------------------------------------------------------------------

  void setUserId(String id) => _userId = id;

  void setCustomProperty(String key, String value) =>
      _customProperties[key] = value;

  /// Fire-and-forget POST to the Azure ingestion endpoint.
  /// Never throws — monitoring must never crash the app.
  void track(List<Map<String, dynamic>> items) {
    if (_instrumentationKey.isEmpty || _ingestionEndpoint.isEmpty) return;
    unawaited(_post(items));
  }

  Future<Map<String, dynamic>> buildEnvelope({
    required String baseType,
    required String name,
    required Map<String, dynamic> baseData,
    Map<String, dynamic>? properties,
  }) async {
    final info = await _getPackageInfo();
    final mergedProps = {..._customProperties, ...?properties};

    return {
      'name':
          'Microsoft.ApplicationInsights.$_instrumentationKey.${baseType.replaceAll('Data', '')}',
      'time': DateTime.now().toUtc().toIso8601String(),
      'iKey': _instrumentationKey,
      'tags': {
        if (_userId != null && _userId!.isNotEmpty) 'ai.user.id': _userId,
        'ai.session.id': sessionId,
        'ai.application.ver': info.version,
        'ai.device.os': Platform.operatingSystem,
        'ai.cloud.roleInstance': 'sigook-mobile',
      },
      'data': {
        'baseType': baseType,
        'baseData': {
          'ver': 2,
          ...baseData,
          if (mergedProps.isNotEmpty) 'properties': mergedProps,
        },
      },
    };
  }

  // ---------------------------------------------------------------------------
  // Private helpers
  // ---------------------------------------------------------------------------

  Future<void> _post(List<Map<String, dynamic>> items) async {
    try {
      final url = '${_ingestionEndpoint.trimRight()}/v2/track';
      final response = await _dio.post<dynamic>(
        url,
        data: items,
        options: Options(headers: {'Content-Type': 'application/json'}),
      );
      assert(() {
        debugPrint(
          '[AppInsights] POST ${response.statusCode} — '
          '${items.map((e) => e['data']?['baseType']).join(', ')}',
        );
        return true;
      }());
    } catch (e) {
      assert(() {
        debugPrint('[AppInsights] POST failed: $e');
        return true;
      }());
    }
  }

  /// Sends a test payload synchronously and returns the HTTP status code.
  /// Use only from diagnostic/debug UI — never from production code paths.
  Future<int?> testConnection() async {
    try {
      final url = '${_ingestionEndpoint.trimRight()}/v2/track';
      final envelope = await buildEnvelope(
        baseType: 'EventData',
        name: 'diagnostic_ping',
        baseData: {'name': 'diagnostic_ping'},
        properties: {'source': 'debug_panel'},
      );
      final response = await _dio.post<dynamic>(
        url,
        data: [envelope],
        options: Options(headers: {'Content-Type': 'application/json'}),
      );
      return response.statusCode;
    } catch (_) {
      return null;
    }
  }

  Future<PackageInfo> _getPackageInfo() async {
    return _packageInfo ??= await PackageInfo.fromPlatform();
  }

  static String _parseKey(String connectionString, String keyName) {
    if (connectionString.isEmpty) return '';
    final regex = RegExp('$keyName=([^;]+)', caseSensitive: false);
    return regex.firstMatch(connectionString)?.group(1) ?? '';
  }

  static String _parseEndpoint(String connectionString) {
    if (connectionString.isEmpty) return '';
    final regex = RegExp(r'IngestionEndpoint=([^;]+)', caseSensitive: false);
    return regex.firstMatch(connectionString)?.group(1) ??
        'https://dc.services.visualstudio.com';
  }
}
