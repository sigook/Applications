import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'main_common.dart';

/// Production environment entry point
/// Run with: flutter run --flavor production -t lib/main_production.dart
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Load production environment variables
  await dotenv.load(fileName: '.env.production');

  // Run the common main app
  await mainCommon();
}
