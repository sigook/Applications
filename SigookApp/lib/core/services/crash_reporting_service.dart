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
    // NOTE: Firebase Crashlytics integration pending
    // To enable, uncomment and configure Firebase:
    // await FirebaseCrashlytics.instance.recordError(
    //   exception,
    //   stackTrace,
    //   reason: reason,
    //   fatal: fatal,
    //   information: information,
    // );

    // For now, log to console in debug mode
    if (kDebugMode) {
      debugPrint('=== Crash Report ${fatal ? '(FATAL)' : ''} ===');
      debugPrint('Exception: $exception');
      if (reason != null) debugPrint('Reason: $reason');
      if (stackTrace != null) debugPrint('Stack Trace:\n$stackTrace');
      if (information != null) debugPrint('Additional Info: $information');
      if (_customKeys.isNotEmpty) debugPrint('Custom Keys: $_customKeys');
      if (_logs.isNotEmpty) debugPrint('Logs: ${_logs.join('\n')}');
      debugPrint('==========================================');
    }
  }

  @override
  Future<void> recordFlutterError(FlutterErrorDetails details) async {
    // NOTE: Firebase Crashlytics integration pending
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

    // NOTE: Firebase Crashlytics integration pending
    // await FirebaseCrashlytics.instance.setCustomKey(key, value);

    if (kDebugMode) {
      debugPrint('Crashlytics Custom Key: $key = $value');
    }
  }

  @override
  Future<void> setUserId(String userId) async {
    await setCustomKey('user_id', userId);

    // NOTE: Firebase Crashlytics integration pending
    // await FirebaseCrashlytics.instance.setUserIdentifier(userId);
  }

  @override
  Future<void> log(String message) async {
    _logs.add(message);

    // Keep only last 50 logs to prevent memory issues
    if (_logs.length > 50) {
      _logs.removeAt(0);
    }

    // NOTE: Firebase Crashlytics integration pending
    // await FirebaseCrashlytics.instance.log(message);

    if (kDebugMode) {
      debugPrint('Crashlytics Log: $message');
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
