import 'package:flutter/material.dart';
import 'core/config/env_loader.dart';
import 'main_common.dart';

/// Staging environment entry point
/// Run with: flutter run --flavor staging -t lib/main_staging.dart
///
/// Environment variables can be provided via:
/// - .env.staging file (local development)
/// - --dart-define flags (CI/CD builds)
Future<void> main() async {
  try {
    WidgetsFlutterBinding.ensureInitialized();

    // Try to load staging environment variables from .env file
    // This is optional - CI/CD builds use --dart-define instead
    await EnvLoader.load('.env.staging');

    // Run the common main app
    await mainCommon();
  } catch (e, stackTrace) {
    debugPrint('❌ App crashed during initialization:');
    debugPrint('Error: $e');
    debugPrint('Stack trace: $stackTrace');
    rethrow;
  }
}
