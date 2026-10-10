import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/worker_profile.dart';
import '../repositories/profile_repository.dart';

class GetWorkerProfile implements UseCase<WorkerProfile, NoParams> {
  final ProfileRepository repository;

  GetWorkerProfile(this.repository);

  @override
  Future<Either<Failure, WorkerProfile>> call(NoParams params) async {
    return await repository.getWorkerBasicInfo();
  }
}
