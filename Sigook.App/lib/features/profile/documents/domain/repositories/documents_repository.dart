import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';

abstract class DocumentsRepository {
  Future<Either<Failure, void>> update(
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  });
}
