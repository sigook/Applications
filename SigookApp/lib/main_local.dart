import 'package:flutter/material.dart';
import 'main_common.dart';

/// Local development entry point - connects to locally running services
/// Run with: flutter run --dart-define-from-file=.env.local --flavor production -t lib/main_local.dart
///
/// Prerequisites:
/// 1. Run Covenant.IdentityServer locally
/// 2. Run Covenant.Api locally
/// 3. Create .env.local file with local URLs (see .env.example)
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await mainCommon();
}
