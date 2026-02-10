import 'package:flutter/foundation.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

/// Utility for loading environment configuration files.
/// Used by entry points (main_staging.dart, main_production.dart, main_local.dart).
///
/// In local development, developers maintain their own .env files.
/// In CI/CD, the pipeline generates the .env file from Azure DevOps variable groups.
class EnvLoader {
  /// Load environment configuration from the specified .env file.
  /// Throws if the file is missing or cannot be loaded.
  ///
  /// [fileName] - The name of the .env file (e.g., '.env.staging', '.env.production')
  static Future<void> load(String fileName) async {
    try {
      await dotenv.load(fileName: fileName);
      debugPrint('Loaded environment from $fileName');
    } catch (e) {
      debugPrint('ERROR: Failed to load $fileName: $e');
      debugPrint('For local development, copy .env.example to $fileName');
      debugPrint('For CI/CD, ensure the pipeline generates $fileName');
      rethrow;
    }
  }
}
