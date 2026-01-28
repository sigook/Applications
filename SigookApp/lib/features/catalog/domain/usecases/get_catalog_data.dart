import 'package:dartz/dartz.dart';
import '../../../../core/error/failures.dart';
import '../../../../core/usecases/usecase.dart';
import '../entities/catalog_item.dart';
import '../repositories/catalog_repository.dart';

class GetAvailability implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetAvailability(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getAvailability();
  }
}

class GetAvailabilityTime implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetAvailabilityTime(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getAvailabilityTime();
  }
}

class GetCountries implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetCountries(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getCountries();
  }
}

class GetGenders implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetGenders(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getGenders();
  }
}

class GetIdentificationTypes implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetIdentificationTypes(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getIdentificationTypes();
  }
}

class GetLanguages implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetLanguages(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getLanguages();
  }
}

class GetSkills implements UseCase<List<CatalogItem>, NoParams> {
  final CatalogRepository repository;

  GetSkills(this.repository);

  @override
  Future<Either<Failure, List<CatalogItem>>> call(NoParams params) async {
    return await repository.getSkills();
  }
}
