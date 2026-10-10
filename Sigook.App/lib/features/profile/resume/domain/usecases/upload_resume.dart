import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/resume_repository.dart';

class UploadResume {
  final ResumeRepository repository;
  UploadResume(this.repository);

  Future<Either<Failure, void>> call(String filePath) =>
      repository.upload(filePath);
}
