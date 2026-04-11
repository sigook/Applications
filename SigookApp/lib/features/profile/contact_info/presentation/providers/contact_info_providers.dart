import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../presentation/providers/profile_providers.dart';
import '../../data/repositories/contact_info_repository_impl.dart';
import '../../domain/repositories/contact_info_repository.dart';
import '../../domain/usecases/update_contact_info.dart';

final contactInfoRepositoryProvider = Provider<ContactInfoRepository>((ref) {
  return ContactInfoRepositoryImpl(
    remoteDataSource: ref.read(profileRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateContactInfoUseCaseProvider = Provider<UpdateContactInfo>((ref) {
  return UpdateContactInfo(ref.read(contactInfoRepositoryProvider));
});
