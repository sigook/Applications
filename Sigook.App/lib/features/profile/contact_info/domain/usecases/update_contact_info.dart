import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/contact_info_repository.dart';

class UpdateContactInfo {
  final ContactInfoRepository repository;
  UpdateContactInfo(this.repository);

  Future<Either<Failure, void>> call(Map<String, String> fields) =>
      repository.update(fields);
}
