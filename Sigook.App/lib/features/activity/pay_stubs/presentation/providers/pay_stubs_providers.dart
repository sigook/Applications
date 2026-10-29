import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../../core/providers/core_providers.dart';
import '../../../../auth/presentation/providers/auth_providers.dart';
import '../../data/datasources/pay_stubs_remote_datasource.dart';
import '../../data/repositories/pay_stubs_repository_impl.dart';
import '../../domain/repositories/pay_stubs_repository.dart';
import '../../domain/usecases/download_pay_stub_pdf.dart';
import '../../domain/usecases/get_pay_stub_years.dart';
import '../../domain/usecases/get_pay_stub_ytd_net.dart';
import '../../domain/usecases/get_pay_stubs.dart';

final payStubsDatasourceProvider = Provider<PayStubsRemoteDataSource>((ref) {
  return PayStubsRemoteDataSourceImpl(
    apiClient: ref.read(authenticatedApiClientProvider),
  );
});

final payStubsRepositoryProvider = Provider<PayStubsRepository>((ref) {
  return PayStubsRepositoryImpl(
    datasource: ref.read(payStubsDatasourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

final getPayStubYearsUseCaseProvider = Provider<GetPayStubYears>((ref) {
  return GetPayStubYears(ref.read(payStubsRepositoryProvider));
});

final getPayStubsUseCaseProvider = Provider<GetPayStubs>((ref) {
  return GetPayStubs(ref.read(payStubsRepositoryProvider));
});

final getPayStubYtdNetUseCaseProvider = Provider<GetPayStubYtdNet>((ref) {
  return GetPayStubYtdNet(ref.read(payStubsRepositoryProvider));
});

final downloadPayStubPdfUseCaseProvider = Provider<DownloadPayStubPdf>((ref) {
  return DownloadPayStubPdf(ref.read(payStubsRepositoryProvider));
});
