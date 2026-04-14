import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/models/worker_profile_model.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/contact_info_remote_datasource.dart';
import '../../domain/repositories/contact_info_repository.dart';

class ContactInfoRepositoryImpl implements ContactInfoRepository {
  final ContactInfoRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  ContactInfoRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(Map<String, String> fields) =>
      guardedProfileCall(networkInfo, () async {
        final current = await datasource.getWorkerProfile();
        final updated = current.copyWith(
          mobileNumber: fields['mobileNumber'] ?? current.mobileNumber,
          phone: fields['phone'] ?? current.phone,
          location: current.location?.copyWith(
            address: fields['address'] ?? current.location?.address,
            postalCode: fields['postalCode'] ?? current.location?.postalCode,
            city: fields['cityId'] != null
                ? CityModel(
                    id: fields['cityId'],
                    value: '',
                    province: current.location?.city?.province,
                  )
                : current.location?.city,
          ),
        );
        await datasource.updateContactInfo(current.id, updated);
      });
}
