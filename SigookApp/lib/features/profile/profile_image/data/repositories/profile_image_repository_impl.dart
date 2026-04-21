import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/profile_repository_helpers.dart';
import '../datasources/profile_image_remote_datasource.dart';
import '../../domain/repositories/profile_image_repository.dart';

class ProfileImageRepositoryImpl implements ProfileImageRepository {
  final ProfileImageRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  ProfileImageRepositoryImpl({
    required this.datasource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, void>> upload(String filePath) =>
      guardedProfileCall(networkInfo, () async {
        final profile = await datasource.getWorkerProfile();
        await datasource.uploadProfileImage(profile.id, filePath: filePath);
      });
}
