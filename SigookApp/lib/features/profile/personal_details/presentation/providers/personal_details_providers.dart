import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/personal_details_repository_impl.dart';
import '../../domain/repositories/personal_details_repository.dart';
import '../../domain/usecases/update_personal_details.dart';

final personalDetailsRepositoryProvider =
    Provider<PersonalDetailsRepository>((ref) {
  return PersonalDetailsRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updatePersonalDetailsUseCaseProvider =
    Provider<UpdatePersonalDetails>((ref) {
  return UpdatePersonalDetails(
    ref.read(personalDetailsRepositoryProvider),
  );
});
