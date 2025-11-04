import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/registration_form.dart';

/// Repository interface for registration operations
/// Following Dependency Inversion Principle
abstract class RegistrationRepository {
  /// Submits the complete registration form
  Future<Either<Failure, void>> submitRegistration(RegistrationForm form);

  /// Saves form data locally (draft)
  Future<Either<Failure, void>> saveDraft(RegistrationForm form);

  /// Retrieves saved draft
  Future<Either<Failure, RegistrationForm>> getDraft();

  /// Clears saved draft
  Future<Either<Failure, void>> clearDraft();

  /// Validates email availability
  Future<Either<Failure, bool>> isEmailAvailable(String email);
}
