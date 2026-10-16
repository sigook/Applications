import 'package:flutter/material.dart';
import 'main_common.dart';

/// Default entry point for development
/// Loads staging environment by default
///
/// Run with: flutter run --dart-define-from-file=.env.staging
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await mainCommon();
}
