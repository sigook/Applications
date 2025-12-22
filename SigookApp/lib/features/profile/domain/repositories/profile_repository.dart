import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/worker_profile.dart';

abstract class ProfileRepository {
  Future<Either<Failure, WorkerProfile>> getWorkerProfile(String profileId);
}
