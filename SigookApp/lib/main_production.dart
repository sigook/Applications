import 'package:flutter/material.dart';
import 'core/config/env_loader.dart';
import 'main_common.dart';

/// Production environment entry point
/// Run with: flutter run --flavor production -t lib/main_production.dart
///
/// Environment variables are loaded from .env.production file.
/// Locally, developers maintain their own .env files.
/// In CI/CD, the pipeline generates the .env file from variable groups.
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Load production environment variables from .env file
  await EnvLoader.load('.env.production');

  // Run the common main app
  await mainCommon();
}
