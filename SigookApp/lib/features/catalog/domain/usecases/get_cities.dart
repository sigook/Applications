import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/catalog_item.dart';
import '../repositories/catalog_repository.dart';

class GetCities {
  final CatalogRepository repository;

  GetCities(this.repository);

  Future<Either<Failure, List<CatalogItem>>> call(String provinceId) async {
    return await repository.getCities(provinceId);
  }
}
