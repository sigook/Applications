import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/contact_info_remote_datasource.dart';
import '../../data/repositories/contact_info_repository_impl.dart';
import '../../domain/repositories/contact_info_repository.dart';
import '../../domain/usecases/update_contact_info.dart';

final contactInfoDatasourceProvider =
    Provider<ContactInfoRemoteDataSource>((ref) {
  return ContactInfoRemoteDataSource(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final contactInfoRepositoryProvider = Provider<ContactInfoRepository>((ref) {
  return ContactInfoRepositoryImpl(
    datasource: ref.read(contactInfoDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final updateContactInfoUseCaseProvider = Provider<UpdateContactInfo>((ref) {
  return UpdateContactInfo(ref.read(contactInfoRepositoryProvider));
});
