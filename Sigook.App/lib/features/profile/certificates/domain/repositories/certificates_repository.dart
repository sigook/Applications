import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class CertificatesRepository {
  Future<Either<Failure, void>> upload({
    required String filePath,
    required String description,
  });
  Future<Either<Failure, void>> delete(String certificateId);
}
