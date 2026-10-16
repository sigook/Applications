import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/documents_repository.dart';

class UpdateDocuments {
  final DocumentsRepository repository;
  UpdateDocuments(this.repository);

  Future<Either<Failure, void>> call(
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) =>
      repository.update(fields, filePaths: filePaths);
}
