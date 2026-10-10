import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../../domain/repositories/account_settings_repository.dart';
import '../datasources/account_settings_remote_datasource.dart';

class AccountSettingsRepositoryImpl implements AccountSettingsRepository {
  final AccountSettingsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  AccountSettingsRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> changeEmail({
    required String newEmail,
    required String confirmNewEmail,
  }) =>
      guardedProfileCall(networkInfo, () async {
        await datasource.changeEmail(
          newEmail: newEmail,
          confirmNewEmail: confirmNewEmail,
        );
      });
}
