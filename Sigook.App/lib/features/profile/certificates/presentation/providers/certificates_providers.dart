import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/certificates_remote_datasource.dart';
import '../../data/repositories/certificates_repository_impl.dart';
import '../../domain/repositories/certificates_repository.dart';
import '../../domain/usecases/delete_certificate.dart';
import '../../domain/usecases/upload_certificate.dart';

final certificatesDatasourceProvider =
    Provider<CertificatesRemoteDataSource>((ref) {
  return CertificatesRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final certificatesRepositoryProvider = Provider<CertificatesRepository>((ref) {
  return CertificatesRepositoryImpl(
    datasource: ref.read(certificatesDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadCertificateUseCaseProvider = Provider<UploadCertificate>((ref) {
  return UploadCertificate(ref.read(certificatesRepositoryProvider));
});

final deleteCertificateUseCaseProvider = Provider<DeleteCertificate>((ref) {
  return DeleteCertificate(ref.read(certificatesRepositoryProvider));
});
