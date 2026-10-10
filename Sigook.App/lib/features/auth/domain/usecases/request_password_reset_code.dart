import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/auth_repository.dart';

class RequestPasswordResetCodeParams {
  final String email;

  RequestPasswordResetCodeParams({required this.email});
}

class RequestPasswordResetCode
    implements UseCase<void, RequestPasswordResetCodeParams> {
  final AuthRepository repository;

  RequestPasswordResetCode(this.repository);

  @override
  Future<Either<Failure, void>> call(RequestPasswordResetCodeParams params) {
    return repository.requestPasswordResetCode(params.email);
  }
}
