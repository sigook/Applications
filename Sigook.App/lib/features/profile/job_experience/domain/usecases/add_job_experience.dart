import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/job_experience_repository.dart';

class AddJobExperience {
  final JobExperienceRepository repository;
  AddJobExperience(this.repository);

  Future<Either<Failure, void>> call({
    required String company,
    String? supervisor,
    String? duties,
    required String startDate,
    String? endDate,
    required bool isCurrentJobPosition,
  }) =>
      repository.add(
        company: company,
        supervisor: supervisor,
        duties: duties,
        startDate: startDate,
        endDate: endDate,
        isCurrentJobPosition: isCurrentJobPosition,
      );
}
