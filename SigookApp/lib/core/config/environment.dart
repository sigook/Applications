import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

enum Environment { staging, production }

/// Environment configuration that supports both:
/// - System environment variables (Azure DevOps CI/CD)
/// - .env files via flutter_dotenv (local development)
///
/// Priority: System env vars > dotenv > fallback (empty string)
class EnvironmentConfig {
  /// Get environment variable with fallback chain:
  /// 1. System environment variables (Platform.environment)
  /// 2. dotenv loaded values
  /// 3. Fallback value (default: empty string)
  static String _getEnv(String key, {String fallback = ''}) {
    // First try system environment variables (CI/CD)
    final systemValue = _getSystemEnv(key);
    if (systemValue != null && systemValue.isNotEmpty) {
      return systemValue;
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
    return envString == 'production'
        ? Environment.production
        : Environment.staging;
  }

  static String get environmentName {
    return current == Environment.production ? 'Production' : 'Staging';
  }

  static bool get isProduction => current == Environment.production;

  static bool get isStaging => current == Environment.staging;

  /// Debug helper to print current configuration source
  static void printConfigSource() {
    debugPrint('🔧 Environment Configuration:');
    debugPrint(
      '   ENVIRONMENT: ${_getEnv('ENVIRONMENT')} (${_getSource('ENVIRONMENT')})',
    );
    debugPrint(
      '   AUTH_AUTHORITY: ${authority.isNotEmpty ? '✓ set' : '✗ missing'} (${_getSource('AUTH_AUTHORITY')})',
    );
    debugPrint(
      '   API_BASE_URL: ${apiBaseUrl.isNotEmpty ? '✓ set' : '✗ missing'} (${_getSource('API_BASE_URL')})',
    );
    debugPrint(
      '   CLIENT_ID: ${clientId.isNotEmpty ? '✓ set' : '✗ missing'} (${_getSource('CLIENT_ID')})',
    );
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
