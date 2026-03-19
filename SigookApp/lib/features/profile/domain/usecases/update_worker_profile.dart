import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/profile_repository.dart';

enum ProfileSection { personal, contact, sin, documents, resume, emergency, preferences }

class UpdateWorkerProfile implements UseCase<void, UpdateWorkerProfileParams> {
  final ProfileRepository repository;

  UpdateWorkerProfile(this.repository);

  @override
  Future<Either<Failure, void>> call(UpdateWorkerProfileParams params) async {
    return await repository.updateWorkerBasicInfo(
      params.editedFields,
      section: params.section,
      newFilePaths: params.newFilePaths,
    );
  }
}

class UpdateWorkerProfileParams {
  final Map<String, String> editedFields;
  final ProfileSection section;
  final Map<String, String>? newFilePaths;

  UpdateWorkerProfileParams({
    required this.editedFields,
    required this.section,
    this.newFilePaths,
  });
}
