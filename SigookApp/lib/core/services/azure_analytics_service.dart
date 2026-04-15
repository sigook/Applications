import 'analytics_service.dart';
import 'azure_app_insights_client.dart';

/// Implements [AnalyticsService] by sending telemetry to Azure Application Insights.
class AzureAnalyticsService implements AnalyticsService {
  final AzureAppInsightsClient _client;

  AzureAnalyticsService(this._client);

  @override
  Future<void> logEvent({
    required String name,
    Map<String, dynamic>? parameters,
  }) async {
    final props = parameters?.map(
      (k, v) => MapEntry(k, v?.toString() ?? ''),
    );
    final envelope = await _client.buildEnvelope(
      baseType: 'EventData',
      name: name,
      baseData: {'name': name},
      properties: props,
    );
    _client.track([envelope]);
  }

  @override
  Future<void> logScreenView({
    required String screenName,
    String? screenClass,
  }) async {
    final envelope = await _client.buildEnvelope(
      baseType: 'PageViewData',
      name: screenName,
      baseData: {'name': screenName, 'url': screenClass ?? screenName},
    );
    _client.track([envelope]);
  }

  @override
  Future<void> setUserId(String? userId) async {
    if (userId != null) _client.setUserId(userId);
  }

  @override
  Future<void> setUserProperty({
    required String name,
    required String value,
  }) async {
    _client.setCustomProperty(name, value);
  }

  @override
  Future<void> logLogin({String? method}) async {
    await logEvent(
      name: 'login',
      parameters: {'method': ?method},
    );
  }

  @override
  Future<void> logSignUp({String? method}) async {
    await logEvent(
      name: 'sign_up',
      parameters: {'method': ?method},
    );
  }

  @override
  Future<void> logSearch({String? searchTerm}) async {
    await logEvent(
      name: 'search',
      parameters: {'search_term': ?searchTerm},
    );
  }

  @override
  Future<void> logShare({
    required String contentType,
    required String itemId,
  }) async {
    await logEvent(
      name: 'share',
      parameters: {'content_type': contentType, 'item_id': itemId},
    );
  }

  @override
  Future<void> logPurchase({
    required String currency,
    required double value,
    Map<String, dynamic>? parameters,
  }) async {
    await logEvent(
      name: 'purchase',
      parameters: {
        'currency': currency,
        'value': value.toString(),
        ...?parameters,
      },
    );
  }
}
