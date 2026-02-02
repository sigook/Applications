import 'package:flutter/material.dart';
import 'core/config/env_loader.dart';
import 'main_common.dart';

/// Staging environment entry point
/// Run with: flutter run --flavor staging -t lib/main_staging.dart
///
/// Environment variables are loaded from .env.staging file.
/// Locally, developers maintain their own .env files.
/// In CI/CD, the pipeline generates the .env file from variable groups.
Future<void> main() async {
  try {
    WidgetsFlutterBinding.ensureInitialized();

    // Load staging environment variables from .env file
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
