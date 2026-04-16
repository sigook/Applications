import 'package:flutter/material.dart';
import 'main_common.dart';

/// Production environment entry point
/// Run with: flutter run --dart-define-from-file=.env.production -t lib/main_production.dart
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await mainCommon();
}
