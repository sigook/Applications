import 'dart:io';
import 'package:integration_test/integration_test_driver_extended.dart';

Future<void> main() async {
  final outputDir =
      Platform.environment['SCREENSHOT_DIR'] ?? 'build/store_screenshots';
  final prefix = Platform.environment['SCREENSHOT_PREFIX'] ?? '';

  await integrationDriver(
    onScreenshot: (name, bytes, [args]) async {
      final file = File('$outputDir/$prefix$name.png');
      await file.create(recursive: true);
      await file.writeAsBytes(bytes);
      stdout.writeln('Saved ${file.path} (${bytes.length} bytes)');
      return true;
    },
  );
}
