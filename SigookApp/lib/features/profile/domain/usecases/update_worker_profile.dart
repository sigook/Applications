import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/profile_repository.dart';

class UpdateWorkerProfile implements UseCase<void, UpdateWorkerProfileParams> {
  final ProfileRepository repository;

  UpdateWorkerProfile(this.repository);

  @override
  Future<Either<Failure, void>> call(UpdateWorkerProfileParams params) async {
    return await repository.updateWorkerBasicInfo(
      params.editedFields,
      newFilePaths: params.newFilePaths,
    );
  }
}

class UpdateWorkerProfileParams {
  final Map<String, String> editedFields;
  final Map<String, String>? newFilePaths;

  UpdateWorkerProfileParams({
    required this.editedFields,
    this.newFilePaths,
  });
}
