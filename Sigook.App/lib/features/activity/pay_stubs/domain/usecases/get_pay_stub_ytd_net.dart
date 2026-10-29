import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/pay_stubs_repository.dart';

class GetPayStubYtdNet {
  final PayStubsRepository repository;

  GetPayStubYtdNet(this.repository);

  Future<Either<Failure, double>> call(int year) => repository.getYtdNet(year);
}
