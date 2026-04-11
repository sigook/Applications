import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/certificates_repository_impl.dart';
import '../../domain/repositories/certificates_repository.dart';
import '../../domain/usecases/upload_certificate.dart';

final certificatesRepositoryProvider = Provider<CertificatesRepository>((ref) {
  return CertificatesRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadCertificateUseCaseProvider = Provider<UploadCertificate>((ref) {
  return UploadCertificate(ref.read(certificatesRepositoryProvider));
});
