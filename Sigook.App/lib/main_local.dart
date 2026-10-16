import 'package:flutter/material.dart';
import 'main_common.dart';

/// Local development entry point - connects to locally running services
/// Run with: flutter run --dart-define-from-file=.env.local -t lib/main_local.dart
///
/// Prerequisite: Covenant.Api running locally (it also serves the OAuth endpoints)
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await mainCommon();
}
