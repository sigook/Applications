import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/sin_repository_impl.dart';
import '../../domain/repositories/sin_repository.dart';
import '../../domain/usecases/update_sin.dart';

final sinRepositoryProvider = Provider<SinRepository>((ref) {
  return SinRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateSinUseCaseProvider = Provider<UpdateSin>((ref) {
  return UpdateSin(ref.read(sinRepositoryProvider));
});
