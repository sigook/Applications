import '../../../../../core/network/api_client.dart';
import '../../../data/datasources/profile_base_datasource.dart';
import '../../domain/entities/job_experience.dart';

class JobExperienceRemoteDataSource extends ProfileBaseDatasource {
  @override
  final ApiClient apiClient;

  JobExperienceRemoteDataSource({required this.apiClient});

  Future<void> addJobExperience(
    String workerId, {
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      execute(() async {
        await apiClient.dio.post(
          '/WorkerProfile/$workerId/JobExperience',
          data: {
            'company': company,
            if (supervisor?.isNotEmpty ?? false) 'supervisor': supervisor,
            if (duties?.isNotEmpty ?? false) 'duties': duties,
            'startDate': startDate,
            if (endDate != null) 'endDate': endDate,
            'isCurrentJobPosition': isCurrentJobPosition,
          },
        );
      });

  Future<void> updateJobExperience(
    String workerId,
    String experienceId, {
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      execute(() async {
        await apiClient.dio.put(
          '/WorkerProfile/$workerId/JobExperience/$experienceId',
          data: {
            'company': company,
            if (supervisor?.isNotEmpty ?? false) 'supervisor': supervisor,
            if (duties?.isNotEmpty ?? false) 'duties': duties,
            'startDate': startDate,
            if (endDate != null) 'endDate': endDate,
            'isCurrentJobPosition': isCurrentJobPosition,
          },
        );
      });

  Future<void> deleteJobExperience(String workerId, String experienceId) =>
      execute(() async {
        await apiClient.dio.delete(
          '/WorkerProfile/$workerId/JobExperience/$experienceId',
        );
      });

  Future<List<JobExperience>> fetchJobExperiences() => execute(() async {
        final response = await apiClient.dio.get('/WorkerProfile/me');
        final json = response.data as Map<String, dynamic>;
        final list = json['jobExperiences'] as List<dynamic>? ?? [];
        return list
            .cast<Map<String, dynamic>>()
            .map(JobExperience.fromJson)
            .toList();
      });
}
