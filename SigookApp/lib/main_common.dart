import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'core/config/environment.dart';
import 'core/providers/core_providers.dart';
import 'core/routing/app_router.dart';
import 'core/theme/app_theme.dart';
import 'core/constants/error_messages.dart';
import 'features/auth/presentation/viewmodels/auth_viewmodel.dart';
import 'features/profile/presentation/providers/cached_worker_profile_provider.dart';

Future<void> mainCommon() async {
  try {
    debugPrint('📱  Starting app initialization...');

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
    runApp(_InitErrorApp(error: e.toString()));
  }
}

class _InitErrorApp extends StatelessWidget {
  final String error;
  const _InitErrorApp({required this.error});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        backgroundColor: const Color(0xFF1565C0),
        body: Center(
          child: Padding(
            padding: const EdgeInsets.all(32),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Icon(Icons.error_outline, color: Colors.white, size: 64),
                const SizedBox(height: 24),
                const Text(
                  'Initialization Failed',
                  style: TextStyle(
                    color: Colors.white,
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 16),
                Text(
                  error,
                  style: const TextStyle(color: Colors.white70, fontSize: 13),
                  textAlign: TextAlign.center,
                ),
              ],
            ),
          ),
        ),
      ),
    );
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
    ref.listen<AuthState>(authViewModelProvider, (previous, next) {
      if (previous?.isAuthenticated != true && next.isAuthenticated) {
        // Pre-fetch profile as soon as the user is authenticated so it is
        // already in memory when the drawer (or any other consumer) opens.
        ref.read(cachedWorkerProfileProvider.future).ignore();
      } else if (previous?.isAuthenticated == true && !next.isAuthenticated) {
        // Clear the cached profile on logout so the next user starts fresh.
        ref.invalidate(cachedWorkerProfileProvider);
      }
    });

    return MaterialApp.router(
      title: 'Sigook',
      debugShowCheckedModeBanner: false,
      routerConfig: AppRouter.router,
      theme: AppTheme.lightTheme,
      builder: (context, child) {
        return GestureDetector(
          onTap: () {
            FocusManager.instance.primaryFocus?.unfocus();
          },
          child: child,
        );
      },
    );
  }
}
