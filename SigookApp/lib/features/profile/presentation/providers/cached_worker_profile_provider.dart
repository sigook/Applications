import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/worker_profile.dart';
import 'profile_providers.dart';

final cachedWorkerProfileProvider = FutureProvider.autoDispose<WorkerProfile?>((
  ref,
) async {
  final useCase = ref.watch(getWorkerProfileUseCaseProvider);
  final result = await useCase(NoParams());

  return result.fold((failure) => null, (profile) => profile);
});
