import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/sin_remote_datasource.dart';
import '../../data/repositories/sin_repository_impl.dart';
import '../../domain/repositories/sin_repository.dart';
import '../../domain/usecases/update_sin.dart';

final sinDatasourceProvider = Provider<SinRemoteDataSource>((ref) {
  return SinRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final sinRepositoryProvider = Provider<SinRepository>((ref) {
  return SinRepositoryImpl(
    datasource: ref.read(sinDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateSinUseCaseProvider = Provider<UpdateSin>((ref) {
  return UpdateSin(ref.read(sinRepositoryProvider));
});
