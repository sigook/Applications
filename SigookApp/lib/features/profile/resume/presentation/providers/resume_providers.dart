import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/resume_remote_datasource.dart';
import '../../data/repositories/resume_repository_impl.dart';
import '../../domain/repositories/resume_repository.dart';
import '../../domain/usecases/upload_resume.dart';

final resumeDatasourceProvider = Provider<ResumeRemoteDataSource>((ref) {
  return ResumeRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final resumeRepositoryProvider = Provider<ResumeRepository>((ref) {
  return ResumeRepositoryImpl(
    datasource: ref.read(resumeDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadResumeUseCaseProvider = Provider<UploadResume>((ref) {
  return UploadResume(ref.read(resumeRepositoryProvider));
});
