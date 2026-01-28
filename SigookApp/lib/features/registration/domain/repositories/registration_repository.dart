import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/registration_form.dart';

abstract class RegistrationRepository {
  Future<Either<Failure, void>> submitRegistration(RegistrationForm form);

  Future<Either<Failure, void>> saveDraft(RegistrationForm form);

  Future<Either<Failure, RegistrationForm>> getDraft();

  Future<Either<Failure, void>> clearDraft();
}
