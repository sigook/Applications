import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'core/config/environment.dart';
import 'core/providers/core_providers.dart';
import 'core/routing/app_router.dart';
import 'core/theme/app_theme.dart';
import 'core/constants/error_messages.dart';
import 'core/widgets/navbar_logo.dart';

Future<void> mainCommon() async {
  try {

    //comment
    debugPrint('📱 Starting app initialization...');

    EnvironmentConfig.printConfigSource();

    EnvironmentConfig.validateRequiredConfig();

    debugPrint('📦 Loading SharedPreferences...');
    final sharedPreferences = await SharedPreferences.getInstance();

    debugPrint('🌐 Loading error messages...');
    await ErrorMessages.load();

    debugPrint('🔐 Initializing secure storage...');
    const secureStorage = FlutterSecureStorage(
      aOptions: AndroidOptions(encryptedSharedPreferences: true),
    );

    debugPrint('✅ App initialization complete, running app...');
    runApp(
      ProviderScope(
        overrides: [
          sharedPreferencesProvider.overrideWithValue(sharedPreferences),
          secureStorageProvider.overrideWithValue(secureStorage),
        ],
        child: const MyApp(),
      ),
    );
  } catch (e, stackTrace) {
    debugPrint('❌ Error in mainCommon:');
    debugPrint('Error: $e');
    debugPrint('Stack trace: $stackTrace');
    rethrow;
  }
}

class MyApp extends ConsumerStatefulWidget {
  const MyApp({super.key});

  @override
  ConsumerState<MyApp> createState() => _MyAppState();
}

class _MyAppState extends ConsumerState<MyApp> {
  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Sigook',
      debugShowCheckedModeBanner: false,
      routerConfig: AppRouter.router,
      theme: AppTheme.lightTheme,
      builder: (context, child) {
        return Listener(
          onPointerUp: (_) {
            globalTapNotifier.value++;
          },
          child: GestureDetector(
            onTap: () {
              FocusManager.instance.primaryFocus?.unfocus();
            },
            child: child,
          ),
        );
      },
    );
  }
}
