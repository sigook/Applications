import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/catalog_item.dart';
import '../repositories/catalog_repository.dart';

class GetProvinces {
  final CatalogRepository repository;

  GetProvinces(this.repository);

  Future<Either<Failure, List<CatalogItem>>> call(String countryId) async {
    return await repository.getProvinces(countryId);
  }
}
