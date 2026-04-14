import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/licenses_remote_datasource.dart';
import '../../data/repositories/licenses_repository_impl.dart';
import '../../domain/repositories/licenses_repository.dart';
import '../../domain/usecases/upload_license.dart';

final licensesDatasourceProvider = Provider<LicensesRemoteDataSource>((ref) {
  return LicensesRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final licensesRepositoryProvider = Provider<LicensesRepository>((ref) {
  return LicensesRepositoryImpl(
    datasource: ref.read(licensesDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadLicenseUseCaseProvider = Provider<UploadLicense>((ref) {
  return UploadLicense(ref.read(licensesRepositoryProvider));
});
