import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/models/worker_profile_model.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/documents_remote_datasource.dart';
import '../../domain/repositories/documents_repository.dart';

class DocumentsRepositoryImpl implements DocumentsRepository {
  final DocumentsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  DocumentsRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> update(
    Map<String, String> fields, {
    Map<String, String>? filePaths,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final mutableFields = Map<String, String>.from(fields);
        final deleteId1File = mutableFields.remove('_deleteId1File') == 'true';
        final deleteId2File = mutableFields.remove('_deleteId2File') == 'true';

        final current = await datasource.getWorkerProfile();
        final updated = current.copyWith(
          identificationNumber1: mutableFields['identificationNumber1'] ??
              current.identificationNumber1,
          identificationNumber2: mutableFields['identificationNumber2'] ??
              current.identificationNumber2,
          identificationType1:
              mutableFields.containsKey('identificationType1Id')
                  ? CatalogItemModel(
                      id: mutableFields['identificationType1Id'],
                      value: mutableFields['identificationType1Value'],
                    )
                  : current.identificationType1,
          identificationType2:
              mutableFields.containsKey('identificationType2Id')
                  ? CatalogItemModel(
                      id: mutableFields['identificationType2Id'],
                      value: mutableFields['identificationType2Value'],
                    )
                  : current.identificationType2,
          identificationType1File:
              deleteId1File ? null : current.identificationType1File,
          identificationType2File:
              deleteId2File ? null : current.identificationType2File,
        );
        await datasource.updateDocuments(
          current.id,
          updated,
          newFilePaths: filePaths,
        );
      });
}
