import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../repositories/auth_repository.dart';

class ValidateToken implements UseCase<bool, ValidateTokenParams> {
  final AuthRepository repository;

  ValidateToken(this.repository);

  @override
  Future<Either<Failure, bool>> call(ValidateTokenParams params) async {
    return await repository.validateToken(params.accessToken);
  }
}

class ValidateTokenParams {
  final String accessToken;

  ValidateTokenParams({required this.accessToken});
}
