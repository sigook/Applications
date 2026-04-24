import 'package:dartz/dartz.dart';
import '../../../../../core/error/exceptions.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/entities/job_experience.dart';
import '../../domain/repositories/job_experience_repository.dart';
import '../datasources/job_experience_remote_datasource.dart';

class JobExperienceRepositoryImpl implements JobExperienceRepository {
  final JobExperienceRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  JobExperienceRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> add({
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.addJobExperience(
          profile.id,
          company: company,
          supervisor: supervisor,
          duties: duties,
          startDate: startDate,
          endDate: endDate,
          isCurrentJobPosition: isCurrentJobPosition,
        );
      });

  @override
  Future<Either<Failure, void>> update({
    required String id,
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.updateJobExperience(
          profile.id,
          id,
          company: company,
          supervisor: supervisor,
          duties: duties,
          startDate: startDate,
          endDate: endDate,
          isCurrentJobPosition: isCurrentJobPosition,
        );
      });

  @override
  Future<Either<Failure, void>> delete(String id) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.deleteJobExperience(profile.id, id);
      });

  @override
  Future<Either<Failure, List<JobExperience>>> getAll() async {
    if (!await networkInfo.isConnected) return Left(NetworkFailure());
    try {
      final list = await datasource.fetchJobExperiences();
      return Right(list);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: $e'));
    }
  }
}
