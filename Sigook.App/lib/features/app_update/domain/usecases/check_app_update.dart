import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/app_update_check.dart';
import '../repositories/app_update_repository.dart';

class CheckAppUpdate implements UseCase<AppUpdateCheck, String> {
  final AppUpdateRepository repository;

  CheckAppUpdate(this.repository);

  @override
  Future<Either<Failure, AppUpdateCheck>> call(String currentVersion) async {
    final result = await repository.getVersionRequirement();
    return result.map(
      (requirement) => AppUpdateCheck(
        currentVersion: currentVersion,
        requirement: requirement,
      ),
    );
  }
}
