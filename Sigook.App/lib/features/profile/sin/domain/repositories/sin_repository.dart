import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class SinRepository {
  Future<Either<Failure, void>> update(
    Map<String, String> fields, {
    String? filePath,
  });
}
