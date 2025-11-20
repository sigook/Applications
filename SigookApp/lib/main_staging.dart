import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'main_common.dart';

/// Staging environment entry point
/// Run with: flutter run --flavor staging -t lib/main_staging.dart
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Load staging environment variables
  await dotenv.load(fileName: '.env.staging');

  // Run the common main app
  await mainCommon();
}
