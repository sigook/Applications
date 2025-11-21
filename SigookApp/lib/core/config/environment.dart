import 'package:flutter_dotenv/flutter_dotenv.dart';

/// Environment types
enum Environment { staging, production }

/// Environment configuration holder
class EnvironmentConfig {
  /// Get OAuth Authority URL from environment
  static String get authority => dotenv.get('AUTH_AUTHORITY');

  /// Get API Base URL from environment
  static String get apiBaseUrl => dotenv.get('API_BASE_URL');

  /// Get OAuth Client ID from environment
  static String get clientId => dotenv.get('CLIENT_ID');

  /// Get OAuth Redirect URI from environment
  static String get redirectUri => dotenv.get('REDIRECT_URI');

  /// Get OAuth Post Logout Redirect URI from environment
  static String get postLogoutRedirectUri =>
      dotenv.get('POST_LOGOUT_REDIRECT_URI');

  /// Get OAuth Scopes from environment (comma-separated in .env)
  static List<String> get scopes {
    final scopesString = dotenv.get('SCOPES');
    return scopesString.split(',').map((s) => s.trim()).toList();
  }

  /// Get App Name from environment
  static String get appName => dotenv.get('APP_NAME');

  /// Get current environment type
  static Environment get current {
    final envString = dotenv.get('ENVIRONMENT').toLowerCase();
    return envString == 'production'
        ? Environment.production
        : Environment.staging;
  }

  /// Environment display name
  static String get environmentName {
    return current == Environment.production ? 'Production' : 'Staging';
  }

  /// Check if current environment is production
  static bool get isProduction => current == Environment.production;

  /// Check if current environment is staging
  static bool get isStaging => current == Environment.staging;
}
