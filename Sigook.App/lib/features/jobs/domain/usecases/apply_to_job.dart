import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/jobs_repository.dart';

class ApplyToJob implements UseCase<void, ApplyToJobParams> {
  final JobsRepository repository;

  ApplyToJob(this.repository);

  @override
  Future<Either<Failure, void>> call(ApplyToJobParams params) async {
    return await repository.applyToJob(params.jobId);
  }
}

class ApplyToJobParams {
  final String jobId;

  ApplyToJobParams({required this.jobId});
}
