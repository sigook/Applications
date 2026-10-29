import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/pay_stub_summary.dart';
import '../repositories/pay_stubs_repository.dart';

class DownloadPayStubPdf {
  final PayStubsRepository repository;

  DownloadPayStubPdf(this.repository);

  Future<Either<Failure, String>> call(PayStubSummary payStub) =>
      repository.downloadPdf(payStub);
}
