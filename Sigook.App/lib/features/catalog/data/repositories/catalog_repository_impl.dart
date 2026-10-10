import 'package:dartz/dartz.dart';
import '../../../../core/error/exceptions.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/network/network_info.dart';
import '../../domain/entities/catalog_item.dart';
import '../../domain/repositories/catalog_repository.dart';
import '../datasources/catalog_remote_datasource.dart';

class CatalogRepositoryImpl implements CatalogRepository {
  final CatalogRemoteDataSource remoteDataSource;
  final NetworkInfo networkInfo;

  CatalogRepositoryImpl({
    required this.remoteDataSource,
    required this.networkInfo,
  });

  @override
  Future<Either<Failure, List<CatalogItem>>> getAvailability() async {
    return _getCatalogData(() => remoteDataSource.getAvailability());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getAvailabilityTime() async {
    return _getCatalogData(() => remoteDataSource.getAvailabilityTime());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getCountries() async {
    return _getCatalogData(() => remoteDataSource.getCountries());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getProvinces(
    String countryId,
  ) async {
    return _getCatalogData(() => remoteDataSource.getProvinces(countryId));
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getCities(
    String provinceId,
  ) async {
    return _getCatalogData(() => remoteDataSource.getCities(provinceId));
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getGenders() async {
    return _getCatalogData(() => remoteDataSource.getGenders());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getIdentificationTypes() async {
    return _getCatalogData(() => remoteDataSource.getIdentificationTypes());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getLanguages() async {
    return _getCatalogData(() => remoteDataSource.getLanguages());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getSkills() async {
    return _getCatalogData(() => remoteDataSource.getSkills());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getLiftingCapacities() async {
    return _getCatalogData(() => remoteDataSource.getLiftingCapacities());
  }

  @override
  Future<Either<Failure, List<CatalogItem>>> getDaysOfWeek() async {
    return _getCatalogData(() => remoteDataSource.getDaysOfWeek());
  }

  Future<Either<Failure, List<CatalogItem>>> _getCatalogData(
    Future<List<CatalogItem>> Function() getData,
  ) async {
    if (!await networkInfo.isConnected) {
      return Left(NetworkFailure());
    }

    try {
      final items = await getData();
      return Right(items);
    } on ServerException catch (e) {
      return Left(ServerFailure(message: e.message));
    } on NetworkException catch (e) {
      return Left(NetworkFailure(message: e.message));
    } on ParseException catch (e) {
      return Left(ParseFailure(message: e.message));
    } catch (e) {
      return Left(ServerFailure(message: 'Unexpected error: ${e.toString()}'));
    }
  }
}
