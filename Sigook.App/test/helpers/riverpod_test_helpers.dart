import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

/// Registers an automatic [addTearDown] to dispose the container after each
/// test and returns it for immediate use.
///
/// Usage:
/// ```dart
/// final container = buildContainer(ProviderContainer(overrides: [...]));
/// ```
ProviderContainer buildContainer(ProviderContainer container) {
  addTearDown(container.dispose);
  return container;
}
