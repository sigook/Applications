import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class EmergencyRepository {
  Future<Either<Failure, void>> update(Map<String, String> fields);
}
