import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/job_experience_repository.dart';

class UpdateJobExperience {
  final JobExperienceRepository repository;
  UpdateJobExperience(this.repository);

  Future<Either<Failure, void>> call({
    required String id,
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      repository.update(
        id: id,
        company: company,
        supervisor: supervisor,
        duties: duties,
        startDate: startDate,
        endDate: endDate,
        isCurrentJobPosition: isCurrentJobPosition,
      );
}
