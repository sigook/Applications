import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/pay_stub_summary.dart';

abstract class PayStubsRepository {
  Future<Either<Failure, List<int>>> getYears();
  Future<Either<Failure, PayStubPage>> getPayStubs({
    required int year,
    required int pageIndex,
    required int pageSize,
  });
  Future<Either<Failure, double>> getYtdNet(int year);
  Future<Either<Failure, String>> downloadPdf(PayStubSummary payStub);
}
