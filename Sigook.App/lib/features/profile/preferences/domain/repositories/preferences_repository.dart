import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class PreferencesRepository {
  Future<Either<Failure, void>> update(Map<String, String> fields);
}
