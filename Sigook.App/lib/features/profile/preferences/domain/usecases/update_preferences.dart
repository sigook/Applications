import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/preferences_repository.dart';

class UpdatePreferences {
  final PreferencesRepository repository;
  UpdatePreferences(this.repository);

  Future<Either<Failure, void>> call(Map<String, String> fields) =>
      repository.update(fields);
}
