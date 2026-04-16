// Basic widget test for Sigook App
// For comprehensive testing, add tests for individual features

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigook_app_flutter/main_common.dart';

void main() {
  testWidgets('App smoke test - verifies app builds', (
    WidgetTester tester,
  ) async {
    await tester.pumpWidget(const ProviderScope(child: MyApp()));

    expect(find.byType(MyApp), findsOneWidget);
  });
}
