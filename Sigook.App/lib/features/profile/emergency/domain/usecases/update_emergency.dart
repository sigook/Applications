import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/emergency_repository.dart';

class UpdateEmergency {
  final EmergencyRepository repository;
  UpdateEmergency(this.repository);

  Future<Either<Failure, void>> call(Map<String, String> fields) =>
      repository.update(fields);
}
