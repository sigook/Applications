import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/catalog_item.dart';
import '../providers/catalog_providers.dart';

part 'catalog_notifiers.g.dart';

@riverpod
class Availability extends _$Availability {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getAvailabilityProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class AvailabilityTime extends _$AvailabilityTime {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getAvailabilityTimeProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class Countries extends _$Countries {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getCountriesProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class Genders extends _$Genders {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getGendersProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class IdentificationTypes extends _$IdentificationTypes {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getIdentificationTypesProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class Languages extends _$Languages {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getLanguagesProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}

@riverpod
class Skills extends _$Skills {
  @override
  Future<List<CatalogItem>> build() async {
    final useCase = ref.read(getSkillsProvider);
    final result = await useCase(NoParams());

    return result.fold(
      (failure) => throw Exception(failure.message),
      (data) => data,
    );
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(build);
  }
}
