import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/documents_remote_datasource.dart';
import '../../data/repositories/documents_repository_impl.dart';
import '../../domain/repositories/documents_repository.dart';
import '../../domain/usecases/update_documents.dart';

final documentsDatasourceProvider =
    Provider<DocumentsRemoteDataSource>((ref) {
  return DocumentsRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final documentsRepositoryProvider = Provider<DocumentsRepository>((ref) {
  return DocumentsRepositoryImpl(
    datasource: ref.read(documentsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateDocumentsUseCaseProvider = Provider<UpdateDocuments>((ref) {
  return UpdateDocuments(ref.read(documentsRepositoryProvider));
});
