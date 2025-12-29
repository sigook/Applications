import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../services/analytics_service.dart';
import '../services/crash_reporting_service.dart';

part 'analytics_providers.g.dart';

@riverpod
AnalyticsService analyticsService(Ref ref) {
  return AnalyticsServiceImpl();
}

@riverpod
CrashReportingService crashReportingService(Ref ref) {
  return CrashReportingServiceImpl();
}
