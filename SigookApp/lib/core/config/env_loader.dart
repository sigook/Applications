import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

/// Utility for loading environment configuration files.
/// Used by entry points (main_local.dart, main_staging.dart, main_production.dart)
class EnvLoader {
  /// Attempt to load .env file, but don't fail if it doesn't exist.
  /// In CI/CD, environment variables are injected via --dart-define,
  /// so the .env file won't be present.
  ///
  /// [fileName] - The name of the .env file (e.g., '.env.staging', '.env.local')
  static Future<void> load(String fileName) async {
    try {
      await dotenv.load(fileName: fileName);
      debugPrint('✅ Loaded environment from $fileName');
    } catch (e) {
      // .env file not found - this is expected in CI/CD builds
      // Environment variables will be provided via --dart-define
      debugPrint(
        'ℹ️ $fileName not found, using dart-define environment variables',
      );
    }
  }
}
