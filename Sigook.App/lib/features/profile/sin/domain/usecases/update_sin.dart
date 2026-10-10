import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/sin_repository.dart';

class UpdateSin {
  final SinRepository repository;
  UpdateSin(this.repository);

  Future<Either<Failure, void>> call(
    Map<String, String> fields, {
    String? filePath,
  }) =>
      repository.update(fields, filePath: filePath);
}
