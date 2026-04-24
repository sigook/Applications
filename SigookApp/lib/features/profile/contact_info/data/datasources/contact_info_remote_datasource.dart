import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

class ContactInfoRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  ContactInfoRemoteDataSource({required this.apiClient});

  Future<void> updateContactInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/ContactInformation',
            data: profile.toJson(),
          ));
}
