import 'dart:io';
import 'package:dartz/dartz.dart';
import 'package:path_provider/path_provider.dart';
import '../../../../../core/error/failures.dart';
import '../../../../../core/network/network_info.dart';
import '../../../data/repositories/activity_repository_helpers.dart';
import '../../domain/entities/pay_stub_summary.dart';
import '../../domain/repositories/pay_stubs_repository.dart';
import '../datasources/pay_stubs_remote_datasource.dart';

class PayStubsRepositoryImpl implements PayStubsRepository {
  final PayStubsRemoteDataSource datasource;
  final NetworkInfo networkInfo;

  PayStubsRepositoryImpl({required this.datasource, required this.networkInfo});

  @override
  Future<Either<Failure, List<int>>> getYears() =>
      guardedActivityCall(networkInfo, datasource.getYears);

  @override
  Future<Either<Failure, PayStubPage>> getPayStubs({
    required int year,
    required int pageIndex,
    required int pageSize,
  }) =>
      guardedActivityCall(networkInfo, () async {
        final response = await datasource.getPayStubs(
          year: year,
          pageIndex: pageIndex,
          pageSize: pageSize,
        );
        return PayStubPage(
          items: response.items.map((m) => m.toEntity()).toList(),
          pageIndex: response.pageIndex,
          totalCount: response.totalItems,
          hasMore: response.pageIndex < response.totalPages,
        );
      });

  @override
  Future<Either<Failure, double>> getYtdNet(int year) =>
      guardedActivityCall(networkInfo, () => datasource.getYtdNet(year));

  @override
  Future<Either<Failure, String>> downloadPdf(PayStubSummary payStub) =>
      guardedActivityCall(networkInfo, () async {
        final bytes = await datasource.getPdf(payStub.id);
        final directory = await getTemporaryDirectory();
        final name = payStub.number.isEmpty ? payStub.id : payStub.number;
        final file = File('${directory.path}/PayStub-$name.pdf');
        await file.writeAsBytes(bytes, flush: true);
        return file.path;
      });
}
