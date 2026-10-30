import 'package:dartz/dartz.dart';
import '../../../../../core/error/failures.dart';
import '../repositories/pay_stubs_repository.dart';

class GetPayStubYears {
  final PayStubsRepository repository;

  GetPayStubYears(this.repository);

  Future<Either<Failure, List<int>>> call() => repository.getYears();
}
