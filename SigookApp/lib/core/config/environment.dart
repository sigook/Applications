import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

enum Environment { local, staging, production }

/// Environment configuration that supports both:
/// - Compile-time constants via --dart-define (Azure DevOps CI/CD)
/// - .env files via flutter_dotenv (local development)
///
/// Priority: dart-define > dotenv > fallback (empty string)
///
/// HOW IT WORKS:
/// - Local development: .env.staging or .env.production files are loaded
///   via flutter_dotenv. These files must be listed in pubspec.yaml assets.
/// - CI/CD builds: Values are injected at compile time via --dart-define flags.
///   The .env files are not needed (and not present) in CI/CD.
class EnvironmentConfig {
  /// Get environment variable with fallback chain:
  /// 1. Compile-time constants (--dart-define, for CI/CD)
  /// 2. dotenv loaded values (for local development)
  /// 3. Fallback value (default: empty string)
  static String _getEnv(String key, {String fallback = ''}) {
    // First try compile-time constants (CI/CD)
    final dartDefineValue = _getSystemEnv(key);
    if (dartDefineValue != null && dartDefineValue.isNotEmpty) {
      return dartDefineValue;
    }

    // Fall back to dotenv (local development)
    try {
      final dotenvValue = dotenv.maybeGet(key);
      if (dotenvValue != null && dotenvValue.isNotEmpty) {
        return dotenvValue;
      }
    } catch (e) {
      // dotenv not initialized, continue to fallback
    }

    return fallback;
  }

  /// Get system environment variable (works on mobile via compile-time injection)
  static String? _getSystemEnv(String key) {
    switch (key) {
      case 'AUTH_AUTHORITY':
        const value = String.fromEnvironment('AUTH_AUTHORITY');
        return value.isNotEmpty ? value : null;
      case 'API_BASE_URL':
        const value = String.fromEnvironment('API_BASE_URL');
        return value.isNotEmpty ? value : null;
      case 'CLIENT_ID':
        const value = String.fromEnvironment('CLIENT_ID');
        return value.isNotEmpty ? value : null;
      case 'REDIRECT_URI':
        const value = String.fromEnvironment('REDIRECT_URI');
        return value.isNotEmpty ? value : null;
      case 'POST_LOGOUT_REDIRECT_URI':
        const value = String.fromEnvironment('POST_LOGOUT_REDIRECT_URI');
        return value.isNotEmpty ? value : null;
      case 'SCOPES':
        const value = String.fromEnvironment('SCOPES');
        return value.isNotEmpty ? value : null;
      case 'APP_NAME':
        const value = String.fromEnvironment('APP_NAME');
        return value.isNotEmpty ? value : null;
      case 'ENVIRONMENT':
        const value = String.fromEnvironment('ENVIRONMENT');
        return value.isNotEmpty ? value : null;
      default:
        return null;
    }
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
        'Ensure you have a valid .env file or --dart-define flags set.',
      );
    }
  }

  /// Debug helper to print current configuration and its source
  /// Call this after app initialization to verify config is loaded correctly
  static void printConfigSource() {
    debugPrint('');
    debugPrint('╔══════════════════════════════════════════════════════════════╗');
    debugPrint('║                 ENVIRONMENT CONFIGURATION                     ║');
    debugPrint('╠══════════════════════════════════════════════════════════════╣');
    debugPrint('║ Source priority: dart-define > dotenv > fallback             ║');
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
    final source = _getSource(key == 'POST_LOGOUT_URI' ? 'POST_LOGOUT_REDIRECT_URI' : key);
    final status = value.isNotEmpty ? '✓' : '✗';
    final displayValue = value.isNotEmpty
        ? (value.length > 35 ? '${value.substring(0, 35)}...' : value)
        : '(missing)';
    debugPrint('║ $status $key: $displayValue');
    debugPrint('║   └─ source: $source');
  }

  static String _getSource(String key) {
    final systemValue = _getSystemEnv(key);
    if (systemValue != null && systemValue.isNotEmpty) {
      return 'dart-define';
    }
    try {
      final dotenvValue = dotenv.maybeGet(key);
      if (dotenvValue != null && dotenvValue.isNotEmpty) {
        return 'dotenv';
      }
    } catch (e) {
      // dotenv not initialized
    }
    return 'fallback';
  }
}
