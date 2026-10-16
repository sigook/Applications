import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/other_documents_repository.dart';

class UploadOtherDocument {
  final OtherDocumentsRepository repository;
  UploadOtherDocument(this.repository);

  Future<Either<Failure, void>> call({
    required String filePath,
    required String description,
  }) =>
      repository.upload(filePath: filePath, description: description);
}
