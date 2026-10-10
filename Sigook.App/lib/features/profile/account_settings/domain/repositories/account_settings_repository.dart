import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class AccountSettingsRepository {
  Future<Either<Failure, void>> changeEmail({
    required String newEmail,
    required String confirmNewEmail,
  });
}
