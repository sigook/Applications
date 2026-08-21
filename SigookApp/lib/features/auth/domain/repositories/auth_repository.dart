import 'package:dartz/dartz.dart';
import 'package:sigook_app_flutter/core/error/failures.dart';
import 'package:sigook_app_flutter/features/auth/domain/entities/auth_token.dart';

abstract class AuthRepository {
  Future<Either<Failure, AuthToken>> signIn({
    required String email,
    required String password,
  });
  Future<Either<Failure, AuthToken>> refreshToken(String refreshToken);
  Future<Either<Failure, String>> getUserRole(String accessToken);
  Future<Either<Failure, void>> logout();
  Future<Either<Failure, void>> deactivateAccount(String accessToken);
  Future<Either<Failure, void>> requestPasswordResetCode(String email);
  Future<Either<Failure, void>> resetPassword({
    required String email,
    required String code,
    required String newPassword,
  });
  Future<Either<Failure, void>> resendConfirmationLink(String email);
}
