import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/licenses_repository.dart';

class DeleteLicense {
  final LicensesRepository repository;
  DeleteLicense(this.repository);

  Future<Either<Failure, void>> call(String licenseId) =>
      repository.delete(licenseId);
}
