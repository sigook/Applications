import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/emergency_repository.dart';

class EmergencyRepositoryImpl implements EmergencyRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  EmergencyRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(Map<String, String> fields) =>
      guardedProfileCall(networkInfo, () async {
        final workerId = await remoteDataSource.getWorkerId();
        final current = await remoteDataSource.getWorkerFullProfile(workerId);
        final updated = current.copyWith(
          contactEmergencyName:
              fields['contactEmergencyName'] ?? current.contactEmergencyName,
          contactEmergencyLastName: fields['contactEmergencyLastName'] ??
              current.contactEmergencyLastName,
          contactEmergencyPhone:
              fields['contactEmergencyPhone'] ?? current.contactEmergencyPhone,
        );
        await remoteDataSource.updateWorkerEmergencyInfo(workerId, updated);
      });
}
