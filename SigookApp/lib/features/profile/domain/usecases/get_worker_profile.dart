import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/worker_profile.dart';
import '../repositories/profile_repository.dart';

class GetWorkerProfile
    implements UseCase<WorkerProfile, GetWorkerProfileParams> {
  final ProfileRepository repository;

  GetWorkerProfile(this.repository);

  @override
  Future<Either<Failure, WorkerProfile>> call(
    GetWorkerProfileParams params,
  ) async {
    return await repository.getWorkerProfile(params.profileId);
  }
}

class GetWorkerProfileParams {
  final String profileId;

  GetWorkerProfileParams({required this.profileId});
}
