import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

enum Environment { local, staging, production }

/// Environment configuration loaded from .env files via flutter_dotenv.
///
/// .env files are loaded by EnvLoader in each entry point
/// (main_staging.dart, main_production.dart).
/// In CI/CD, the pipeline generates the .env file from Azure DevOps
/// variable groups before building.
class EnvironmentConfig {
  /// Get environment variable from dotenv with fallback.
  static String _getEnv(String key, {String fallback = ''}) {
    try {
      final value = dotenv.maybeGet(key);
      if (value != null && value.isNotEmpty) {
        return value;
      }
    } catch (e) {
      // dotenv not initialized
    }
    return fallback;
  }

  static String get authority => _getEnv('AUTH_AUTHORITY');

  static String get apiBaseUrl => _getEnv('API_BASE_URL');

  static String get clientId => _getEnv('CLIENT_ID');

  static String get redirectUri => _getEnv('REDIRECT_URI');

  static String get postLogoutRedirectUri =>
      _getEnv('POST_LOGOUT_REDIRECT_URI');

  static List<String> get scopes {
    final scopesString = _getEnv('SCOPES');
    if (scopesString.isEmpty) return [];
    return scopesString.split(',').map((s) => s.trim()).toList();
  }

  static String get appName => _getEnv('APP_NAME');

  static Environment get current {
    final envString = _getEnv('ENVIRONMENT').toLowerCase();
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

    if (missing.isNotEmpty) {
      throw Exception(
        'Missing required environment configuration: ${missing.join(', ')}\n'
        'Ensure you have a valid .env file for the current environment.',
      );
    }
  }

  /// Debug helper to print current configuration
  /// Call this after app initialization to verify config is loaded correctly
  static void printConfigSource() {
    debugPrint('');
    debugPrint('╔══════════════════════════════════════════════════════════════╗');
    debugPrint('║                 ENVIRONMENT CONFIGURATION                     ║');
    debugPrint('╠══════════════════════════════════════════════════════════════╣');
    debugPrint('║ Source: .env file via flutter_dotenv                         ║');
    debugPrint('╠══════════════════════════════════════════════════════════════╣');
    _printConfigLine('ENVIRONMENT', _getEnv('ENVIRONMENT'));
    _printConfigLine('AUTH_AUTHORITY', authority);
    _printConfigLine('API_BASE_URL', apiBaseUrl);
    _printConfigLine('CLIENT_ID', clientId);
    _printConfigLine('REDIRECT_URI', redirectUri);
    _printConfigLine('POST_LOGOUT_URI', postLogoutRedirectUri);
    _printConfigLine('SCOPES', scopes.join(','));
    _printConfigLine('APP_NAME', appName);
    debugPrint('╚══════════════════════════════════════════════════════════════╝');
    debugPrint('');
  }

  static void _printConfigLine(String key, String value) {
    final status = value.isNotEmpty ? '✓' : '✗';
    final displayValue = value.isNotEmpty
        ? (value.length > 35 ? '${value.substring(0, 35)}...' : value)
        : '(missing)';
    debugPrint('║ $status $key: $displayValue');
  }
}
