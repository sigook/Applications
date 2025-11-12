import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../entities/catalog_item.dart';

/// Repository interface for catalog data
/// Defines contract for fetching catalog items from various sources
abstract class CatalogRepository {
  /// Get availability options
  Future<Either<Failure, List<CatalogItem>>> getAvailability();

  /// Get availability time options
  Future<Either<Failure, List<CatalogItem>>> getAvailabilityTime();

  /// Get country options
  Future<Either<Failure, List<CatalogItem>>> getCountries();

  /// Get provinces by country ID
  Future<Either<Failure, List<CatalogItem>>> getProvinces(String countryId);

  /// Get cities by province ID
  Future<Either<Failure, List<CatalogItem>>> getCities(String provinceId);

  /// Get gender options
  Future<Either<Failure, List<CatalogItem>>> getGenders();

  /// Get identification type options
  Future<Either<Failure, List<CatalogItem>>> getIdentificationTypes();

  /// Get language options
  Future<Either<Failure, List<CatalogItem>>> getLanguages();

  /// Get skills options
  Future<Either<Failure, List<CatalogItem>>> getSkills();
}
