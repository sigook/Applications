import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/catalog_item.dart';

abstract class CatalogRepository {
  Future<Either<Failure, List<CatalogItem>>> getAvailability();

  Future<Either<Failure, List<CatalogItem>>> getAvailabilityTime();

  Future<Either<Failure, List<CatalogItem>>> getCountries();

  Future<Either<Failure, List<CatalogItem>>> getProvinces(String countryId);

  Future<Either<Failure, List<CatalogItem>>> getCities(String provinceId);

  Future<Either<Failure, List<CatalogItem>>> getGenders();

  Future<Either<Failure, List<CatalogItem>>> getIdentificationTypes();

  Future<Either<Failure, List<CatalogItem>>> getLanguages();

  Future<Either<Failure, List<CatalogItem>>> getSkills();

  Future<Either<Failure, List<CatalogItem>>> getLiftingCapacities();

  Future<Either<Failure, List<CatalogItem>>> getDaysOfWeek();
}
