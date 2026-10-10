import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/job_experience.dart';

abstract class JobExperienceRepository {
  Future<Either<Failure, void>> add({
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  });

  Future<Either<Failure, List<JobExperience>>> getAll();

  Future<Either<Failure, void>> update({
    required String id,
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  });

  Future<Either<Failure, void>> delete(String id);
}
