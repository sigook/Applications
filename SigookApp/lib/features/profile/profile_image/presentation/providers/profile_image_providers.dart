import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/profile_image_remote_datasource.dart';
import '../../data/repositories/profile_image_repository_impl.dart';
import '../../domain/repositories/profile_image_repository.dart';
import '../../domain/usecases/upload_profile_image.dart';

final profileImageDatasourceProvider =
    Provider<ProfileImageRemoteDataSource>((ref) {
  return ProfileImageRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final profileImageRepositoryProvider =
    Provider<ProfileImageRepository>((ref) {
  return ProfileImageRepositoryImpl(
    datasource: ref.read(profileImageDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final uploadProfileImageUseCaseProvider =
    Provider<UploadProfileImage>((ref) {
  return UploadProfileImage(ref.read(profileImageRepositoryProvider));
});
