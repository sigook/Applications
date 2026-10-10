import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/certificates_repository.dart';

class UploadCertificate {
  final CertificatesRepository repository;
  UploadCertificate(this.repository);

  Future<Either<Failure, void>> call({
    required String filePath,
    required String description,
  }) =>
      repository.upload(filePath: filePath, description: description);
}
