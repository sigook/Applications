import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/job_details.dart';
import '../repositories/jobs_repository.dart';

class GetJobDetails implements UseCase<JobDetails, GetJobDetailsParams> {
  final JobsRepository repository;

  GetJobDetails(this.repository);

  @override
  Future<Either<Failure, JobDetails>> call(GetJobDetailsParams params) async {
    return await repository.getJobDetails(params.jobId);
  }
}

class GetJobDetailsParams {
  final String jobId;

  GetJobDetailsParams({required this.jobId});
}
