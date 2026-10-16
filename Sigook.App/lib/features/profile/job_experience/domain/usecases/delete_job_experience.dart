import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/job_experience_repository.dart';

class DeleteJobExperience {
  final JobExperienceRepository repository;
  DeleteJobExperience(this.repository);

  Future<Either<Failure, void>> call(String id) => repository.delete(id);
}
