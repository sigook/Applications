import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'main_common.dart';

/// Production environment entry point
/// Run with: flutter run --flavor production -t lib/main_production.dart
///
/// Environment variables can be provided via:
/// - .env.production file (local development)
/// - --dart-define flags (CI/CD builds)
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Try to load production environment variables from .env file
  // This is optional - CI/CD builds use --dart-define instead
  await _loadEnvFile('.env.production');

  // Run the common main app
  await mainCommon();
}

/// Attempt to load .env file, but don't fail if it doesn't exist
/// In CI/CD, environment variables are injected via --dart-define
Future<void> _loadEnvFile(String fileName) async {
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
