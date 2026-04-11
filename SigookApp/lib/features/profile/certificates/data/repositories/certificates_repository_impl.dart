import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/certificates_repository.dart';

class CertificatesRepositoryImpl implements CertificatesRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  CertificatesRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload(String filePath) =>
      guardedProfileCall(networkInfo, () async {
        final workerId = await remoteDataSource.getWorkerId();
        await remoteDataSource.uploadWorkerCertificate(
          workerId,
          filePath: filePath,
        );
      });
}
