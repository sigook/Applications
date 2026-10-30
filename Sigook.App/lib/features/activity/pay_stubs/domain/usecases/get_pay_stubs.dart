import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../entities/pay_stub_summary.dart';
import '../repositories/pay_stubs_repository.dart';

class GetPayStubs {
  final PayStubsRepository repository;

  GetPayStubs(this.repository);

  Future<Either<Failure, PayStubPage>> call({
    required int year,
    int pageIndex = 1,
    int pageSize = 30,
  }) =>
      repository.getPayStubs(year: year, pageIndex: pageIndex, pageSize: pageSize);
}
