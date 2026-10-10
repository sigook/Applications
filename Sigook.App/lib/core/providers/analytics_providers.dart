import 'package:riverpod/riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../config/environment.dart';
import '../services/analytics_service.dart';
import '../services/azure_analytics_service.dart';
import '../services/azure_app_insights_client.dart';
import '../services/azure_crash_reporting_service.dart';
import '../services/crash_reporting_service.dart';

part 'analytics_providers.g.dart';

/// Shared Azure App Insights client — plain Provider so it is not auto-disposed
/// and is reused by both analytics and crash reporting services.
final azureAppInsightsClientProvider = Provider<AzureAppInsightsClient>((ref) {
  return AzureAppInsightsClient(
    connectionString: EnvironmentConfig.appInsightsConnectionString,
  );
});

@riverpod
AnalyticsService analyticsService(Ref ref) {
  return AzureAnalyticsService(ref.read(azureAppInsightsClientProvider));
}

@riverpod
CrashReportingService crashReportingService(Ref ref) {
  return AzureCrashReportingService(ref.read(azureAppInsightsClientProvider));
}
