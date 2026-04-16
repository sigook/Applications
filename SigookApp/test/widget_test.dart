// Basic widget test for Sigook App
// For comprehensive testing, add tests for individual features

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:sigook_app_flutter/core/providers/core_providers.dart';
import 'package:sigook_app_flutter/main_common.dart';

class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

void main() {
  testWidgets('App smoke test - verifies app builds', (WidgetTester tester) async {
    SharedPreferences.setMockInitialValues({});
    final prefs = await SharedPreferences.getInstance();
    final mockStorage = MockFlutterSecureStorage();

    // AuthViewModel reads the cached token on startup
    when(() => mockStorage.read(key: any(named: 'key')))
        .thenAnswer((_) async => null);

    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          secureStorageProvider.overrideWithValue(mockStorage),
          sharedPreferencesProvider.overrideWithValue(prefs),
        ],
        child: const MyApp(),
      ),
    );

    expect(find.byType(MyApp), findsOneWidget);

    // Advance past SplashScreen timers: 120ms (logo animation) + 3s (auto-navigate)
    await tester.pump(const Duration(seconds: 4));
    await tester.pumpAndSettle();
  });
}
