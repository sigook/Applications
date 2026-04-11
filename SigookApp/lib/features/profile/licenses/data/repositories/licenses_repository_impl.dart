import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/licenses_repository.dart';

class LicensesRepositoryImpl implements LicensesRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  LicensesRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload({
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final workerId = await remoteDataSource.getWorkerId();
        await remoteDataSource.uploadWorkerLicense(
          workerId,
          filePath: filePath,
          number: number,
          issued: issued,
          expires: expires,
        );
      });
}
