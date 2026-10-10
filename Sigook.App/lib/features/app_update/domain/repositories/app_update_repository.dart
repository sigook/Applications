import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/app_version_requirement.dart';

abstract class AppUpdateRepository {
  Future<Either<Failure, AppVersionRequirement>> getVersionRequirement();
}
