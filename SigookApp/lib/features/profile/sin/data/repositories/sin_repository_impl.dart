import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/sin_repository.dart';

class SinRepositoryImpl implements SinRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  SinRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(
    Map<String, String> fields, {
    String? filePath,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final mutableFields = Map<String, String>.from(fields);
        final deleteSinFile = mutableFields.remove('_deleteSinFile') == 'true';

        final workerId = await remoteDataSource.getWorkerId();
        final current = await remoteDataSource.getWorkerFullProfile(workerId);
        final updated = current.copyWith(
          socialInsurance:
              mutableFields['socialInsurance'] ?? current.socialInsurance,
          socialInsuranceFile:
              deleteSinFile ? null : current.socialInsuranceFile,
        );
        await remoteDataSource.updateWorkerSinInfo(
          workerId,
          updated,
          sinFilePath: filePath,
        );
      });
}
