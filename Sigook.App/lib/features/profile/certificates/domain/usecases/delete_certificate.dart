import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/certificates_repository.dart';

class DeleteCertificate {
  final CertificatesRepository repository;
  DeleteCertificate(this.repository);

  Future<Either<Failure, void>> call(String certificateId) =>
      repository.delete(certificateId);
}
