import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/licenses_repository_impl.dart';
import '../../domain/repositories/licenses_repository.dart';
import '../../domain/usecases/upload_license.dart';

final licensesRepositoryProvider = Provider<LicensesRepository>((ref) {
  return LicensesRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadLicenseUseCaseProvider = Provider<UploadLicense>((ref) {
  return UploadLicense(ref.read(licensesRepositoryProvider));
});
