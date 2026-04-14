import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/personal_details_remote_datasource.dart';
import '../../domain/repositories/personal_details_repository.dart';

class PersonalDetailsRepositoryImpl implements PersonalDetailsRepository {
  final PersonalDetailsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  PersonalDetailsRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(Map<String, String> fields) =>
      guardedProfileCall(networkInfo, () async {
        final current = await datasource.getWorkerProfile();
        final updated = current.copyWith(
          firstName: fields['firstName'] ?? current.firstName,
          middleName: fields['middleName'] ?? current.middleName,
          lastName: fields['lastName'] ?? current.lastName,
          secondLastName: fields['secondLastName'] ?? current.secondLastName,
          hasVehicle: fields['hasVehicle'] != null
              ? fields['hasVehicle'] == 'true'
              : current.hasVehicle,
        );
        await datasource.updateBasicInfo(current.id, updated);
      });
}
