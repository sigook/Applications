import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class LicensesRepository {
  Future<Either<Failure, void>> upload({
    required String filePath,
    required String number,
    required String issued,
    required String expires,
  });
}
