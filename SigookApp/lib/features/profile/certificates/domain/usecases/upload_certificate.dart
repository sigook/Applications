import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/certificates_repository.dart';

class UploadCertificate {
  final CertificatesRepository repository;
  UploadCertificate(this.repository);

  Future<Either<Failure, void>> call(String filePath) =>
      repository.upload(filePath);
}
