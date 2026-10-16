import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/account_settings_repository.dart';

class ChangeEmail {
  final AccountSettingsRepository repository;
  ChangeEmail(this.repository);

  Future<Either<Failure, void>> call({
    required String newEmail,
    required String confirmNewEmail,
  }) =>
      repository.changeEmail(
        newEmail: newEmail,
        confirmNewEmail: confirmNewEmail,
      );
}
