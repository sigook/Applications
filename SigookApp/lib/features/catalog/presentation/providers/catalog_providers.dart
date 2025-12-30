import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/providers/core_providers.dart';
import '../../../../core/usecases/usecase.dart';
import '../../data/datasources/catalog_remote_datasource.dart';
import '../../data/repositories/catalog_repository_impl.dart';
import '../../domain/entities/catalog_item.dart';
import '../../domain/repositories/catalog_repository.dart';
import '../../domain/usecases/get_catalog_data.dart';
import '../../domain/usecases/get_provinces.dart';
import '../../domain/usecases/get_cities.dart';

/// Provider for remote data source
final catalogRemoteDataSourceProvider = Provider<CatalogRemoteDataSource>((
  ref,
) {
  return CatalogRemoteDataSourceImpl(apiClient: ref.read(apiClientProvider));
});

/// Provider for catalog repository
final catalogRepositoryProvider = Provider<CatalogRepository>((ref) {
  return CatalogRepositoryImpl(
    remoteDataSource: ref.read(catalogRemoteDataSourceProvider),
    networkInfo: ref.read(networkInfoProvider),
  );
});

// Use Case Providers

final getAvailabilityProvider = Provider<GetAvailability>((ref) {
  return GetAvailability(ref.read(catalogRepositoryProvider));
});

final getAvailabilityTimeProvider = Provider<GetAvailabilityTime>((ref) {
  return GetAvailabilityTime(ref.read(catalogRepositoryProvider));
});

final getCountriesProvider = Provider<GetCountries>((ref) {
  return GetCountries(ref.read(catalogRepositoryProvider));
});

final getGendersProvider = Provider<GetGenders>((ref) {
  return GetGenders(ref.read(catalogRepositoryProvider));
});

final getIdentificationTypesProvider = Provider<GetIdentificationTypes>((ref) {
  return GetIdentificationTypes(ref.read(catalogRepositoryProvider));
});

final getLanguagesProvider = Provider<GetLanguages>((ref) {
  return GetLanguages(ref.read(catalogRepositoryProvider));
});

final getSkillsProvider = Provider<GetSkills>((ref) {
  return GetSkills(ref.read(catalogRepositoryProvider));
});

final getProvincesProvider = Provider<GetProvinces>((ref) {
  return GetProvinces(ref.read(catalogRepositoryProvider));
});

final getCitiesProvider = Provider<GetCities>((ref) {
  return GetCities(ref.read(catalogRepositoryProvider));
});

// State Providers for catalog data

final availabilityListProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final useCase = ref.read(getAvailabilityProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final availabilityTimeListProvider = FutureProvider<List<CatalogItem>>((
  ref,
) async {
  final useCase = ref.read(getAvailabilityTimeProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final countriesListProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final useCase = ref.read(getCountriesProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final gendersProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final useCase = ref.read(getGendersProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final identificationTypesListProvider = FutureProvider<List<CatalogItem>>((
  ref,
) async {
  final useCase = ref.read(getIdentificationTypesProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final languagesProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final useCase = ref.read(getLanguagesProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final skillsProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final useCase = ref.read(getSkillsProvider);
  final result = await useCase(NoParams());
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

/// Provinces provider - requires countryId parameter
final provincesProvider = FutureProvider.family<List<CatalogItem>, String>((
  ref,
  countryId,
) async {
  final useCase = ref.read(getProvincesProvider);
  final result = await useCase(countryId);
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

/// Cities provider - requires provinceId parameter
final citiesProvider = FutureProvider.family<List<CatalogItem>, String>((
  ref,
  provinceId,
) async {
  final useCase = ref.read(getCitiesProvider);
  final result = await useCase(provinceId);
  return result.fold(
    (failure) => throw Exception(failure.message),
    (data) => data,
  );
});

final daysOfWeekProvider = FutureProvider<List<CatalogItem>>((ref) async {
  final catalogRepository = ref.watch(catalogRepositoryProvider);
  final result = await catalogRepository.getDaysOfWeek();
  return result.fold(
    (failure) => throw Exception(failure.message),
    (days) => days,
  );
});

/// Lifting capacities provider
final liftingCapacitiesProvider = FutureProvider<List<CatalogItem>>((
  ref,
) async {
  final catalogRepository = ref.watch(catalogRepositoryProvider);
  final result = await catalogRepository.getLiftingCapacities();
  return result.fold(
    (failure) => throw Exception(failure.message),
    (capacities) => capacities,
  );
});
