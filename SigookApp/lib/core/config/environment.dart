import 'package:flutter/foundation.dart';

enum Environment { local, staging, production }

/// Environment configuration loaded via compile-time constants.
///
/// Values are injected at build time using:
/// - Local development: --dart-define-from-file=.env.staging (reads local .env file)
/// - CI/CD (Azure DevOps): --dart-define=KEY=VALUE flags (from pipeline variable groups)
/// - CI/CD (Xcode Cloud): --dart-define=KEY=VALUE flags (from Xcode Cloud env vars)
///
/// String.fromEnvironment is resolved at compile time - values are baked into the binary.
class EnvironmentConfig {
  static const String _environment =
      String.fromEnvironment('ENVIRONMENT');
  static const String _authAuthority =
      String.fromEnvironment('AUTH_AUTHORITY');
  static const String _apiBaseUrl =
      String.fromEnvironment('API_BASE_URL');
  static const String _clientId =
      String.fromEnvironment('CLIENT_ID');
  static const String _redirectUri =
      String.fromEnvironment('REDIRECT_URI');
  static const String _postLogoutRedirectUri =
      String.fromEnvironment('POST_LOGOUT_REDIRECT_URI');
  static const String _scopes =
      String.fromEnvironment('SCOPES');
  static const String _appName =
      String.fromEnvironment('APP_NAME');
  static const String _appInsightsConnectionString =
      String.fromEnvironment('APP_INSIGHTS_CONNECTION_STRING', defaultValue: '');

  // Every value is trimmed: CI variable editors silently keep a pasted
  // trailing space or newline, and an untrimmed client_id makes the token
  // endpoint answer invalid_client, which surfaces as a generic
  // 'invalid credentials' snackbar.
  static String get authority => _authAuthority.trim();

  static String get apiBaseUrl => _apiBaseUrl.trim();

  static String get clientId => _clientId.trim();

  static String get redirectUri => _redirectUri.trim();

  static String get postLogoutRedirectUri => _postLogoutRedirectUri.trim();

  static List<String> get scopes {
    if (_scopes.isEmpty) return [];
    return _scopes.split(',').map((s) => s.trim()).toList();
  }

  static String get appName => _appName;

  static String get appInsightsConnectionString => _appInsightsConnectionString;

  static Environment get current {
    final envString = _environment.toLowerCase();
    switch (envString) {
      case 'production':
        return Environment.production;
      case 'local':
        return Environment.local;
      default:
        return Environment.staging;
    }
  }

  static String get environmentName {
    switch (current) {
      case Environment.production:
        return 'Production';
      case Environment.local:
        return 'Local';
      case Environment.staging:
        return 'Staging';
    }
  }

  static bool get isProduction => current == Environment.production;

  static bool get isStaging => current == Environment.staging;

  static bool get isLocal => current == Environment.local;

  /// Validates that all required configuration values are present.
  /// Throws an exception if any required values are missing.
  /// Call this during app initialization to fail fast.
  static void validateRequiredConfig() {
    final missing = <String>[];

    if (authority.isEmpty) missing.add('AUTH_AUTHORITY');
    if (apiBaseUrl.isEmpty) missing.add('API_BASE_URL');
    if (clientId.isEmpty) missing.add('CLIENT_ID');
    if (redirectUri.isEmpty) missing.add('REDIRECT_URI');
    // Not validating SCOPES let the app boot fine and then fail at
    // sign-in with a generic 'invalid credentials' snackbar, because
    // an empty scope makes IdentityServer answer invalid_scope.
    if (scopes.isEmpty) missing.add('SCOPES');

    if (missing.isNotEmpty) {
      throw Exception(
        'Missing required environment configuration: ${missing.join(', ')}\n'
        'Ensure you pass environment variables via --dart-define-from-file or --dart-define.\n'
        'Example: flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart',
      );
    }
  }

  /// Debug helper to print current configuration
  static void printConfigSource() {
    debugPrint('');
    debugPrint('========================================================');
    debugPrint('           ENVIRONMENT CONFIGURATION                     ');
    debugPrint('========================================================');
    debugPrint(' Source: compile-time constants (--dart-define)           ');
    debugPrint('========================================================');
    _printConfigLine('ENVIRONMENT', _environment);
    _printConfigLine('AUTH_AUTHORITY', authority);
    _printConfigLine('API_BASE_URL', apiBaseUrl);
    _printConfigLine('CLIENT_ID', clientId);
    _printConfigLine('REDIRECT_URI', redirectUri);
    _printConfigLine('POST_LOGOUT_URI', postLogoutRedirectUri);
    _printConfigLine('SCOPES', scopes.join(','));
    _printConfigLine('APP_NAME', appName);
    debugPrint(' APP_INSIGHTS_CONNECTION_STRING: ${_appInsightsConnectionString.isNotEmpty ? '[set]' : '[not set]'}');
    debugPrint('========================================================');
    debugPrint('');
  }

  static void _printConfigLine(String key, String value) {
    final status = value.isNotEmpty ? '[OK]' : '[MISSING]';
    final displayValue = value.isNotEmpty
        ? (value.length > 35 ? '${value.substring(0, 35)}...' : value)
        : '(missing)';
    // Delimited so a stray leading/trailing space is visible in the log.
    debugPrint(' $status $key: <$displayValue>');
  }
}
