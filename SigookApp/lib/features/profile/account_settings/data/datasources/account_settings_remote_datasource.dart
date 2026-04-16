import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class AccountSettingsRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  AccountSettingsRemoteDataSource({required this.apiClient});

  Future<void> changeEmail({
    required String newEmail,
    required String confirmNewEmail,
  }) =>
      execute(() async {
        await apiClient.dio.post('/Account/ChangeEmail', data: {
          'newEmail': newEmail,
          'confirmNewEmail': confirmNewEmail,
        });
      });
}
