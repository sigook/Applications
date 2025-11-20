import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod/riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';
import '../network/api_client.dart';
import '../network/network_info.dart';

/// Global SharedPreferences provider
/// Must be overridden in main.dart with an actual instance
final sharedPreferencesProvider = Provider<SharedPreferences>((ref) {
  throw UnimplementedError(
    'SharedPreferences must be initialized in main.dart',
  );
});

/// Global FlutterSecureStorage provider for secure auth token storage
/// Must be overridden in main.dart with an actual instance
final secureStorageProvider = Provider<FlutterSecureStorage>((ref) {
  throw UnimplementedError(
    'FlutterSecureStorage must be initialized in main.dart',
  );
});

/// API client provider for HTTP requests
/// Note: Auth interceptor is added separately to avoid circular dependencies
final apiClientProvider = Provider<ApiClient>((ref) {
  // Create API client without auth interceptor initially
  // The auth interceptor is added via the Dio instance's interceptors list
  // after initialization to avoid circular dependencies
  return ApiClient();
});

/// Network connectivity provider
final networkInfoProvider = Provider<NetworkInfo>((ref) {
  return NetworkInfoImpl(Connectivity());
});
