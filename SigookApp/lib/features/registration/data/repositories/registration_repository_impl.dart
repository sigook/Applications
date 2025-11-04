import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../domain/entities/registration_form.dart';
import '../../domain/repositories/registration_repository.dart';
import '../datasources/registration_local_datasource.dart';
import '../models/registration_form_model.dart';

class RegistrationRepositoryImpl implements RegistrationRepository {
  final RegistrationLocalDataSource localDataSource;

  RegistrationRepositoryImpl({required this.localDataSource});

  @override
  Future<Either<Failure, void>> submitRegistration(
      RegistrationForm form) async {
    try {
      // TODO: Implement API call to submit registration
      // For now, just simulate success
      await Future.delayed(const Duration(seconds: 2));
      
      // Clear draft after successful submission
      await localDataSource.clearDraft();
      
      return const Right(null);
    } catch (e) {
      return Left(ServerFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> saveDraft(RegistrationForm form) async {
    try {
      final model = RegistrationFormModel.fromEntity(form);
      await localDataSource.saveDraft(model);
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, RegistrationForm>> getDraft() async {
    try {
      final model = await localDataSource.getDraft();
      if (model == null) {
        return Right(RegistrationForm.empty());
      }
      return Right(model.toEntity());
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, void>> clearDraft() async {
    try {
      await localDataSource.clearDraft();
      return const Right(null);
    } catch (e) {
      return Left(CacheFailure(message: e.toString()));
    }
  }

  @override
  Future<Either<Failure, bool>> isEmailAvailable(String email) async {
    try {
      // TODO: Implement API call to check email availability
      // For now, simulate check
      await Future.delayed(const Duration(seconds: 1));
      
      // Simulate: email is available if it doesn't contain "test"
      final isAvailable = !email.toLowerCase().contains('test');
      return Right(isAvailable);
    } catch (e) {
      return Left(ServerFailure(message: e.toString()));
    }
  }
}
