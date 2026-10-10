import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/job_experience_remote_datasource.dart';
import '../../data/repositories/job_experience_repository_impl.dart';
import '../../domain/entities/job_experience.dart';
import '../../domain/repositories/job_experience_repository.dart';
import '../../domain/usecases/add_job_experience.dart';
import '../../domain/usecases/update_job_experience.dart';
import '../../domain/usecases/delete_job_experience.dart';

final jobExperienceDatasourceProvider =
    Provider<JobExperienceRemoteDataSource>((ref) {
  return JobExperienceRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final jobExperienceRepositoryProvider =
    Provider<JobExperienceRepository>((ref) {
  return JobExperienceRepositoryImpl(
    datasource: ref.read(jobExperienceDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final addJobExperienceUseCaseProvider = Provider<AddJobExperience>((ref) {
  return AddJobExperience(ref.read(jobExperienceRepositoryProvider));
});

final updateJobExperienceUseCaseProvider = Provider<UpdateJobExperience>((ref) {
  return UpdateJobExperience(ref.read(jobExperienceRepositoryProvider));
});

final deleteJobExperienceUseCaseProvider = Provider<DeleteJobExperience>((ref) {
  return DeleteJobExperience(ref.read(jobExperienceRepositoryProvider));
});

final jobExperienceListProvider = FutureProvider<List<JobExperience>>((ref) async {
  final repo = ref.read(jobExperienceRepositoryProvider);
  final result = await repo.getAll();
  return result.fold(
    (failure) => throw Exception(failure.message),
    (list) => list,
  );
});
