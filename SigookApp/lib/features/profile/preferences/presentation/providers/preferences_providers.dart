import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/preferences_remote_datasource.dart';
import '../../data/repositories/preferences_repository_impl.dart';
import '../../domain/repositories/preferences_repository.dart';
import '../../domain/usecases/update_preferences.dart';

final preferencesDatasourceProvider =
    Provider<PreferencesRemoteDataSource>((ref) {
  return PreferencesRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final preferencesRepositoryProvider = Provider<PreferencesRepository>((ref) {
  return PreferencesRepositoryImpl(
    datasource: ref.read(preferencesDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updatePreferencesUseCaseProvider = Provider<UpdatePreferences>((ref) {
  return UpdatePreferences(ref.read(preferencesRepositoryProvider));
});
