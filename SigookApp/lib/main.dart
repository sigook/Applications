import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'main_common.dart';

/// Default entry point for development
/// Loads staging environment by default
///
/// For flavor-specific builds, use:
/// - flutter run --flavor staging -t lib/main_staging.dart
/// - flutter run --flavor production -t lib/main_production.dart
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Load staging environment by default for development
  await dotenv.load(fileName: '.env.staging');

  // Run the common main app
  await mainCommon();
}
