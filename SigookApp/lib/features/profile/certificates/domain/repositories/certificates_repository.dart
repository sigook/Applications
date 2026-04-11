import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class CertificatesRepository {
  Future<Either<Failure, void>> upload(String filePath);
}
