import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class LicensesRepository {
  Future<Either<Failure, void>> upload({
    required String filePath,
    required String description,
    required String? number,
    required String? issued,
    required String? expires,
  });

  Future<Either<Failure, void>> delete(String licenseId);
}
