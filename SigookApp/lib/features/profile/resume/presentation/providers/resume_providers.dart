import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/resume_repository_impl.dart';
import '../../domain/repositories/resume_repository.dart';
import '../../domain/usecases/upload_resume.dart';

final resumeRepositoryProvider = Provider<ResumeRepository>((ref) {
  return ResumeRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadResumeUseCaseProvider = Provider<UploadResume>((ref) {
  return UploadResume(ref.read(resumeRepositoryProvider));
});
