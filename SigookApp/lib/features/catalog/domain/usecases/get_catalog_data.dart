import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/catalog_item.dart';
import '../repositories/catalog_repository.dart';

/// Use case for getting availability options
class GetAvailability implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetAvailability(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getAvailability();
  }
}

/// Use case for getting availability time options
class GetAvailabilityTime implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetAvailabilityTime(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getAvailabilityTime();
  }
}

/// Use case for getting country options
class GetCountries implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetCountries(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getCountries();
  }
}

/// Use case for getting gender options
class GetGenders implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetGenders(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getGenders();
  }
}

/// Use case for getting identification type options
class GetIdentificationTypes implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetIdentificationTypes(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getIdentificationTypes();
  }
}

/// Use case for getting language options
class GetLanguages implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetLanguages(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getLanguages();
  }
}

/// Use case for getting skills options
class GetSkills implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetSkills(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getSkills();
  }
}
