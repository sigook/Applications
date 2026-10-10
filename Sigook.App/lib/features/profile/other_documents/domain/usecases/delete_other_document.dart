import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/other_documents_repository.dart';

class DeleteOtherDocument {
  final OtherDocumentsRepository repository;
  DeleteOtherDocument(this.repository);

  Future<Either<Failure, void>> call(String documentId) =>
      repository.delete(documentId);
}
