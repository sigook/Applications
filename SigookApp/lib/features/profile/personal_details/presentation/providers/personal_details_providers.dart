import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/personal_details_remote_datasource.dart';
import '../../data/repositories/personal_details_repository_impl.dart';
import '../../domain/repositories/personal_details_repository.dart';
import '../../domain/usecases/update_personal_details.dart';

final personalDetailsDatasourceProvider =
    Provider<PersonalDetailsRemoteDataSource>((ref) {
  return PersonalDetailsRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final personalDetailsRepositoryProvider =
    Provider<PersonalDetailsRepository>((ref) {
  return PersonalDetailsRepositoryImpl(
    datasource: ref.read(personalDetailsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updatePersonalDetailsUseCaseProvider =
    Provider<UpdatePersonalDetails>((ref) {
  return UpdatePersonalDetails(ref.read(personalDetailsRepositoryProvider));
});
