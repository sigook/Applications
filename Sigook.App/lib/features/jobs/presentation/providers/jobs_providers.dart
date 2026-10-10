import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/jobs_remote_datasource.dart';
import '../../data/datasources/jobs_local_datasource.dart';
import '../../data/repositories/jobs_repository_impl.dart';
import '../../domain/repositories/jobs_repository.dart';
import '../../domain/usecases/get_jobs.dart';
import '../../domain/usecases/get_job_details.dart';
import '../../domain/usecases/apply_to_job.dart';

final jobsRemoteDataSourceProvider = Provider<JobsRemoteDataSource>((ref) {
  return JobsRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final jobsLocalDataSourceProvider = Provider<JobsLocalDataSource>((ref) {
  return JobsLocalDataSourceImpl();
});

final jobsRepositoryProvider = Provider<JobsRepository>((ref) {
  return JobsRepositoryImpl(
    remoteDataSource: ref.read(jobsRemoteDataSourceProvider),
    localDataSource: ref.read(jobsLocalDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getJobsUseCaseProvider = Provider<GetJobs>((ref) {
  return GetJobs(ref.read(jobsRepositoryProvider));
});

final getJobDetailsUseCaseProvider = Provider<GetJobDetails>((ref) {
  return GetJobDetails(ref.read(jobsRepositoryProvider));
});

final applyToJobUseCaseProvider = Provider<ApplyToJob>((ref) {
  return ApplyToJob(ref.read(jobsRepositoryProvider));
});
