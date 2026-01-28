import 'package:flutter/material.dart';
import 'core/config/env_loader.dart';
import 'main_common.dart';

/// Local development entry point - connects to locally running services
/// Run with: flutter run --flavor staging -t lib/main_local.dart
///
/// This profile is for LOCAL DEVELOPMENT ONLY:
/// - Connects to locally running IdentityServer (localhost)
/// - Connects to locally running Covenant.Api (localhost)
/// - NOT used by CI/CD pipeline (pipeline only uses staging/production)
///
/// Prerequisites:
/// 1. Run Covenant.IdentityServer locally
/// 2. Run Covenant.Api locally
/// 3. Create .env.local file with local URLs (see .env.example)
///
/// Note: Uses --flavor staging for Android build variant, but loads .env.local
Future<void> main() async {
  try {
    WidgetsFlutterBinding.ensureInitialized();

    // Load local environment variables from .env.local
    // This file points to localhost services
    await EnvLoader.load('.env.local');

    // Run the common main app
    await mainCommon();
  } catch (e, stackTrace) {
    debugPrint('❌ App crashed during initialization:');
    debugPrint('Error: $e');
    debugPrint('Stack trace: $stackTrace');
    rethrow;
  }
}
