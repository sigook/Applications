import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/sin_remote_datasource.dart';
import '../../domain/repositories/sin_repository.dart';

class SinRepositoryImpl implements SinRepository {
  final SinRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  SinRepositoryImpl({
    required this.datasource,
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

        final current = await datasource.getWorkerProfile();
        final updated = current.copyWith(
          socialInsurance:
              mutableFields['socialInsurance'] ?? current.socialInsurance,
          socialInsuranceFile:
              deleteSinFile ? null : current.socialInsuranceFile,
        );
        await datasource.updateSinInfo(
          current.id,
          updated,
          sinFilePath: filePath,
        );
      });
}
