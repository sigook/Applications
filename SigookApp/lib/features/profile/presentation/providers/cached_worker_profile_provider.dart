import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../auth/presentation/viewmodels/auth_viewmodel.dart';
import '../../domain/entities/worker_profile.dart';
import '../../domain/usecases/get_worker_profile.dart';
import 'profile_providers.dart';

final cachedWorkerProfileProvider = FutureProvider.autoDispose<WorkerProfile?>((
  ref,
) async {
  // DISABLED: Profile endpoint temporarily disabled, only logout functionality enabled
  // Return null to show blank fields in UI
  return null;

  // Original implementation commented out:
  // final authState = ref.watch(authViewModelProvider);
  // final profileId = authState.token?.userInfo?.sub;
  //
  // if (profileId == null) {
  //   return null;
  // }
  //
  // final useCase = ref.watch(getWorkerProfileUseCaseProvider);
  // final result = await useCase(GetWorkerProfileParams(profileId: profileId));
  //
  // return result.fold((failure) => null, (profile) => profile);
});
