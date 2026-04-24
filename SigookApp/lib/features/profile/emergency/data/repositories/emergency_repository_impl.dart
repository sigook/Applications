import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/emergency_remote_datasource.dart';
import '../../domain/repositories/emergency_repository.dart';

class EmergencyRepositoryImpl implements EmergencyRepository {
  final EmergencyRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  EmergencyRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(Map<String, String> fields) =>
      guardedProfileCall(networkInfo, () async {
        final current = await datasource.getWorkerProfile();
        final updated = current.copyWith(
          haveAnyHealthProblem: fields.containsKey('haveAnyHealthProblem')
              ? fields['haveAnyHealthProblem'] == 'true'
              : current.haveAnyHealthProblem,
          contactEmergencyName:
              fields['contactEmergencyName'] ?? current.contactEmergencyName,
          contactEmergencyLastName: fields['contactEmergencyLastName'] ??
              current.contactEmergencyLastName,
          contactEmergencyPhone:
              fields['contactEmergencyPhone'] ?? current.contactEmergencyPhone,
        );
        await datasource.updateEmergencyInfo(current.id, updated);
      });
}
