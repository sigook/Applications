import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/resume_remote_datasource.dart';
import '../../domain/repositories/resume_repository.dart';

class ResumeRepositoryImpl implements ResumeRepository {
  final ResumeRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  ResumeRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload(String filePath) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.uploadResume(profile.id, filePath: filePath);
      });
}
