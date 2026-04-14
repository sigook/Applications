import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/certificates_remote_datasource.dart';
import '../../domain/repositories/certificates_repository.dart';

class CertificatesRepositoryImpl implements CertificatesRepository {
  final CertificatesRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  CertificatesRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload(String filePath) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.uploadCertificate(profile.id, filePath: filePath);
      });
}
