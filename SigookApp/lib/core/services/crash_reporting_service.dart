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

class CrashReportingServiceImpl implements CrashReportingService {
  final List<String> _logs = [];
  final Map<String, dynamic> _customKeys = {};

  @override
  Future<void> recordError(
    dynamic exception,
    StackTrace? stackTrace, {
    dynamic reason,
    bool fatal = false,
    Map<String, dynamic>? information,
  }) async {
    // TODO: Implement Firebase Crashlytics
    // await FirebaseCrashlytics.instance.recordError(
    //   exception,
    //   stackTrace,
    //   reason: reason,
    //   fatal: fatal,
    //   information: information,
    // );

    // For now, log to console in debug mode
    if (kDebugMode) {
      print('=== Crash Report ${fatal ? '(FATAL)' : ''} ===');
      print('Exception: $exception');
      if (reason != null) print('Reason: $reason');
      if (stackTrace != null) print('Stack Trace:\n$stackTrace');
      if (information != null) print('Additional Info: $information');
      if (_customKeys.isNotEmpty) print('Custom Keys: $_customKeys');
      if (_logs.isNotEmpty) print('Logs: ${_logs.join('\n')}');
      print('==========================================');
    }
  }

  @override
  Future<void> recordFlutterError(FlutterErrorDetails details) async {
    // TODO: Implement Firebase Crashlytics
    // await FirebaseCrashlytics.instance.recordFlutterError(details);

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
    _customKeys[key] = value;

    // TODO: Implement Firebase Crashlytics
    // await FirebaseCrashlytics.instance.setCustomKey(key, value);

    if (kDebugMode) {
      print('Crashlytics Custom Key: $key = $value');
    }
  }

  @override
  Future<void> setUserId(String userId) async {
    await setCustomKey('user_id', userId);

    // TODO: Implement Firebase Crashlytics
    // await FirebaseCrashlytics.instance.setUserIdentifier(userId);
  }

  @override
  Future<void> log(String message) async {
    _logs.add(message);

    // Keep only last 50 logs to prevent memory issues
    if (_logs.length > 50) {
      _logs.removeAt(0);
    }

    // TODO: Implement Firebase Crashlytics
    // await FirebaseCrashlytics.instance.log(message);

    if (kDebugMode) {
      print('Crashlytics Log: $message');
    }
  }
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
