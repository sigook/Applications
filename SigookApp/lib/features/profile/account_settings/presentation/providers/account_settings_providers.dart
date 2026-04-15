import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/account_settings_remote_datasource.dart';
import '../../data/repositories/account_settings_repository_impl.dart';
import '../../domain/repositories/account_settings_repository.dart';
import '../../domain/usecases/change_email.dart';

final accountSettingsDatasourceProvider =
    Provider<AccountSettingsRemoteDataSource>((ref) {
  return AccountSettingsRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final accountSettingsRepositoryProvider =
    Provider<AccountSettingsRepository>((ref) {
  return AccountSettingsRepositoryImpl(
    datasource: ref.read(accountSettingsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final changeEmailUseCaseProvider = Provider<ChangeEmail>((ref) {
  return ChangeEmail(ref.read(accountSettingsRepositoryProvider));
});
