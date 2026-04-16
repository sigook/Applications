import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/emergency_remote_datasource.dart';
import '../../data/repositories/emergency_repository_impl.dart';
import '../../domain/repositories/emergency_repository.dart';
import '../../domain/usecases/update_emergency.dart';

final emergencyDatasourceProvider =
    Provider<EmergencyRemoteDataSource>((ref) {
  return EmergencyRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final emergencyRepositoryProvider = Provider<EmergencyRepository>((ref) {
  return EmergencyRepositoryImpl(
    datasource: ref.read(emergencyDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateEmergencyUseCaseProvider = Provider<UpdateEmergency>((ref) {
  return UpdateEmergency(ref.read(emergencyRepositoryProvider));
});
