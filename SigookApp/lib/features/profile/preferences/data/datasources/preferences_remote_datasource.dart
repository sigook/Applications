import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';

class PreferencesRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  PreferencesRemoteDataSource({required this.apiClient});

  Future<void> _post(String workerId, String path, Object data) =>
      execute(() => apiClient.dio.post(
            '/WorkerProfile/$workerId/$path',
            data: data,
          ));

  Future<void> updateOtherInformation(
    String workerId, {
    required Map<String, String> lift,
  }) =>
      _post(workerId, 'OtherInformation', {'lift': lift});

  Future<void> updateAvailabilities(
    String workerId, {
    required List<Map<String, String>> availabilities,
  }) =>
      _post(workerId, 'Availabilities', availabilities);

  Future<void> updateAvailabilityTimes(
    String workerId, {
    required List<Map<String, String>> availabilityTimes,
  }) =>
      _post(workerId, 'AvailabilityTimes', availabilityTimes);

  Future<void> updateAvailabilityDays(
    String workerId, {
    required List<Map<String, String>> availabilityDays,
  }) =>
      _post(workerId, 'AvailabilityDays', availabilityDays);

  Future<void> updateLocationPreferences(
    String workerId, {
    required List<Map<String, String>> locationPreferences,
  }) =>
      _post(workerId, 'LocationPreferences', locationPreferences);

  Future<void> updateSkills(
    String workerId, {
    required List<String> skills,
  }) =>
      _post(workerId, 'Skills', skills);

  Future<void> updateLanguages(
    String workerId, {
    required List<Map<String, String>> languages,
  }) =>
      _post(workerId, 'Languages', languages);
}
