import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/auth_repository.dart';

class ResendConfirmationLinkParams {
  final String email;

  ResendConfirmationLinkParams({required this.email});
}

class ResendConfirmationLink
    implements UseCase<void, ResendConfirmationLinkParams> {
  final AuthRepository repository;

  ResendConfirmationLink(this.repository);

  @override
  Future<Either<Failure, void>> call(ResendConfirmationLinkParams params) {
    return repository.resendConfirmationLink(params.email);
  }
}
