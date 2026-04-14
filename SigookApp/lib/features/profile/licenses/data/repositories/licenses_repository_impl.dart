import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/licenses_remote_datasource.dart';
import '../../domain/repositories/licenses_repository.dart';

class LicensesRepositoryImpl implements LicensesRepository {
  final LicensesRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  LicensesRepositoryImpl({
    required this.datasource,
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
        final profile = await datasource.getWorkerProfile();
        await datasource.uploadLicense(
          profile.id,
          filePath: filePath,
          number: number,
          issued: issued,
          expires: expires,
        );
      });
}
