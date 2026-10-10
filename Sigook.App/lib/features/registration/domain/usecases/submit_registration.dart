import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/registration_form.dart';
import '../repositories/registration_repository.dart';

class SubmitRegistration implements UseCase<void, RegistrationForm> {
  final RegistrationRepository repository;

  SubmitRegistration(this.repository);

  @override
  Future<Either<Failure, void>> call(RegistrationForm params) async {
    if (!params.isComplete) {
      return const Left(
        ValidationFailure(message: 'Please complete all required fields'),
      );
    }

    return await repository.submitRegistration(params);
  }
}
