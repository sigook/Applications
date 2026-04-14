import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../../data/models/worker_profile_model.dart';

class EmergencyRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  EmergencyRemoteDataSource({required this.apiClient});

  Future<void> updateEmergencyInfo(
    String workerId,
    WorkerProfileModel profile,
  ) =>
      execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/EmergencyInformation',
            data: {
              'haveAnyHealthProblem': profile.haveAnyHealthProblem,
              if (profile.healthProblem != null)
                'healthProblem': profile.healthProblem,
              if (profile.otherHealthProblem != null)
                'otherHealthProblem': profile.otherHealthProblem,
              if (profile.contactEmergencyName != null)
                'contactEmergencyName': profile.contactEmergencyName,
              if (profile.contactEmergencyLastName != null)
                'contactEmergencyLastName': profile.contactEmergencyLastName,
              if (profile.contactEmergencyPhone != null)
                'contactEmergencyPhone': profile.contactEmergencyPhone,
            },
          ));
}
