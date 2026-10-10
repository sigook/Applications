import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/other_documents_remote_datasource.dart';
import '../../data/repositories/other_documents_repository_impl.dart';
import '../../domain/repositories/other_documents_repository.dart';
import '../../domain/usecases/delete_other_document.dart';
import '../../domain/usecases/upload_other_document.dart';

final otherDocumentsDatasourceProvider =
    Provider<OtherDocumentsRemoteDataSource>((ref) {
  return OtherDocumentsRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final otherDocumentsRepositoryProvider =
    Provider<OtherDocumentsRepository>((ref) {
  return OtherDocumentsRepositoryImpl(
    datasource: ref.read(otherDocumentsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadOtherDocumentUseCaseProvider =
    Provider<UploadOtherDocument>((ref) {
  return UploadOtherDocument(ref.read(otherDocumentsRepositoryProvider));
});

final deleteOtherDocumentUseCaseProvider =
    Provider<DeleteOtherDocument>((ref) {
  return DeleteOtherDocument(ref.read(otherDocumentsRepositoryProvider));
});
