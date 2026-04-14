import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

class PersonalDetailsRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  PersonalDetailsRemoteDataSource({required this.apiClient});

  Future<void> updateBasicInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/BasicInformation',
            data: profile.toJson(),
          ));
}
