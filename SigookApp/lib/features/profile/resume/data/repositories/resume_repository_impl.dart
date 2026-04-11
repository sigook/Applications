import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/datasources/profile_remote_datasource.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/resume_repository.dart';

class ResumeRepositoryImpl implements ResumeRepository {
  final ProfileRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  ResumeRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload(String filePath) =>
      guardedProfileCall(networkInfo, () async {
        final workerId = await remoteDataSource.getWorkerId();
        await remoteDataSource.uploadWorkerResume(workerId, filePath: filePath);
      });
}
