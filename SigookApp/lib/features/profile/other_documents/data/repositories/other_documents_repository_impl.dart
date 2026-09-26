import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/other_documents_remote_datasource.dart';
import '../../domain/repositories/other_documents_repository.dart';

class OtherDocumentsRepositoryImpl implements OtherDocumentsRepository {
  final OtherDocumentsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  OtherDocumentsRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload({
    required String filePath,
    required String description,
  }) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.uploadOtherDocument(
          profile.id,
          filePath: filePath,
          description: description,
        );
      });

  @override
  Future<Either<Failure, void>> delete(String documentId) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.deleteOtherDocument(profile.id, documentId);
      });
}
