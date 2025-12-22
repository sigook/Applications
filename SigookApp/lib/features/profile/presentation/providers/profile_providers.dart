import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/profile_remote_datasource.dart';
import '../../data/repositories/profile_repository_impl.dart';
import '../../domain/repositories/profile_repository.dart';
import '../../domain/usecases/get_worker_profile.dart';

final profileRemoteDataSourceProvider = Provider<ProfileRemoteDataSource>((
  ref,
) {
  return ProfileRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final profileRepositoryProvider = Provider<ProfileRepository>((ref) {
  return ProfileRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getWorkerProfileUseCaseProvider = Provider<GetWorkerProfile>((ref) {
  return GetWorkerProfile(ref.read(profileRepositoryProvider));
});
