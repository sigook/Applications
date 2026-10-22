import 'dart:async';
import 'package:flutter/foundation.dart';

abstract class CrashReportingService {
  Future<void> recordError(
    dynamic exception,
    StackTrace? stackTrace, {
    dynamic reason,
    bool fatal = false,
    Map<String, dynamic>? information,
  });

  Future<void> recordFlutterError(FlutterErrorDetails details);

  Future<void> setCustomKey(String key, dynamic value);

  Future<void> setUserId(String userId);

  Future<void> log(String message);
}

void setupCrashReporting(CrashReportingService crashReporting) {
  FlutterError.onError = (FlutterErrorDetails details) {
    crashReporting.recordFlutterError(details);
  };

  PlatformDispatcher.instance.onError = (error, stack) {
    crashReporting.recordError(error, stack, fatal: true);
    return true;
  };
}
