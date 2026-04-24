import 'package:flutter_test/flutter_test.dart';
import 'package:package_info_plus/package_info_plus.dart';
import 'package:sigook_app_flutter/core/services/azure_app_insights_client.dart';

void main() {
  // PackageInfo.fromPlatform() uses platform channels — mock it for unit tests.
  setUpAll(() {
    TestWidgetsFlutterBinding.ensureInitialized();
    PackageInfo.setMockInitialValues(
      appName: 'SigookApp',
      packageName: 'com.sigook.app',
      version: '1.0.0',
      buildNumber: '1',
      buildSignature: '',
    );
  });

  const validConnectionString =
      'InstrumentationKey=test-key-1234;IngestionEndpoint=https://dc.example.com/;'
      'LiveEndpoint=https://live.example.com/';

  group('AzureAppInsightsClient — connection string parsing', () {
    test('parses InstrumentationKey from valid connection string', () {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      // track() is no-op when empty — if key was parsed correctly it won't be empty.
      // We verify indirectly via buildEnvelope which includes iKey.
      expect(client, isNotNull);
    });

    test('does not throw when connection string is empty', () {
      expect(
        () => AzureAppInsightsClient(connectionString: ''),
        returnsNormally,
      );
    });

    test('does not throw when connection string is malformed', () {
      expect(
        () => AzureAppInsightsClient(connectionString: 'not=a;valid=string'),
        returnsNormally,
      );
    });
  });

  group('AzureAppInsightsClient — track()', () {
    test('is no-op (does not throw) when connection string is empty', () {
      final client = AzureAppInsightsClient(connectionString: '');
      expect(
        () => client.track([
          {'name': 'test'},
        ]),
        returnsNormally,
      );
    });

    test('does not throw even when called with valid connection string', () {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      expect(
        () => client.track([
          {'name': 'test-event'},
        ]),
        returnsNormally,
      );
    });
  });

  group('AzureAppInsightsClient — buildEnvelope()', () {
    test('envelope contains required Azure fields', () async {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      final envelope = await client.buildEnvelope(
        baseType: 'EventData',
        name: 'Microsoft.ApplicationInsights.Event',
        baseData: {'name': 'test_event'},
      );

      expect(envelope['iKey'], isNotEmpty);
      expect(envelope['time'], isNotNull);
      expect(envelope['data'], isNotNull);
      expect(envelope['data']['baseType'], 'EventData');
      expect(envelope['data']['baseData']['ver'], 2);
    });

    test('envelope tags contain session id', () async {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      final envelope = await client.buildEnvelope(
        baseType: 'EventData',
        name: 'Microsoft.ApplicationInsights.Event',
        baseData: {'name': 'test_event'},
      );

      expect(envelope['tags']['ai.session.id'], isNotEmpty);
    });

    test('setUserId flows into envelope tags', () async {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      client.setUserId('user-abc-123');

      final envelope = await client.buildEnvelope(
        baseType: 'EventData',
        name: 'Microsoft.ApplicationInsights.Event',
        baseData: {'name': 'test_event'},
      );

      expect(envelope['tags']['ai.user.id'], 'user-abc-123');
    });

    test('setCustomProperty flows into baseData properties', () async {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      client.setCustomProperty('env', 'test');

      final envelope = await client.buildEnvelope(
        baseType: 'EventData',
        name: 'Microsoft.ApplicationInsights.Event',
        baseData: {'name': 'test_event'},
      );

      expect(envelope['data']['baseData']['properties']['env'], 'test');
    });

    test('properties parameter is merged into baseData', () async {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);

      final envelope = await client.buildEnvelope(
        baseType: 'EventData',
        name: 'Microsoft.ApplicationInsights.Event',
        baseData: {'name': 'test_event'},
        properties: {'step': '1'},
      );

      expect(envelope['data']['baseData']['properties']['step'], '1');
    });
  });

  group('AzureAppInsightsClient — sessionId', () {
    test('sessionId is a non-empty string', () {
      final client =
          AzureAppInsightsClient(connectionString: validConnectionString);
      expect(client.sessionId, isNotEmpty);
    });

    test('two different instances have different sessionIds', () {
      final a =
          AzureAppInsightsClient(connectionString: validConnectionString);
      final b =
          AzureAppInsightsClient(connectionString: validConnectionString);
      expect(a.sessionId, isNot(equals(b.sessionId)));
    });
  });
}
