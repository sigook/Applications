import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/personal_details_repository.dart';

class UpdatePersonalDetails {
  final PersonalDetailsRepository repository;
  UpdatePersonalDetails(this.repository);

  Future<Either<Failure, void>> call(Map<String, String> fields) =>
      repository.update(fields);
}
