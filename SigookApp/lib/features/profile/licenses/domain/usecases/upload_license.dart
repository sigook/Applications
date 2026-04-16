import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/licenses_repository.dart';

class UploadLicense {
  final LicensesRepository repository;
  UploadLicense(this.repository);

  Future<Either<Failure, void>> call({
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  }) =>
      repository.upload(
        filePath: filePath,
        number: number,
        issued: issued,
        expires: expires,
      );
}
