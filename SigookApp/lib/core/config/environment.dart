import 'package:flutter_dotenv/flutter_dotenv.dart';

enum Environment { staging, production }

class EnvironmentConfig {
  static String get authority => dotenv.get('AUTH_AUTHORITY');

  static String get apiBaseUrl => dotenv.get('API_BASE_URL');

  static String get clientId => dotenv.get('CLIENT_ID');

  static String get redirectUri => dotenv.get('REDIRECT_URI');

  static String get postLogoutRedirectUri =>
      dotenv.get('POST_LOGOUT_REDIRECT_URI');

  static List<String> get scopes {
    final scopesString = dotenv.get('SCOPES');
    return scopesString.split(',').map((s) => s.trim()).toList();
  }

  static String get appName => dotenv.get('APP_NAME');

  static Environment get current {
    final envString = dotenv.get('ENVIRONMENT').toLowerCase();
    return envString == 'production'
        ? Environment.production
        : Environment.staging;
  }

  static String get environmentName {
    return current == Environment.production ? 'Production' : 'Staging';
  }

  static bool get isProduction => current == Environment.production;

  static bool get isStaging => current == Environment.staging;
}
