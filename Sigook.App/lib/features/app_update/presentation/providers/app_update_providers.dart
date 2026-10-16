import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/providers/core_providers.dart';
import '../../data/datasources/app_update_remote_datasource.dart';
import '../../data/repositories/app_update_repository_impl.dart';
import '../../domain/entities/app_update_check.dart';
import '../../domain/repositories/app_update_repository.dart';
import '../../domain/usecases/check_app_update.dart';

final appUpdateRemoteDataSourceProvider = Provider<AppUpdateRemoteDataSource>((
  ref,
) {
  return AppUpdateRemoteDataSourceImpl(apiClient: ref.read(apiClientProvider));
});

final appUpdateRepositoryProvider = Provider<AppUpdateRepository>((ref) {
  return AppUpdateRepositoryImpl(
    remoteDataSource: ref.read(appUpdateRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final checkAppUpdateProvider = Provider<CheckAppUpdate>((ref) {
  return CheckAppUpdate(ref.read(appUpdateRepositoryProvider));
});

/// Null means "could not check" (offline, API down): the app must never be
/// locked out because of a failed check, only because of a confirmed one.
final appUpdateCheckProvider = FutureProvider<AppUpdateCheck?>((ref) async {
  final info = await PackageInfo.fromPlatform();
  final result = await ref.read(checkAppUpdateProvider)(info.version);
  return result.fold(
    (failure) {
      debugPrint('⬆️ [APP UPDATE] Check unavailable: ${failure.message}');
      return null;
    },
    (check) {
      debugPrint(
        '⬆️ [APP UPDATE] current ${check.currentVersion}, minimum '
        '${check.requirement.minimumVersion}, required: ${check.updateRequired}',
      );
      return check;
    },
  );
});
