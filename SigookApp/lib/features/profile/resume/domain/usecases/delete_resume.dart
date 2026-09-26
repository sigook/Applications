import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/resume_repository.dart';

class DeleteResume {
  final ResumeRepository repository;
  DeleteResume(this.repository);

  Future<Either<Failure, void>> call() => repository.delete();
}
