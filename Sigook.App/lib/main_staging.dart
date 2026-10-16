import 'package:flutter/material.dart';
import 'main_common.dart';

/// Staging environment entry point
/// Run with: flutter run --dart-define-from-file=.env.staging -t lib/main_staging.dart
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await mainCommon();
}
