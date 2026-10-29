import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/usecases/usecase.dart';
import '../../domain/entities/worker_profile.dart';
import 'profile_providers.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';

final cachedWorkerProfileProvider = FutureProvider<WorkerProfile?>((ref) async {
  ref.watch(sessionUserIdProvider);
  final useCase = ref.watch(getWorkerProfileUseCaseProvider);
  final result = await useCase(NoParams());

  return result.fold((failure) => null, (profile) => profile);
});
