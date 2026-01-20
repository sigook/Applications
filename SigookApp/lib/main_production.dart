import 'package:flutter/material.dart';
import 'core/config/env_loader.dart';
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
  await EnvLoader.load('.env.production');

  // Run the common main app
  await mainCommon();
}
