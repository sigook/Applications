import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/worker_profile.dart';
import '../usecases/update_worker_profile.dart';

abstract class ProfileRepository {
  Future<Either<Failure, WorkerProfile>> getWorkerBasicInfo();
  Future<Either<Failure, void>> updateWorkerBasicInfo(
    Map<String, String> editedFields, {
    required ProfileSection section,
    Map<String, String>? newFilePaths,
  });
}
